﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WildlifeMortalities.Data.Entities.Mortalities;

namespace WildlifeMortalities.Data.Entities.BiologicalSubmissions;

public abstract class BioSubmission
{
    public int Id { get; set; }
    public BioSubmissionStatus Status { get; set; }
    public DateTimeOffset? DateSubmitted { get; set; }
    public DateTimeOffset? DateModified { get; set; }
    public string Comment { get; set; } = string.Empty;
    public Age? Age { get; set; }

    public virtual bool CanBeAnalysed { get; }
    public abstract void ClearDependencies();
    public abstract bool HasSubmittedAllRequiredOrganicMaterial();
}

public abstract class BioSubmission<T> : BioSubmission
    where T : Mortality
{
    protected BioSubmission() { }

    protected BioSubmission(T mortality)
    {
        Status = BioSubmissionStatus.NotSubmitted;
        Mortality = mortality;
    }

    public int MortalityId { get; set; }

    public T Mortality { get; set; }

    public override void ClearDependencies() => Mortality = null!;
}

public class BioSubmissionConfig : IEntityTypeConfiguration<BioSubmission>
{
    public void Configure(EntityTypeBuilder<BioSubmission> builder)
    {
        builder.OwnsOne(
            b => b.Age,
            a =>
            {
                a.Property(a => a.Confidence).IsRequired();
                a.WithOwner();
            }
        );
        builder.Ignore(b => b.CanBeAnalysed);
    }
}

public class Age
{
    public int Years { get; set; }
    public ConfidenceInAge? Confidence { get; set; }
}

public enum ConfidenceInAge
{
    [Display(Name = "Fair")]
    Fair = 10,

    [Display(Name = "Good")]
    Good = 20,

    [Display(Name = "Poor")]
    Poor = 30
}

public enum BioSubmissionStatus
{
    [Display(Name = "Not submitted")]
    NotSubmitted = 10,

    [Display(Name = "Partially submitted")]
    PartiallySubmitted = 20,

    [Display(Name = "Submitted")]
    Submitted = 30,

    [Display(Name = "Analysis complete")]
    AnalysisComplete = 40,
}
