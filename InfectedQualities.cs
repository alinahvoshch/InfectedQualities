using InfectedQualities.Content.Biomes;
using InfectedQualities.Content.Extras;
using InfectedQualities.Content.Tiles;
using InfectedQualities.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace InfectedQualities
{
    public class InfectedQualities : Mod
	{
        public override void Load()
        {
            foreach(InfectionType infectionType in Enum.GetValues(typeof(InfectionType)))
            {
                foreach (MossType mossType in Enum.GetValues(typeof(MossType)))
                {
                    AddContent(new InfectedMoss(infectionType, mossType));
                }
            }

            InfectedQualitiesUtilities.PylonCrystalHighlightTexture = ModContent.Request<Texture2D>("InfectedQualities/Content/Extras/Tiles/Pylon_CrystalHighlight");

            if (ModContent.GetInstance<InfectedQualitiesClientConfig>().InfectedPlantera)
            {
                for (int i = 1; i < 3; i++)
                {
                    AddBossHeadTexture("InfectedQualities/Content/Extras/MapIcons/CorruptPlantera_MapIcon_" + i);
                    AddBossHeadTexture("InfectedQualities/Content/Extras/MapIcons/CrimsonPlantera_MapIcon_" + i);
                    AddBossHeadTexture("InfectedQualities/Content/Extras/MapIcons/HallowedPlantera_MapIcon_" + i);
                }
            }
        }

        public override void PostSetupContent()
        {
            InfectedQualitiesModSupport.PostSetupContent();
        }

        public override object Call(params object[] args)
        {
            return args switch
            {
                ["ZoneCorruptJungle", Player player] => player.InModBiome<CorruptJungle>(),
                ["ZoneCrimsonJungle", Player player] => player.InModBiome<CrimsonJungle>(),
                ["ZoneHallowedJungle", Player player] => player.InModBiome<HallowedJungle>(),
                ["GetWallBiomeSightColor", int type] => InfectedQualitiesModSupport.ModWallBiomeSight[type],
                ["SetWallBiomeSightColor", int type, Color color] => InfectedQualitiesModSupport.ModWallBiomeSight[type] = color,
                _ => throw new Exception("You buffoon, you failed to use InfectedQualities.Call")
            };
        }
    }
}