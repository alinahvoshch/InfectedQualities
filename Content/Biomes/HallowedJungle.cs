using Terraria;
using Terraria.Graphics.Capture;
using Terraria.ModLoader;
using InfectedQualities.Core;
using Terraria.ID;
using InfectedQualities.Utilities;

namespace InfectedQualities.Content.Biomes
{
    public class HallowedJungle : ModBiome
    {
        public override int Music
        {
            get
            {
                if(ModContent.GetInstance<InfectedQualitiesClientConfig>().HallowedJungleMusic && !Main.LocalPlayer.ZoneGraveyard && !Main.LocalPlayer.ZoneMeteor)
                {
                    if(WorldUtilities.MusicUnderground())
                    {
                        if(Main.remixWorld)
                        {
                            return WorldUtilities.OtherworldMusic() ? MusicID.OtherworldlyHallow : MusicID.TheHallow;
                        }
                        return WorldUtilities.OtherworldMusic() ? MusicID.OtherworldlyUGHallow : MusicID.UndergroundHallow;
                    }
                    else if(Main.dayTime && !Main.IsItRaining)
                    {
                        if (WorldUtilities.OtherworldMusic())
                        {
                            return MusicID.OtherworldlyHallow;
                        }
                        if (Main.IsItAHappyWindyDay && !Main.remixWorld)
                        {
                            return MusicID.WindyDay;
                        }
                        return MusicID.TheHallow;
                    }
                }
                return -1;
            }
        }

        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeMedium;

        public override CaptureBiome.TileColorStyle TileColorStyle => CaptureBiome.TileColorStyle.Jungle;

        public override string BackgroundPath => "Terraria/Images/MapBG22";

        public override bool IsBiomeActive(Player player)
        {
            if (Main.hardMode && player.ZoneHallow && player.ZoneJungle && !player.ZoneGlowshroom)
            {
                return !player.ZoneSkyHeight && !player.ZoneUnderworldHeight && !player.ZoneDungeon && !player.ZoneLihzhardTemple;
            }
            return false;
        }

        public override bool IsLoadingEnabled(Mod mod) => ModContent.GetInstance<InfectedQualitiesServerConfig>().InfectedBiomes;
    }
}
