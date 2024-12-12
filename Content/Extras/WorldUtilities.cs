using System.Reflection;
using Terraria;

namespace InfectedQualities.Content.Extras
{
    public static class WorldUtilities
    {
        public static bool ZoneSurface(this Player player)
        {
            if (Main.remixWorld)
            {
                return player.ZoneRockLayerHeight;
            }
            return player.ZoneOverworldHeight;
        }

        public static bool ZoneCavern(this Player player)
        {
            if (Main.remixWorld)
            {
                return player.ZoneOverworldHeight;
            }
            return player.ZoneRockLayerHeight;
        }

        public static bool MusicUnderground(this Player player)
        {
            if (player.position.Y >= Main.worldSurface * 16.0 + (Main.screenHeight / 2) && (Main.remixWorld || !WorldGen.oceanDepths((int)(Main.screenPosition.X + (Main.screenWidth / 2)) / 16, (int)(Main.screenPosition.Y + (Main.screenHeight / 2)) / 16)))
            {
                if (Main.remixWorld)
                {
                    return player.position.Y >= Main.rockLayer * 16.0 + (Main.screenHeight / 2);
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
