using Terraria;
using Terraria.Graphics.Capture;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;
using Terraria.ID;
using InfectedQualities.Core;
using InfectedQualities.Content.Extras;

namespace InfectedQualities.Content.Biomes
{
    public class CorruptJungle : ModBiome
    {
        public override int Music => -1;

        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;

        public override CaptureBiome.TileColorStyle TileColorStyle => CaptureBiome.TileColorStyle.Corrupt;

        public override string BestiaryIcon => "InfectedQualities/Content/Extras/BestiaryIcons/Corrupt_Jungle";

        public override string BackgroundPath => "Terraria/Images/MapBG23";

        public override Color? BackgroundColor => Color.Purple;

        public override int BiomeTorchItemType => ItemID.CursedTorch;

        public override int BiomeCampfireItemType => ItemID.CursedCampfire;

        public override bool IsBiomeActive(Player player) => Main.hardMode && player.ZoneCorrupt && player.ZoneJungle && player.ZoneCavern() && !player.ZoneDungeon && !player.ZoneLihzhardTemple && !player.ZoneGlowshroom;

        public override bool IsLoadingEnabled(Mod mod) => ModContent.GetInstance<InfectedQualitiesServerConfig>().InfectedBiomes;
    }
}
