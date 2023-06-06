﻿using Microsoft.EntityFrameworkCore;
using WildlifeMortalities.Data;
using WildlifeMortalities.Data.Entities;
using WildlifeMortalities.Data.Entities.Mortalities;
using WildlifeMortalities.Data.Entities.Reports;
using WildlifeMortalities.Data.Entities.Reports.MultipleMortalities;
using WildlifeMortalities.Data.Entities.Reports.SingleMortality;
using WildlifeMortalities.Data.Entities.Rules.BagLimit;
using WildlifeMortalities.Data.Entities.Seasons;
using WildlifeMortalities.Data.Rules;
using static WildlifeMortalities.Data.Entities.Violation;

namespace WildlifeMortalities.Test.Rules;

public class BagLimitTester
{
    [Theory]
    [InlineData(typeof(HumanWildlifeConflictMortalityReport))]
    [InlineData(typeof(TrappedMortalitiesReport))]
    [InlineData(typeof(CollaredMortalityReport))]
    [InlineData(typeof(ResearchMortalityReport))]
    public async Task Process_WithNotApplicableReport_ReturnsNotApplicableResult(Type type)
    {
        var report = type.GetConstructor(Array.Empty<Type>())!.Invoke(null) as Report;
        var rule = new BagLimitRule();
        using var context = new AppDbContext();

        var result = await rule.Process(report, context);
        result.IsApplicable.Should().BeFalse();
        result.Violations.Should().BeEmpty();
    }

    [Fact]
    public async Task Process_WithOneCaribou_ReturnsLegal()
    {
        var report = new IndividualHuntedMortalityReport
        {
            HuntedActivity = new HuntedActivity()
            {
                Mortality = new CaribouMortality()
                {
                    DateOfDeath = new DateTimeOffset(2023, 4, 1, 0, 0, 0, TimeSpan.FromHours(-7)),
                    Herd = CaribouMortality.CaribouHerd.Atlin
                }
            }
        };

        var rule = new BagLimitRule();
        using var context = new AppDbContext();

        var result = await rule.Process(report, context);
        result.IsApplicable.Should().BeTrue();
        result.Violations.Should().BeEmpty();
    }

    [Fact]
    public async Task Process_WithUnconfiguredEntry_ReturnsViolation()
    {
        var report = new IndividualHuntedMortalityReport
        {
            HuntedActivity = new HuntedActivity()
            {
                Mortality = new CaribouMortality()
                {
                    DateOfDeath = new DateTimeOffset(2023, 4, 1, 0, 0, 0, TimeSpan.FromHours(-7)),
                    Herd = CaribouMortality.CaribouHerd.Atlin
                },
                GameManagementArea = new GameManagementArea
                {
                    Zone = "4",
                    Subzone = "03",
                    Id = 10,
                }
            },
            Season = new HuntingSeason(2023)
        };

        var rule = new BagLimitRule();
        var builder = new DbContextOptionsBuilder<AppDbContext>();
        const string TestName = nameof(Process_WithUnconfiguredEntry_ReturnsViolation);
        builder.UseInMemoryDatabase(TestName);

        var context = new AppDbContext(builder.Options);

        var result = await rule.Process(report, context);
        result.Violations.Should().ContainSingle();
        var violation = result.Violations.First();

        violation.Description
            .Should()
            .Be(
                "Bag limit has not been configured for Caribou in 4-03 for 23/24 season. Please report to service desk."
            );
        violation.Activity.Should().Be(report.HuntedActivity);
        violation.Severity.Should().Be(ViolationSeverity.InternalError);
        violation.Rule.Should().Be(RuleType.BagLimit);
    }

    [Fact]
    public void Matches_WithMatching_ReturnsTrue()
    {
        var season = new HuntingSeason(2023);
        var area = new GameManagementArea
        {
            Zone = "4",
            Subzone = "03",
            Id = 10,
        };
        var entry = new BagLimitEntry
        {
            Area = area,
            Season = season,
            Sex = Data.Enums.Sex.Male,
            Species = Data.Enums.Species.Moose
        };

        var activity = new HuntedActivity
        {
            Mortality = new MooseMortality { Sex = Data.Enums.Sex.Male },
            GameManagementArea = area,
        };

        entry.Matches(activity, season).Should().BeTrue();
    }

