using InfectedQualities.Common;
using InfectedQualities.Content.Extras.Tiles;
using InfectedQualities.Core;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace InfectedQualities.Content.Extras
{
    public static class TextureUtilities
    {
        internal static Asset<Texture2D> PylonCrystalHighlightTexture { get; set; } = null;

        public static void ReplacePlanteraType(InfectionType infectionType)
        {
            TextureAssets.Npc[NPCID.Plantera] = infectionType switch
            {
                InfectionType.Corrupt => InfectedQualitiesGlobalNPC.CorruptPlantera,
                InfectionType.Crimson => InfectedQualitiesGlobalNPC.CrimsonPlantera,
                InfectionType.Hallowed => InfectedQualitiesGlobalNPC.HallowedPlantera,
                _ => TextureAssets.Npc[NPCID.Plantera]
            };

            string planteraType = infectionType.ToString();
            TextureAssets.Npc[NPCID.PlanterasHook] = ModContent.Request<Texture2D>("InfectedQualities/Content/NPCs/" + planteraType + "Plantera_Hook");
            TextureAssets.Chain26 = ModContent.Request<Texture2D>("InfectedQualities/Content/Extras/" + planteraType + "Plantera_Hook_Vine");
            TextureAssets.Npc[NPCID.PlanterasTentacle] = ModContent.Request<Texture2D>("InfectedQualities/Content/NPCs/" + planteraType + "Plantera_Tentacle");
            TextureAssets.Chain27 = ModContent.Request<Texture2D>("InfectedQualities/Content/Extras/" + planteraType + "Plantera_Tentacle_Vine");
            TextureAssets.Npc[NPCID.Spore] = ModContent.Request<Texture2D>("InfectedQualities/Content/NPCs/" + planteraType + "Plantera_Spore");
            TextureAssets.Projectile[ProjectileID.SeedPlantera] = ModContent.Request<Texture2D>("InfectedQualities/Content/Projectiles/" + planteraType + "Plantera_Seed");
        }

        public static InfectionType? GetPlanteraType()
        {
            if (ModContent.GetInstance<InfectedQualitiesClientConfig>().InfectedPlantera)
            {
                if (TextureAssets.Npc[NPCID.Plantera] == InfectedQualitiesGlobalNPC.CorruptPlantera)
                {
                    return InfectionType.Corrupt;
                }
                else if (TextureAssets.Npc[NPCID.Plantera] == InfectedQualitiesGlobalNPC.CrimsonPlantera)
                {
                    return InfectionType.Crimson;
                }
                else if (TextureAssets.Npc[NPCID.Plantera] == InfectedQualitiesGlobalNPC.HallowedPlantera)
                {
                    return InfectionType.Hallowed;
                }
            }
            return null;
        }
    }
}
