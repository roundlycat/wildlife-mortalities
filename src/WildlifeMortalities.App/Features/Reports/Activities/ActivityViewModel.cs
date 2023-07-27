﻿using System.Reflection;
using FluentValidation;
using FluentValidation.Validators;
using WildlifeMortalities.App.Features.Shared.Mortalities;
using WildlifeMortalities.Data.Entities.Reports.SingleMortality;
using WildlifeMortalities.Data.Enums;
using WildlifeMortalities.Shared.Services;

namespace WildlifeMortalities.App.Features.Reports;

public abstract class ActivityViewModel
{
    protected ActivityViewModel() =>
        MortalityWithSpeciesSelectionViewModel = new MortalityWithSpeciesSelectionViewModel();

    public bool HasNoViolations { get; }
    public bool HasOnlyPotentiallyIllegalViolations { get; }
    public bool HasIllegalViolations { get; }

    public Activity? Activity { get; }

    protected ActivityViewModel(Activity activity, ReportDetail? reportDetail = null)
    {
        Activity = activity;

        MortalityWithSpeciesSelectionViewModel = new MortalityWithSpeciesSelectionViewModel
        {
            Species = activity.Mortality.Species,
            MortalityViewModel = MortalityViewModel.Create(activity.Mortality, reportDetail)
        };

        Comment = activity.Comment;

        HasNoViolations = activity.Violations.Count == 0;
        HasOnlyPotentiallyIllegalViolations = activity.Violations.All(
            x => x.Severity == Data.Entities.Violation.SeverityType.PotentiallyIllegal
        );
        HasIllegalViolations = activity.Violations.Any(
            x => x.Severity == Data.Entities.Violation.SeverityType.Illegal
        );
    }

    protected ActivityViewModel(ActivityViewModel viewModel, Species species)
    {
        Activity = viewModel.Activity;
        Comment = viewModel.Comment;

        IsCompleted = viewModel.IsCompleted;
        MortalityWithSpeciesSelectionViewModel = new MortalityWithSpeciesSelectionViewModel
        {
            Species = species,
            MortalityViewModel = MortalityViewModel.Create(
                species,
                viewModel.MortalityWithSpeciesSelectionViewModel!.MortalityViewModel
            )
        };
    }

    public static ActivityViewModel Create(int activityId, ReportDetail reportDetail)
    {
        var activity = reportDetail.Report.GetActivities().First(x => x.Id == activityId);
        return activity switch
        {
            HuntedActivity huntedActivity
                => new HuntedActivityViewModel(huntedActivity, reportDetail),
            TrappedActivity trappedActivity
                => new TrappedActivityViewModel(trappedActivity, reportDetail),
            _ => throw new System.Diagnostics.UnreachableException()
        };
    }

    public bool IsCompleted { get; set; }
    public string Comment { get; set; } = string.Empty;

    public MortalityWithSpeciesSelectionViewModel MortalityWithSpeciesSelectionViewModel { get; set; }
}

public abstract class ActivityViewModelValidator<T> : AbstractValidator<T>
    where T : ActivityViewModel
{
    protected ActivityViewModelValidator()
    {
        RuleFor(x => x.Comment).MaximumLength(1000);

        RuleFor(x => x.MortalityWithSpeciesSelectionViewModel.Species)
            .NotNull()
            .WithMessage("Please select a species.");

        RuleFor(x => x.MortalityWithSpeciesSelectionViewModel.MortalityViewModel)
            .NotNull()
            .When(x => x.MortalityWithSpeciesSelectionViewModel.Species != null)
            .SetInheritanceValidator(x => x.AddMortalityValidators());
    }
}

public static class FluentValidationExtensions
{
    private static readonly Dictionary<
        Type,
        List<(MethodInfo, ConstructorInfo)>
    > _mortalityValidatorFactories = new();

    static FluentValidationExtensions() { }

    public static void AddMortalityValidators<T>(
        this PolymorphicValidator<T, MortalityViewModel> builder
    )
    {
        builder.Add(new MortalityViewModelValidator());

        if (_mortalityValidatorFactories.ContainsKey(typeof(T)) == false)
        {
            List<(MethodInfo, ConstructorInfo)> values = new();

            var mortalityViewModelType = typeof(MortalityViewModel);
            var relevantAssembly = mortalityViewModelType.Assembly;
            var allTypes = relevantAssembly.GetTypes();

            List<(Type, Type)> mortalityViewModelTypes = new();
            foreach (var item in allTypes)
            {
                if (!item.IsSubclassOf(mortalityViewModelType))
                {
                    continue;
                }

                var validatorType = typeof(AbstractValidator<>).MakeGenericType(item);

                mortalityViewModelTypes.Add((item, validatorType));
            }

            foreach (var item in allTypes)
            {
                foreach (var (vmType, validatorType) in mortalityViewModelTypes)
                {
                    if (!item.IsSubclassOf(validatorType))
                    {
                        continue;
                    }

                    var defaultConstructor = item.GetConstructor(Array.Empty<Type>());

                    var type = typeof(PolymorphicValidator<T, MortalityViewModel>);
                    var addMethod = type.GetMethods()
                        .Where(
                            x => x.Name == nameof(PolymorphicValidator<T, MortalityViewModel>.Add)
                        )
                        .Select(x => new { Method = x, parameters = x.GetParameters() })
                        .FirstOrDefault(
                            x =>
                                x.parameters.Length == 2
                                && x.parameters[0].ParameterType.IsAssignableTo(typeof(IValidator))
                        )
                        ?.Method;

                    var genericAddMethod = addMethod?.MakeGenericMethod(vmType);

                    if (genericAddMethod != null)
                    {
                        values.Add((genericAddMethod!, defaultConstructor!));
                    }
                }
            }

            _mortalityValidatorFactories.Add(typeof(T), values);
        }

        foreach (var (addMethod, validatorFactory) in _mortalityValidatorFactories[typeof(T)])
        {
            addMethod?.Invoke(
                builder,
                new[] { validatorFactory.Invoke(null), Array.Empty<string>() }
            );
        }
    }
}
