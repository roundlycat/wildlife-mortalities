﻿using FluentValidation;
using WildlifeMortalities.Data.Entities.Mortalities;

namespace WildlifeMortalities.App.Features.MortalityReports;

public class ThinhornSheepMortalityViewModel : MortalityViewModel
{
    public ThinhornSheepBodyColour? BodyColour { get; set; }
    public ThinhornSheepTailColour? TailColour { get; set; }

    public ThinhornSheepMortalityViewModel() : base(Data.Enums.AllSpecies.ThinhornSheep) { }

    public override Mortality GetMortality()
    {
        var mortality = new ThinhornSheepMortality
        {
            BodyColour = BodyColour!.Value,
            TailColour = TailColour!.Value
        };

        SetBaseValues(mortality);
        return mortality;
    }

    public override Dictionary<string, string> GetProperties()
    {
        var result = base.GetProperties();
        result.Add("Body colour", BodyColour!.Value.ToString());
        result.Add("Tail colour", TailColour!.Value.ToString());

        return result;
    }
}

public class ThinhornSheepViewModelValidator : AbstractValidator<ThinhornSheepMortalityViewModel>
{
    public ThinhornSheepViewModelValidator()
    {
        RuleFor(x => x.BodyColour).NotNull();
        RuleFor(x => x.TailColour).NotNull();
    }
}