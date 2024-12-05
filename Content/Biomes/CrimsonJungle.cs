using Terraria;
using Terraria.Graphics.Capture;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;
using Terraria.ID;
using InfectedQualities.Core;
using InfectedQualities.Content.Extras;

namespace InfectedQualities.Content.Biomes
{
    public class CrimsonJungle : ModBiome
    {
        public override int Music => -1;

        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;

        public override CaptureBiome.TileColorStyle TileColorStyle => CaptureBiome.TileColorStyle.Crimson;

        public override string BestiaryIcon => "InfectedQualities/Content/Extras/BestiaryIcons/CrimsonJungle";

        public override string BackgroundPath => "Terraria/Images/MapBG24";

        public override Color? BackgroundColor => Color.Red;

        public override int BiomeTorchItemType => ItemID.IchorTorch;

        public override int BiomeCampfireItemType => ItemID.IchorCampfire;

        public override bool IsBiomeActive(Player player) => Main.hardMode && player.ZoneCrimson && player.ZoneJungle && player.ZoneCavern() && !player.ZoneDungeon && !player.ZoneLihzhardTemple && !player.ZoneGlowshroom;

        public override bool IsLoadingEnabled(Mod mod) => ModContent.GetInstance<InfectedQualitiesServerConfig>().InfectedBiomes;
    }
}
