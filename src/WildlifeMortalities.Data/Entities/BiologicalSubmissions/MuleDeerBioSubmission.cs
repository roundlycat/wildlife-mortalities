﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WildlifeMortalities.Data.Entities.BiologicalSubmissions.Shared;
using WildlifeMortalities.Data.Entities.Mortalities;

namespace WildlifeMortalities.Data.Entities.BiologicalSubmissions;

public class MuleDeerBioSubmission : BioSubmission<MuleDeerMortality>
{
    public MuleDeerBioSubmission() { }

    public MuleDeerBioSubmission(MuleDeerMortality mortality)
        : base(mortality) { }

    [IsRequiredOrganicMaterialForBioSubmission("Hide")]
    public bool? IsHideProvided { get; set; }

    [IsRequiredOrganicMaterialForBioSubmission("Head")]
    public bool? IsHeadProvided { get; set; }

    [IsRequiredOrganicMaterialForBioSubmission("Antlers")]
    public bool? IsAntlersProvided { get; set; }

    public override bool HasSubmittedAllRequiredOrganicMaterial() =>
        IsHideProvided == true && IsHeadProvided == true && IsAntlersProvided == true;
}

public class MuleDeerBioSubmissionConfig : IEntityTypeConfiguration<MuleDeerBioSubmission>
{
    public void Configure(EntityTypeBuilder<MuleDeerBioSubmission> builder)
    {
        builder
            .ToTable("BioSubmissions")
            .HasOne(b => b.Mortality)
            .WithOne(m => m.BioSubmission)
            .OnDelete(DeleteBehavior.NoAction);
        builder
            .HasIndex(x => x.MortalityId)
            .HasFilter($"[{nameof(MuleDeerBioSubmission)}_MortalityId] IS NOT NULL");
    }
}
