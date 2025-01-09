using InfectedQualities.Content.Tiles.Plants;
using InfectedQualities.Content.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using InfectedQualities.Core;

namespace InfectedQualities.Utilities
{
    public static class TileUtilities
    {
        public static void TileMerge(ushort tileFrom, ushort tileTo)
        {
            Main.tileMerge[tileFrom][tileTo] = true;
            Main.tileMerge[tileTo][tileFrom] = true;
        }

        public static bool TileExposedToLava(int i, int j)
        {
            bool flag = false;
            for (int x = i - 1; x < i + 2; x++)
            {
                for (int y = j - 1; y < j + 2; y++)
                {
                    if (Main.tile[x, y] != null && Main.tile[x, y].LiquidAmount > 0 && Main.tile[x, y].LiquidType == LiquidID.Lava)
                    {
                        flag = true;
                        break;
                    }
                }
            }
            return flag;
        }

        public static void DefaultInfectionSpread(int i, int j, InfectionType infectionType, ushort curedTile)
        {
            if (WorldGen.nearbyChlorophyte(i, j))
            {
                if (WorldGen.AllowedToSpreadInfections && Main.remixWorld)
                {
                    Main.tile[i, j].TileType = curedTile;
                    WorldGen.SquareTileFrame(i, j);
                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendTileSquare(-1, i, j);
                    }
                }
            }
            else
            {
                SpreadInfection(i, j, infectionType);
            }
        }

        public static void SpreadInfection(int i, int j, InfectionType infectionType)
        {
            if (Main.hardMode && WorldGen.AllowedToSpreadInfections && !InfectedQualitiesModSupport.PureglowRange(i))
            {
                if (NPC.downedPlantBoss && WorldGen.genRand.NextBool(2)) return;

                int x;
                int y;
                ushort[] tilesToConvert = [
                    TileID.Grass,
                    TileID.GolfGrass,
                    TileID.Stone,
                    TileID.JungleGrass,
                    TileID.Sand,
                    TileID.Sandstone,
                    TileID.HardenedSand,
                    TileID.SnowBlock,
                    TileID.IceBlock,
                    TileID.JungleThorns
                ];

                int[][] convertedTiles = [
                    [TileID.CorruptGrass, TileID.CrimsonGrass, TileID.HallowedGrass],
                    [TileID.CorruptGrass, TileID.CrimsonGrass, TileID.GolfGrassHallowed],
                    [TileID.Ebonstone, TileID.Crimstone, TileID.Pearlstone],
                    [TileID.CorruptJungleGrass, TileID.CrimsonJungleGrass, -1],
                    [TileID.Ebonsand, TileID.Crimsand, TileID.Pearlsand],
                    [TileID.CorruptSandstone, TileID.CrimsonSandstone, TileID.HallowSandstone],
                    [TileID.CorruptHardenedSand, TileID.CrimsonHardenedSand, TileID.HallowHardenedSand],
                    [-1, -1, -1],
                    [TileID.CorruptIce, TileID.FleshIce, TileID.HallowedIce],
                    [TileID.CorruptThorns, TileID.CrimsonThorns, -1]
                ];

                if (ModContent.GetInstance<InfectedQualitiesServerConfig>().InfectedBiomes)
                {
                    convertedTiles[3][2] = ModContent.TileType<HallowedJungleGrass>();
                    convertedTiles[7][(short)infectionType] = GetSnowType(infectionType);
                    convertedTiles[9][2] = ModContent.TileType<HallowedThorns>();
                }

                while (true)
                {
                    x = i + WorldGen.genRand.Next(-3, 4);
                    y = j + WorldGen.genRand.Next(-3, 4);

                    if (WorldGen.CountNearBlocksTypes(x, y, 2, 1, [TileID.Sunflower]) == 0)
                    {
                        if (ModContent.GetInstance<InfectedQualitiesServerConfig>().InfectedMosses && Main.tileMoss[Main.tile[i, j].TileType])
                        {
                            ConvertEnum<MossType>(x, y, infectionType, true);
                            if (Main.netMode == NetmodeID.Server)
                            {
                                NetMessage.SendTileSquare(-1, x, y);
                            }
                        }
                        else if (ModContent.GetInstance<InfectedQualitiesServerConfig>().InfectedGemstones && Main.tileStone[Main.tile[i, j].TileType])
                        {
                            ConvertEnum<GemType>(x, y, infectionType, true);
                            if (Main.netMode == NetmodeID.Server)
                            {
                                NetMessage.SendTileSquare(-1, x, y);
                            }
                        }
                        else
                        {
                            for (int tile = 0; tile < tilesToConvert.Length; tile++)
                            {
                                if (Main.tile[x, y].TileType == tilesToConvert[tile] && convertedTiles[tile][(short)infectionType] != -1)
                                {
                                    Main.tile[x, y].TileType = (ushort)convertedTiles[tile][(short)infectionType];
                                    WorldGen.SquareTileFrame(x, y);
                                    if (Main.netMode == NetmodeID.Server)
                                    {
                                        NetMessage.SendTileSquare(-1, x, y);
                                    }
                                    break;
                                }
                            }
                        }
                    }
                    if (WorldGen.genRand.NextBool(2)) break;
                }

                x = i + WorldGen.genRand.Next(-2, 3);
                y = j + WorldGen.genRand.Next(-2, 3);

                ushort[][] wallsToConvert = [
                    [WallID.GrassUnsafe, WallID.FlowerUnsafe, WallID.JungleUnsafe],
                    [WallID.Stone]
                ];

                ushort[][] convertedWalls = [
                    [WallID.CorruptGrassUnsafe, WallID.CrimsonGrassUnsafe, WallID.HallowedGrassUnsafe],
                    [WallID.EbonstoneUnsafe, WallID.CrimstoneUnsafe, WallID.PearlstoneBrickUnsafe]
                ];

                for (int listIndex = 0; listIndex < wallsToConvert.Length; listIndex++)
                {
                    for (int wallIndex = 0; wallIndex < wallsToConvert[listIndex].Length; wallIndex++)
                    {
                        if (Main.tile[x, y].WallType == wallsToConvert[listIndex][wallIndex])
                        {
                            Main.tile[x, y].WallType = convertedWalls[listIndex][(short)infectionType];
                            WorldGen.SquareWallFrame(x, y);
                            if (Main.netMode == NetmodeID.Server)
                            {
                                NetMessage.SendTileSquare(-1, x, y);
                            }
                            break;
                        }
                    }
                }
            }
        }

