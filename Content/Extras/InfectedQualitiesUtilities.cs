using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using InfectedQualities.Content.Tiles;
using Terraria.GameContent;
using InfectedQualities.Content.Tiles.Plants;
using InfectedQualities.Core;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using System;
using InfectedQualities.Common;

namespace InfectedQualities.Content.Extras
{
    public static class InfectedQualitiesUtilities
    {
        internal static Asset<Texture2D> PylonCrystalHighlightTexture { get; set; } = null;

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
            if (RefectionMethod(i, j, "nearbyChlorophyte"))
            {
                if (WorldGen.AllowedToSpreadInfections && Main.remixWorld)
                {
                    Main.tile[i, j].TileType = curedTile;
                    WorldGen.SquareTileFrame(i, j);
                    NetMessage.SendTileSquare(-1, i, j);
                }
            }
            else
            {
                SpreadInfection(i, j, infectionType);
            }
        }

        public static void SpreadInfection(int i, int j, InfectionType infectionType)
        {
            if (Main.hardMode && WorldGen.AllowedToSpreadInfections)
            {
                if (NPC.downedPlantBoss && WorldGen.genRand.NextBool(2))
                {
                    return;
                }

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
                    [TileID.CorruptJungleGrass, TileID.CrimsonJungleGrass,  ModContent.TileType<HallowedJungleGrass>()],
                    [TileID.Ebonsand, TileID.Crimsand, TileID.Pearlsand],
                    [TileID.CorruptSandstone, TileID.CrimsonSandstone, TileID.HallowSandstone],
                    [TileID.CorruptHardenedSand, TileID.CrimsonHardenedSand, TileID.HallowHardenedSand],
                    [GetSnowType(InfectionType.Corrupt), GetSnowType(InfectionType.Crimson), GetSnowType(InfectionType.Hallowed)],
                    [TileID.CorruptIce, TileID.FleshIce, TileID.HallowedIce],
                    [TileID.CorruptThorns, TileID.CrimsonThorns, ModContent.TileType<HallowedThorns>()]
                ];

                while (true)
                {
                    x = i + WorldGen.genRand.Next(-3, 4);
                    y = j + WorldGen.genRand.Next(-3, 4);

                    if (WorldGen.CountNearBlocksTypes(x, y, 2, 1, [TileID.Sunflower]) == 0)
                    {
                        for (int tile = 0; tile < tilesToConvert.Length; tile++)
                        {
                            if (Main.tile[x, y].TileType == tilesToConvert[tile])
                            {
                                Main.tile[x, y].TileType = (ushort)convertedTiles[tile][(short)infectionType];
                                WorldGen.SquareTileFrame(x, y);
                                NetMessage.SendTileSquare(-1, x, y);
                            }
                            else if (ConvertMoss(x, y, infectionType, true))
                            {
                                NetMessage.SendTileSquare(-1, x, y);
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
                            NetMessage.SendTileSquare(-1, x, y);
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
                    NetMessage.SendTileSquare(-1, i, j - 1, 5);
                }
            }
        }

        public static string GetPlanteraType()
        {
            if (ModContent.GetInstance<InfectedQualitiesClientConfig>().InfectedPlantera)
            {
                if (TextureAssets.Npc[NPCID.Plantera] == InfectedQualitiesGlobalNPC.CorruptPlantera)
                {
                    return "Corrupt";
                }
                else if (TextureAssets.Npc[NPCID.Plantera] == InfectedQualitiesGlobalNPC.CrimsonPlantera)
                {
                    return "Crimson";
                }
                else if (TextureAssets.Npc[NPCID.Plantera] == InfectedQualitiesGlobalNPC.HallowedPlantera)
                {
                    return "Hallowed";
                }
            }
            return null;
        }

        public static void FixSpreadCompability(int i, int j, InfectionType infectionType)
        {
            if (!RefectionMethod(i, j, "nearbyChlorophyte") && Main.hardMode && WorldGen.AllowedToSpreadInfections)
            {
                if (NPC.downedPlantBoss && WorldGen.genRand.NextBool(2))
                {
                    return;
                }

                int x;
                int y;
                short id = (short)infectionType;
                while (true)
                {
                    x = i + WorldGen.genRand.Next(-3, 4);
                    y = j + WorldGen.genRand.Next(-3, 4);

                    if (WorldGen.CountNearBlocksTypes(x, y, 2, 1, [TileID.Sunflower]) == 0)
                    {
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

                        for (int tile = 0; tile < tilesToConvert.Length; tile++)
                        {
                            if (Main.tile[x, y].TileType == tilesToConvert[tile] && convertedTiles[tile][id] != -1)
                            {
                                Main.tile[x, y].TileType = (ushort)convertedTiles[tile][id];
                                WorldGen.SquareTileFrame(x, y);
                                NetMessage.SendTileSquare(-1, x, y);
                                break;
                            }
                            else if (ConvertMoss(x, y, infectionType, true))
                            {
                                NetMessage.SendTileSquare(-1, x, y);
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
                    Main.tile[x, y].WallType = convertedWalls[id];
                    WorldGen.SquareWallFrame(x, y);
                    NetMessage.SendTileSquare(-1, x, y);
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
            if (RefectionMethod(i, j, "nearbyChlorophyte"))
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
                SpreadInfection(i, j, infectionType);
            }
        }

        public static bool ConvertMoss(int i, int j, InfectionType? infectionType, bool safe = false)
        {
            if (ModContent.GetInstance<InfectedQualitiesServerConfig>().InfectedMosses && TileID.Sets.Conversion.Moss[Main.tile[i, j].TileType])
            {
                foreach (MossType mossType in Enum.GetValues(typeof(MossType)))
                {
                    if (infectionType.HasValue && Main.tile[i, j].TileType == GetMossType(null, mossType))
                    {
                        Main.tile[i, j].TileType = GetMossType(infectionType, mossType);
                        WorldGen.SquareTileFrame(i, j);
                        return true;
                    }
                    else if(!safe)
                    {
                        foreach (InfectionType cycledInfection in Enum.GetValues(typeof(InfectionType)))
                        {
                            if(!infectionType.Equals(cycledInfection) && Main.tile[i, j].TileType == GetMossType(cycledInfection, mossType))
                            {
                                Main.tile[i, j].TileType = GetMossType(infectionType, mossType);
                                WorldGen.SquareTileFrame(i, j);
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        public static ushort GetMossType(InfectionType? infectionType, MossType mossType)
        {
            if (!infectionType.HasValue)
            {
                return mossType switch
                {
                    MossType.Green => TileID.GreenMoss,
                    MossType.Brown => TileID.BrownMoss,
                    MossType.Red => TileID.RedMoss,
                    MossType.Blue => TileID.BlueMoss,
                    MossType.Purple => TileID.PurpleMoss,
                    MossType.Lava => TileID.LavaMoss,
                    MossType.Krypton => TileID.KryptonMoss,
                    MossType.Xenon => TileID.XenonMoss,
                    MossType.Argon => TileID.ArgonMoss,
                    MossType.Neon => TileID.VioletMoss,
                    MossType.Helium => TileID.RainbowMoss,
                    _ => TileID.Stone
                };
            }
            return ModContent.GetInstance<InfectedQualities>().Find<ModTile>(infectionType.Value.ToString() + mossType.ToString() + "Moss").Type;
        }

        public static ushort GetSnowType(InfectionType infectionType) => ModContent.GetInstance<InfectedQualities>().Find<ModTile>(infectionType.ToString() + "Snow").Type;

        /// <summary>
        /// This is temporary, I have to use refection until the methods get public for the stable release.
        /// </summary>
        public static bool RefectionMethod(int x, int y, string name)
        {
            return (bool)typeof(WorldGen).GetMethod(name, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static).Invoke(null, [x, y]);
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
}
