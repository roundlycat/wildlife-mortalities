﻿using WildlifeMortalities.Data.Entities.Reports.MultipleMortalities;

namespace WildlifeMortalities.App.Features.Reports;

public class TrappedReportViewModel
{
    public List<TrappedActivityViewModel> TrappedActivityViewModels { get; set; } = new();

    public TrappedMortalitiesReport GetReport(int personId)
    {
        var report = new TrappedMortalitiesReport
        {
            ClientId = personId,
            TrappedActivities = TrappedActivityViewModels.Select(x => x.GetActivity()).ToList()
        };

        return report;
    }
}
