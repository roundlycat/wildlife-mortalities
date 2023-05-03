﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WildlifeMortalities.Data.Entities.Reports;

namespace WildlifeMortalities.Data.Entities.Authorizations;

public class OutfitterAssistantGuideLicence : Authorization, IHasOutfitterAreas
{
    public List<OutfitterArea> OutfitterAreas { get; set; } = null!;

    public override AuthorizationResult GetResult(Report report) =>
        throw new NotImplementedException();

    public override string GetAuthorizationType() => "Outfitter assistant guide licence";

    protected override void UpdateInternal(Authorization authorization)
    {
        if (authorization is not OutfitterAssistantGuideLicence outfitterAssistantGuideLicence)
        {
            throw new ArgumentException(
                $"Expected {nameof(OutfitterAssistantGuideLicence)} but received {authorization.GetType().Name}"
            );
        }

        OutfitterAreas = outfitterAssistantGuideLicence.OutfitterAreas;
    }
}

public class OutfitterAssistantGuideLicenceConfig
    : IEntityTypeConfiguration<OutfitterAssistantGuideLicence>
{
    public void Configure(EntityTypeBuilder<OutfitterAssistantGuideLicence> builder) =>
        builder.ToTable("Authorizations");
}
