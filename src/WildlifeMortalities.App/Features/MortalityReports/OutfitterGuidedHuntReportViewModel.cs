﻿using FluentValidation;
using MudBlazor;
using WildlifeMortalities.Data.Entities;
using WildlifeMortalities.Data.Entities.People;
using WildlifeMortalities.Data.Entities.Reports.MultipleMortalities;
using WildlifeMortalities.Data.Enums;

namespace WildlifeMortalities.App.Features.MortalityReports;

public class OutfitterGuidedHuntReportViewModel
{
    public DateRange? HuntingDateRange { get; set; }
    public Client? SelectedGuide { get; set; }
    public List<Client> Guides { get; set; } = new();
    public OutfitterArea? OutfitterArea { get; set; }
    public GuidedHuntResult? Result { get; set; }

    public List<HuntedActivityViewModel> HuntedMortalityReportViewModels { get; set; } = new();

    public OutfitterGuidedHuntReport GetReport(int personId)
    {
        // Clear mortality reports if the hunter wasn't successful
        if (Result is not GuidedHuntResult.WentHuntingAndKilledWildlife)
        {
            HuntedMortalityReportViewModels.Clear();
        }

        var report = new OutfitterGuidedHuntReport
        {
            HuntStartDate = (DateTime)HuntingDateRange!.Start!,
            HuntEndDate = (DateTime)HuntingDateRange.End!,
            Guides = Guides,
            OutfitterArea = OutfitterArea!,
            Result = Result!.Value,
            ClientId = personId,
            HuntedActivities = HuntedMortalityReportViewModels.Select(x => x.GetActivity()).ToList()
        };

        return report;
    }
}

public class OutfitterGuidedHuntReportViewModelValidator
    : AbstractValidator<OutfitterGuidedHuntReportViewModel>
{
    public OutfitterGuidedHuntReportViewModelValidator()
    {
        RuleFor(x => x.HuntingDateRange).NotNull();
        RuleFor(x => x.Guides).NotEmpty();
        RuleFor(x => x.OutfitterArea).NotNull();
        RuleFor(x => x.Result).IsInEnum().NotNull();
    }
}
