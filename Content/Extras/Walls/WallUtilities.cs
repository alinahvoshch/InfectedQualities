using InfectedQualities.Content.Extras.Tiles;
using InfectedQualities.Core;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;

namespace InfectedQualities.Content.Extras.Walls
{
    public static class WallUtilities
    {
        public static bool GetBiomeSightColor(int i, int j, int type, out Color sightColor)
        {
            sightColor = InfectedQualitiesModSupport.ModWallBiomeSight[type];
            if (ModContent.GetInstance<InfectedQualitiesClientConfig>().BiomeSightWallHighlighting && Main.LocalPlayer.biomeSight && !WorldGen.SolidTile(i, j, true) && (Main.tile[i, j].LiquidAmount == 0 || Main.tile[i, j].LiquidType == LiquidID.Water))
            {
                if (sightColor == default)
                {
                    if (WallID.Sets.Corrupt[type])
                    {
                        sightColor = new(200, 100, 240);
                        return true;
                    }
                    else if (WallID.Sets.Crimson[type])
                    {
                        sightColor = new(255, 100, 100);
                        return true;
                    }
                    else if (WallID.Sets.Hallow[type])
                    {
                        sightColor = new(255, 160, 240);
                        return true;
                    }
                    return false;
                }
                return true;
            }
            return false;
        }

        public static void WallSpread(int i, int j, InfectionType infectionType)
        {
            if (InfectedQualitiesUtilities.RefectionMethod(i, j, "nearbyChlorophyte"))
            {
                if (WorldGen.AllowedToSpreadInfections && Main.remixWorld)
                {
                    if (Main.tile[i, j].WallType is WallID.CorruptGrassUnsafe or WallID.CrimsonGrassUnsafe or WallID.HallowedGrassUnsafe)
                    {
                        Main.tile[i, j].WallType = WallID.JungleUnsafe;
                        WorldGen.SquareWallFrame(i, j);
                        NetMessage.SendTileSquare(-1, i, j);
                    }
                    else if (Main.tile[i, j].WallType is WallID.EbonstoneUnsafe or WallID.CrimstoneUnsafe or WallID.PearlstoneBrickUnsafe)
                    {
                        Main.tile[i, j].WallType = WallID.Stone;
                        WorldGen.SquareWallFrame(i, j);
                        NetMessage.SendTileSquare(-1, i, j);
                    }
                    else if (Main.tile[i, j].WallType is WallID.CorruptHardenedSand or WallID.CrimsonHardenedSand or WallID.HallowHardenedSand)
                    {
                        Main.tile[i, j].WallType = WallID.HardenedSand;
                        WorldGen.SquareWallFrame(i, j);
                        NetMessage.SendTileSquare(-1, i, j);
                    }
                    else if (Main.tile[i, j].WallType is WallID.CorruptSandstone or WallID.CrimsonSandstone or WallID.HallowSandstone)
                    {
                        Main.tile[i, j].WallType = WallID.Sandstone;
                        WorldGen.SquareWallFrame(i, j);
                        NetMessage.SendTileSquare(-1, i, j);
                    }
                }
            }
            else
            {
                TileUtilities.SpreadInfection(i, j, infectionType);
            }
        }
    }
}
