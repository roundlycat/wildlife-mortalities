﻿using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using WildlifeMortalities.Data.Entities.Mortalities;
using WildlifeMortalities.Data.Entities.BiologicalSubmissions.Shared;

namespace WildlifeMortalities.Data.Entities.BiologicalSubmissions;

public class WolverineBioSubmission : BioSubmission<WolverineMortality>, IHasFurbearerSeal
{
    public WolverineBioSubmission() { }

    public WolverineBioSubmission(WolverineMortality mortality)
        : base(mortality) { }

    [IsRequiredOrganicMaterialForBioSubmission("Pelt")]
    public bool? IsPeltProvided { get; set; }
    public string? FurbearerSealNumber { get; set; }

    public override bool CanBeAnalysed => true;

    public override bool HasSubmittedAllRequiredOrganicMaterial() => IsPeltProvided == true;
}

public class WolverineBioSubmissionConfig : IEntityTypeConfiguration<WolverineBioSubmission>
{
    public void Configure(EntityTypeBuilder<WolverineBioSubmission> builder)
    {
        builder
            .ToTable("BioSubmissions")
            .HasOne(b => b.Mortality)
            .WithOne(m => m.BioSubmission)
            .OnDelete(DeleteBehavior.NoAction);

        builder
            .HasIndex(x => x.MortalityId)
            .HasFilter($"[{nameof(WolverineBioSubmission)}_MortalityId] IS NOT NULL");
    }
}