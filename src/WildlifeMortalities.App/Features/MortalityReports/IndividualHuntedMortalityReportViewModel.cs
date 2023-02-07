﻿using FluentValidation;
using WildlifeMortalities.Data.Entities.Reports.SingleMortality;

namespace WildlifeMortalities.App.Features.MortalityReports;

public class IndividualHuntedMortalityReportViewModel
{
    public HuntedActivityViewModel HuntedActivityViewModel { get; set; }

    public IndividualHuntedMortalityReport GetReport(int personId)
    {
        return new IndividualHuntedMortalityReport()
        {
            HuntedActivity = HuntedActivityViewModel.GetActivity(),
            ClientId = personId
            // Todo add date logic
        };
    }
}

public class IndividualHuntedMortalityReportViewModelValidator
    : AbstractValidator<IndividualHuntedMortalityReportViewModel>
{
    public IndividualHuntedMortalityReportViewModelValidator() { }
}
