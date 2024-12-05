using Terraria;
using Terraria.Graphics.Capture;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using InfectedQualities.Core;

namespace InfectedQualities.Content.Biomes
{
    public class HallowedJungle : ModBiome
    {
        public override int Music => -1;

        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeMedium;

        public override CaptureBiome.TileColorStyle TileColorStyle => CaptureBiome.TileColorStyle.Jungle;

        public override string BestiaryIcon => "InfectedQualities/Content/Extras/BestiaryIcons/Hallowed_Jungle";

        public override string BackgroundPath => "Terraria/Images/MapBG22";

        public override Color? BackgroundColor => Color.Blue;

        public override bool IsBiomeActive(Player player) => Main.hardMode && player.ZoneHallow && player.ZoneJungle && !player.ZoneDungeon && !player.ZoneLihzhardTemple && !player.ZoneGlowshroom && !player.ZoneSkyHeight && !player.ZoneUnderworldHeight;

        public override bool IsLoadingEnabled(Mod mod) => ModContent.GetInstance<InfectedQualitiesServerConfig>().InfectedBiomes;
    }
}
