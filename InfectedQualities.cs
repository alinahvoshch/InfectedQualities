using InfectedQualities.Content.Biomes;
using InfectedQualities.Content.Extras;
using InfectedQualities.Content.Extras.Tiles;
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
            if (ModContent.GetInstance<InfectedQualitiesServerConfig>().PylonOfNight)
            {
                TextureUtilities.PylonCrystalHighlightTexture = ModContent.Request<Texture2D>("InfectedQualities/Content/Extras/Tiles/Pylon_CrystalHighlight");
            }

            foreach (InfectionType infectionType in Enum.GetValues(typeof(InfectionType)))
            {
                AddContent(new InfectedSnow(infectionType));

                foreach(GemType gemType in Enum.GetValues(typeof(GemType)))
                {
                    AddContent(new InfectedGemstone(infectionType, gemType));
                }

                foreach (MossType mossType in Enum.GetValues(typeof(MossType)))
                {
                    AddContent(new InfectedMoss(infectionType, mossType));
                }

                if (ModContent.GetInstance<InfectedQualitiesClientConfig>().InfectedPlantera)
                {
                    for (int i = 1; i < 3; i++)
                    {
                        AddBossHeadTexture("InfectedQualities/Content/Extras/MapIcons/" + infectionType.ToString() + "Plantera_MapIcon_" + i);
                    }
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
                ["ZoneCorruptJungle", Player player] => ModContent.GetInstance<InfectedQualitiesServerConfig>().InfectedBiomes && player.InModBiome<CorruptJungle>(),
                ["ZoneCrimsonJungle", Player player] => ModContent.GetInstance<InfectedQualitiesServerConfig>().InfectedBiomes && player.InModBiome<CrimsonJungle>(),
                ["ZoneHallowedJungle", Player player] => ModContent.GetInstance<InfectedQualitiesServerConfig>().InfectedBiomes && player.InModBiome<HallowedJungle>(),
                ["GetWallBiomeSightColor", int type] => InfectedQualitiesModSupport.ModWallBiomeSight[type],
                ["SetWallBiomeSightColor", int type, Color color] => InfectedQualitiesModSupport.ModWallBiomeSight[type] = color,
                _ => throw new Exception("You buffoon, you failed to use InfectedQualities.Call")
            };
        }
    }
}