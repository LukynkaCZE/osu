// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Bindables;
using osu.Framework.Graphics.Sprites;
using osu.Game.Configuration;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.UI;
using osu.Game.Scoring;
using osuTK;

namespace osu.Game.Rulesets.Osu.Mods
{
    internal class OsuModShrink : Mod, IApplicableToScoreProcessor, IUpdatableByPlayfield
    {
        public override string Name => "Shrink";
        public override string Acronym => "SHRNK";
        public override IconUsage? Icon => FontAwesome.Solid.Compress;
        public override ModType Type => ModType.Fun;
        public override string Description => "The Circles! They are shrinking!";
        public override double ScoreMultiplier => 1;
        public override Type[] IncompatibleMods => new[] { typeof(OsuModEasy), typeof(OsuModHardRock), typeof(OsuModDifficultyAdjust) };
        public ScoreRank AdjustRank(ScoreRank rank, double accuracy) => rank;

        [SettingSource("Starting Size", "How big the starting hitobject should be", 0)]
        public BindableFloat StartingSize { get; } = new BindableFloat(1.15f)
        {
            Precision = 0.01f,
            MinValue = 0.1f,
            MaxValue = 2f,
        };
        [SettingSource("Shrink (less = bigger shrink)", "How much should size decrease per combo", 0)]
        public BindableFloat SizeDecreasePerCombo { get; } = new BindableFloat(110f)
        {
            Precision = 10f,
            MinValue = 50f,
            MaxValue = 300.0f,
        };

        public float ComboCS = -500;

        public void ApplyToScoreProcessor(ScoreProcessor scoreProcessor)
        {
            scoreProcessor.Combo.BindValueChanged(combo =>
            {
                ComboCS = StartingSize.Value - (combo.NewValue / SizeDecreasePerCombo.Value);
            });
        }

        public void Update(Playfield playfield)
        {
            if (ComboCS == -500)
            {
                ComboCS = StartingSize.Value;
            }

            ShrinkHitObjects(playfield);
        }

        public void ShrinkHitObjects(Playfield playfield)
        {
            foreach (var osuObject in playfield.AllHitObjects)
            {
                osuObject.Scale = new Vector2(ComboCS);
            }
        }
    }
}
