﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WildlifeMortalities.Data.Entities.MortalityReports;
using WildlifeMortalities.Data.Enums;

namespace WildlifeMortalities.Data.Entities.Mortalities;

public abstract class Mortality
{
    public int Id { get; set; }
    public int MortalityReportId { get; set; }
    public MortalityReport MortalityReport { get; set; } = null!;
    public DateTime? DateOfDeath { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public Sex Sex { get; set; }
    public string Discriminator { get; set; } = null!;
}

public class MortalityConfig : IEntityTypeConfiguration<Mortality>
{
    public void Configure(EntityTypeBuilder<Mortality> builder)
    {
        builder.ToTable("Mortalities");
        builder.Property(m => m.Sex).HasConversion<string>();
        builder.Property(m => m.Latitude).HasPrecision(10, 8);
        builder.Property(m => m.Longitude).HasPrecision(11, 8);
    }
}
