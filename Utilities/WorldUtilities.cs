using System.Reflection;
using Terraria;

namespace InfectedQualities.Utilities
{
    public static class WorldUtilities
    {
        public static bool ZoneSurfaceOrUnderground(this Player player, bool underground)
        {
            bool surfaceLayer = player.ZoneOverworldHeight;
            bool remixSurfaceLayer = player.ZoneRockLayerHeight;
            if(underground)
            {
                Utils.Swap(ref remixSurfaceLayer, ref surfaceLayer);
            }

            if (Main.remixWorld)
            {
                return remixSurfaceLayer;
            }
            return surfaceLayer;
        }

        public static bool MusicUnderground()
        {
            if (Main.LocalPlayer.position.Y >= Main.worldSurface * 16.0 + Main.screenHeight / 2 && (Main.remixWorld || !WorldGen.oceanDepths((int)(Main.screenPosition.X + Main.screenWidth / 2) / 16, (int)(Main.screenPosition.Y + Main.screenHeight / 2) / 16)))
            {
                if (Main.remixWorld)
                {
                    return Main.LocalPlayer.position.Y >= Main.rockLayer * 16.0 + Main.screenHeight / 2;
                }
                return true;
            }
            return false;
        }

        public static bool OtherworldMusic()
        {
            FieldInfo swapMusic = typeof(Main).GetField("swapMusic", BindingFlags.NonPublic | BindingFlags.Static);
            return Main.drunkWorld ^ (bool)swapMusic.GetValue(null);
        }
    }
}
