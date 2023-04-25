﻿using FluentValidation;
using WildlifeMortalities.Data.Entities.Reports.SingleMortality;

namespace WildlifeMortalities.App.Features.Reports;

public class IndividualHuntedMortalityReportViewModel : MortalityReportViewModel
{
    private readonly int _reportId = Constants.EfCore.TemporaryAutoGeneratedKey;

    public IndividualHuntedMortalityReportViewModel() { }

    public IndividualHuntedMortalityReportViewModel(IndividualHuntedMortalityReport report)
        : base(report)
    {
        HuntedActivityViewModel = new HuntedActivityViewModel(report.HuntedActivity, report);
        _reportId = report.Id;
    }

    public HuntedActivityViewModel HuntedActivityViewModel { get; set; } = new();

    public override IndividualHuntedMortalityReport GetReport(int personId)
    {
        var report = new IndividualHuntedMortalityReport()
        {
            Id = _reportId,
            ClientId = personId,
            HuntedActivity = HuntedActivityViewModel.GetActivity(),
        };

        SetReportBaseValues(report);
        return report;
    }
}

public class IndividualHuntedMortalityReportViewModelValidator
    : MortalityReportViewModelValidator<IndividualHuntedMortalityReportViewModel>
{
    public IndividualHuntedMortalityReportViewModelValidator()
    {
        RuleFor(x => x.HuntedActivityViewModel)
            .SetValidator(new HuntedActivityViewModelValidator());
        // Todo: attach validation message
        RuleFor(x => x.DateSubmitted)
            .Must(
                (model, dateSubmitted) =>
                    (dateSubmitted ?? DateTimeOffset.Now)
                    >= model
                        .HuntedActivityViewModel
                        .MortalityWithSpeciesSelectionViewModel
                        .MortalityViewModel
                        .DateOfDeath
            )
            .WithMessage("Date submitted cannot occur before date of death.");
    }
}
