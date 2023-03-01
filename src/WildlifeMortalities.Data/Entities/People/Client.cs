﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WildlifeMortalities.Data.Entities.Authorizations;
using WildlifeMortalities.Data.Entities.Reports.MultipleMortalities;

namespace WildlifeMortalities.Data.Entities.People;

public class Client : Person
{
    public string EnvClientId { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    // Client is the guided hunter
    public List<SpecialGuideLicence> SpecialGuideLicences { get; set; } = null!;
    public DateTime BirthDate { get; set; }
    public List<Authorization> Authorizations { get; set; } = null!;

    //public List<OutfitterGuidedHuntReport> OutfitterGuidedHuntReportsAsAssistantGuide { get; set; } =
    //    null!;
    public List<OutfitterGuidedHuntReport> OutfitterGuidedHuntReportsAsChiefGuide { get; set; } =
        null!;
    public List<OutfitterGuidedHuntReport> OutfitterGuidedHuntReportsAsClient { get; set; } = null!;
    public List<SpecialGuidedHuntReport> SpecialGuidedHuntReportsAsGuide { get; set; } = null!;
    public List<SpecialGuidedHuntReport> SpecialGuidedHuntReportsAsClient { get; set; } = null!;
    public List<TrappedMortalitiesReport> TrappedMortalitiesReports { get; set; } = null!;

    public override string ToString() => $"{FirstName} {LastName} ({EnvClientId})";
}

public class ClientConfig : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.HasIndex(c => c.EnvClientId).IsUnique();
        builder.Property(c => c.FirstName).HasColumnName(nameof(Client.FirstName));
        builder.Property(c => c.LastName).HasColumnName(nameof(Client.LastName));
    }
}