        public static void AttemptToPlaceInfectedPlant(int i, int j, int type, ushort vanillaVariant, int range)
        {
            if (j > Main.rockLayer && WorldGen.genRand.NextBool(25) && Main.hardMode && Main.tile[i, j].LiquidAmount == 0)
            {
                bool flag = true;
                for (int m = i - range; m < i + range; m += 2)
                {
                    for (int n = j - range; n < j + range; n += 2)
                    {
                        if (!WorldGen.InWorld(m, n) || Main.tile[m, n].HasTile && (Main.tile[m, n].TileType == type || Main.tile[m, n].TileType == vanillaVariant))
                        {
                            flag = false;
                            break;
                        }
                    }
                }

                if (flag && WorldGen.PlaceObject(i, j - 1, type))
                {
                    WorldGen.SquareTileFrame(i, j - 1);
                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendTileSquare(-1, i, j - 1, 5);
                    }
                }
            }
        }

        public static void FixSpreadCompability(int i, int j, InfectionType infectionType)
        {
            if (!WorldGen.nearbyChlorophyte(i, j) && Main.hardMode && WorldGen.AllowedToSpreadInfections && !InfectedQualitiesModSupport.PureglowRange(i))
            {
                if (NPC.downedPlantBoss && WorldGen.genRand.NextBool(2)) return;

                int x;
                int y;
                ushort[] tilesToConvert = [
                    TileID.JungleGrass,
                    TileID.SnowBlock,
                    TileID.JungleThorns
                ];

                int[][] convertedTiles = [
                    [-1, -1,  ModContent.TileType<HallowedJungleGrass>()],
                    [GetSnowType(InfectionType.Corrupt), GetSnowType(InfectionType.Crimson), GetSnowType(InfectionType.Hallowed)],
                    [-1, -1, ModContent.TileType<HallowedThorns>()]
                ];

                while (true)
                {
                    x = i + WorldGen.genRand.Next(-3, 4);
                    y = j + WorldGen.genRand.Next(-3, 4);

                    if (WorldGen.CountNearBlocksTypes(x, y, 2, 1, [TileID.Sunflower]) == 0)
                    {
                        if (ModContent.GetInstance<InfectedQualitiesServerConfig>().InfectedMosses && Main.tileMoss[Main.tile[i, j].TileType])
                        {
                            ConvertEnum<MossType>(x, y, infectionType, true);
                            if (Main.netMode == NetmodeID.Server)
                            {
                                NetMessage.SendTileSquare(-1, x, y);
                            }
                        }
                        else if (ModContent.GetInstance<InfectedQualitiesServerConfig>().InfectedGemstones && Main.tileStone[Main.tile[i, j].TileType])
                        {
                            ConvertEnum<GemType>(x, y, infectionType, true);
                            if (Main.netMode == NetmodeID.Server)
                            {
                                NetMessage.SendTileSquare(-1, x, y);
                            }
                        }
                        else if (ModContent.GetInstance<InfectedQualitiesServerConfig>().InfectedBiomes)
                        {
                            for (int tile = 0; tile < tilesToConvert.Length; tile++)
                            {
                                if (Main.tile[x, y].TileType == tilesToConvert[tile] && convertedTiles[tile][(short)infectionType] != -1)
                                {
                                    Main.tile[x, y].TileType = (ushort)convertedTiles[tile][(short)infectionType];
                                    WorldGen.SquareTileFrame(x, y);
                                    if (Main.netMode == NetmodeID.Server)
                                    {
                                        NetMessage.SendTileSquare(-1, x, y);
                                    }
                                    break;
                                }
                            }
                        }
                    }
                    if (WorldGen.genRand.NextBool(2)) break;
                }

                x = i + WorldGen.genRand.Next(-2, 3);
                y = j + WorldGen.genRand.Next(-2, 3);

                ushort[] convertedWalls = [
                    WallID.EbonstoneUnsafe,
                    WallID.CrimstoneUnsafe,
                    WallID.PearlstoneBrickUnsafe
                ];

                if (Main.tile[x, y].WallType == WallID.Stone)
                {
                    Main.tile[x, y].WallType = convertedWalls[(short)infectionType];
                    WorldGen.SquareWallFrame(x, y);
                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendTileSquare(-1, x, y);
                    }
                }
            }
        }

        public static void GetTopLeft(int i, int j, out int x, out int y, out short num)
        {
            num = -1;
            x = Main.tile[i, j].TileFrameX / 18;
            while (x > 1)
            {
                x -= 2;
            }
            x = i - x;

            y = Main.tile[i, j].TileFrameY / 18;
            while (y > 1)
            {
                y -= 2;
            }
            y = j - y;

            for (int k = x; k < x + 2; k++)
            {
                if (Main.tile[k, y + 2].TileType == TileID.CorruptJungleGrass)
                {
                    num = 0;
                }
                else if (Main.tile[k, y + 2].TileType == TileID.CrimsonJungleGrass)
                {
                    num = 36;
                }
                else if (Main.tile[k, y + 2].TileType == ModContent.TileType<HallowedJungleGrass>())
                {
                    num = 72;
                }
            }
        }

        public static void ConvertEnum<T>(int i, int j, InfectionType? infectionType, bool safe = false) where T : Enum
        {
            foreach (T enumType in Enum.GetValues(typeof(T)))
            {
                if (infectionType.HasValue && Main.tile[i, j].TileType == GetEnumType(null, enumType))
                {
                    Main.tile[i, j].TileType = GetEnumType(infectionType, enumType);
                    WorldGen.SquareTileFrame(i, j);
                    break;
                }
                else if (!safe)
                {
                    foreach (InfectionType cycledInfection in Enum.GetValues(typeof(InfectionType)))
                    {
                        if (!infectionType.Equals(cycledInfection) && Main.tile[i, j].TileType == GetEnumType(cycledInfection, enumType))
                        {
                            Main.tile[i, j].TileType = GetEnumType(infectionType, enumType);
                            WorldGen.SquareTileFrame(i, j);
                            break;
                        }
                    }
                }
            }
        }

        public static ushort GetEnumType(InfectionType? infectionType, Enum enumType, string prefix = null)
        {
            if (prefix == null)
            {
                if (enumType.GetType() == typeof(MossType))
                {
                    prefix = "Moss";
                }
                else if (enumType.GetType() == typeof(GemType))
                {
                    prefix = "Gemstone";
                }
            }

            if (!infectionType.HasValue)
            {
                if (prefix == "Moss")
                {
                    if ((MossType)enumType == MossType.Helium)
                    {
                        return TileID.RainbowMoss;
                    }
                    else if ((MossType)enumType == MossType.Neon)
                    {
                        return TileID.VioletMoss;
                    }
                    return (ushort)TileID.Search.GetId(enumType.ToString() + prefix);
                }
                else if (prefix == "Gemstone")
                {
                    if ((GemType)enumType == GemType.Amber)
                    {
                        return TileID.AmberStoneBlock;
                    }
                    return (ushort)TileID.Search.GetId(enumType.ToString());
                }
                else if (prefix == "MossBrick")
                {
                    if ((MossType)enumType == MossType.Helium)
                    {
                        return TileID.RainbowMossBrick;
                    }
                    else if ((MossType)enumType == MossType.Neon)
                    {
                        return TileID.VioletMossBrick;
                    }
                    return (ushort)TileID.Search.GetId(enumType.ToString() + prefix);
                }
            }
            return ModContent.GetInstance<InfectedQualities>().Find<ModTile>(infectionType.Value.ToString() + enumType.ToString() + prefix).Type;
        }

        public static ushort GetSnowType(InfectionType infectionType) => ModContent.GetInstance<InfectedQualities>().Find<ModTile>(infectionType.ToString() + "Snow").Type;

        public static void WallSpread(int i, int j, InfectionType infectionType)
        {
            if (WorldGen.nearbyChlorophyte(i, j))
            {
                if (WorldGen.AllowedToSpreadInfections && Main.remixWorld)
                {
                    if (Main.tile[i, j].WallType is WallID.CorruptGrassUnsafe or WallID.CrimsonGrassUnsafe or WallID.HallowedGrassUnsafe)
                    {
                        Main.tile[i, j].WallType = WallID.JungleUnsafe;
                        WorldGen.SquareWallFrame(i, j);
                        if (Main.netMode == NetmodeID.Server)
                        {
                            NetMessage.SendTileSquare(-1, i, j);
                        }
                    }
                    else if (Main.tile[i, j].WallType is WallID.EbonstoneUnsafe or WallID.CrimstoneUnsafe or WallID.PearlstoneBrickUnsafe)
                    {
                        Main.tile[i, j].WallType = WallID.Stone;
                        WorldGen.SquareWallFrame(i, j);
                        if (Main.netMode == NetmodeID.Server)
                        {
                            NetMessage.SendTileSquare(-1, i, j);
                        }
                    }
                    else if (Main.tile[i, j].WallType is WallID.CorruptHardenedSand or WallID.CrimsonHardenedSand or WallID.HallowHardenedSand)
                    {
                        Main.tile[i, j].WallType = WallID.HardenedSand;
                        WorldGen.SquareWallFrame(i, j);
                        if (Main.netMode == NetmodeID.Server)
                        {
                            NetMessage.SendTileSquare(-1, i, j);
                        }
                    }
                    else if (Main.tile[i, j].WallType is WallID.CorruptSandstone or WallID.CrimsonSandstone or WallID.HallowSandstone)
                    {
                        Main.tile[i, j].WallType = WallID.Sandstone;
                        WorldGen.SquareWallFrame(i, j);
                        if (Main.netMode == NetmodeID.Server)
                        {
                            NetMessage.SendTileSquare(-1, i, j);
                        }
                    }
                }
            }
            else
            {
                SpreadInfection(i, j, infectionType);
            }
        }
    }

    public enum InfectionType
    {
        Corrupt,
        Crimson,
        Hallowed
    }

    public enum MossType
    {
        Green,
        Brown,
        Red,
        Blue,
        Purple,
        Lava,
        Krypton,
        Xenon,
        Argon,
        Neon,
        Helium
    }

    public enum GemType
    {
        Sapphire,
        Ruby,
        Emerald,
        Topaz,
        Amethyst,
        Diamond,
        Amber
    }
}