    [Theory]
    [InlineData(Data.Enums.Sex.Male)]
    [InlineData(Data.Enums.Sex.Female)]
    [InlineData(Data.Enums.Sex.Unknown)]
    public void Matches_WithMatching_NoSexSpecified_ReturnsTrue(Data.Enums.Sex sex)
    {
        var season = new HuntingSeason(2023);
        var area = new GameManagementArea
        {
            Zone = "4",
            Subzone = "03",
            Id = 10,
        };
        var entry = new BagLimitEntry
        {
            Area = area,
            Season = season,
            Sex = null,
            Species = Data.Enums.Species.Moose
        };

        var activity = new HuntedActivity
        {
            Mortality = new MooseMortality { Sex = sex },
            GameManagementArea = area,
        };

        entry.Matches(activity, season).Should().BeTrue();
    }

    [Fact]
    public void Matches_WithDifferentSex_ReturnsFalse()
    {
        var season = new HuntingSeason(2023);
        var area = new GameManagementArea
        {
            Zone = "4",
            Subzone = "03",
            Id = 10,
        };
        var entry = new BagLimitEntry
        {
            Area = area,
            Season = season,
            Sex = Data.Enums.Sex.Male,
            Species = Data.Enums.Species.Moose
        };

        var activity = new HuntedActivity
        {
            Mortality = new MooseMortality { Sex = Data.Enums.Sex.Female },
            GameManagementArea = area,
        };

        entry.Matches(activity, season).Should().BeFalse();
    }

    [Fact]
    public void Matches_WithDifferentSpecies_ReturnsFalse()
    {
        var season = new HuntingSeason(2023);
        var area = new GameManagementArea
        {
            Zone = "4",
            Subzone = "03",
            Id = 10,
        };
        var entry = new BagLimitEntry
        {
            Area = area,
            Season = season,
            Sex = Data.Enums.Sex.Male,
            Species = Data.Enums.Species.Moose
        };

        var activity = new HuntedActivity
        {
            Mortality = new CaribouMortality { Sex = Data.Enums.Sex.Male },
            GameManagementArea = area,
        };

        entry.Matches(activity, season).Should().BeFalse();
    }

    [Fact]
    public void Matches_WithDifferentSeason_ReturnsFalse()
    {
        var season = new HuntingSeason(2023) { Id = 1 };
        var season2 = new HuntingSeason(2024) { Id = 2 };
        var area = new GameManagementArea
        {
            Zone = "4",
            Subzone = "03",
            Id = 10,
        };
        var entry = new BagLimitEntry
        {
            Area = area,
            Season = season,
            Sex = Data.Enums.Sex.Male,
            Species = Data.Enums.Species.Moose
        };

        var activity = new HuntedActivity
        {
            Mortality = new MooseMortality { Sex = Data.Enums.Sex.Male },
            GameManagementArea = area,
        };

        entry.Matches(activity, season2).Should().BeFalse();
    }

    [Fact]
    public void Matches_WithDifferentArea_ReturnsFalse()
    {
        var season = new HuntingSeason(2023);
        var area = new GameManagementArea
        {
            Zone = "4",
            Subzone = "03",
            Id = 10,
        };
        var entry = new BagLimitEntry
        {
            Area = area,
            Season = season,
            Sex = Data.Enums.Sex.Male,
            Species = Data.Enums.Species.Moose
        };

        var activity = new HuntedActivity
        {
            Mortality = new MooseMortality { Sex = Data.Enums.Sex.Male },
            GameManagementArea = new GameManagementArea
            {
                Zone = "4",
                Subzone = "02",
                Id = 9
            },
        };

        entry.Matches(activity, season).Should().BeFalse();
    }
}
