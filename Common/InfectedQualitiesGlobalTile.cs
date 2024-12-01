﻿using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using InfectedQualities.Content.Tiles;
using Terraria.ObjectData;
using System.Linq;
using InfectedQualities.Content.Tiles.Plants;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using InfectedQualities.Core;

namespace InfectedQualities.Common
{
    public class InfectedQualitiesGlobalTile : GlobalTile
    {
        public override void SetStaticDefaults()
        {
            TileID.Sets.Corrupt[TileID.CorruptPlants] = true;
            TileID.Sets.Corrupt[TileID.CorruptThorns] = true;
            TileID.Sets.AddCorruptionTile(TileID.CorruptVines);
            TileID.Sets.CorruptCountCollection.Remove(TileID.CorruptPlants);
            TileID.Sets.CorruptCountCollection.Remove(TileID.CorruptThorns);
            TileID.Sets.CorruptCountCollection.Remove(TileID.CorruptVines);

            TileID.Sets.Crimson[TileID.CrimsonPlants] = true;
            TileID.Sets.Crimson[TileID.CrimsonThorns] = true;
            TileID.Sets.AddCrimsonTile(TileID.CrimsonPlants);
            TileID.Sets.AddCrimsonTile(TileID.CrimsonVines);
            TileID.Sets.CrimsonCountCollection.Remove(TileID.CrimsonPlants);
            TileID.Sets.CrimsonCountCollection.Remove(TileID.CrimsonThorns);
            TileID.Sets.CrimsonCountCollection.Remove(TileID.CrimsonVines);

            TileID.Sets.Hallow[TileID.HallowedPlants] = true;
            TileID.Sets.Hallow[TileID.HallowedPlants2] = true;
            TileID.Sets.HallowBiome[TileID.HallowedVines] = 1;
            TileID.Sets.HallowCountCollection.Remove(TileID.HallowedPlants);
            TileID.Sets.HallowCountCollection.Remove(TileID.HallowedPlants2);
            TileID.Sets.HallowCountCollection.Remove(TileID.HallowedVines);

            TileID.Sets.CanGrowCrystalShards[TileID.HallowedGrass] = true;
            TileID.Sets.CanGrowCrystalShards[TileID.GolfGrassHallowed] = true;

            if(ModContent.GetInstance<InfectedQualitiesServerConfig>().InfectedBiomes)
            {
                TileID.Sets.SlowlyDiesInWater[TileID.CorruptPlants] = false;
                TileID.Sets.SlowlyDiesInWater[TileID.CrimsonPlants] = false;
                TileID.Sets.SlowlyDiesInWater[TileID.HallowedPlants] = false;

                TileID.Sets.AddJungleTile(TileID.CorruptJungleGrass, 2);
                TileID.Sets.AddJungleTile(TileID.CrimsonJungleGrass, 2);

                TileObjectData sunflower = TileObjectData.GetTileData(TileID.Sunflower, 0);
                sunflower.AnchorValidTiles = [.. sunflower.AnchorValidTiles, ModContent.TileType<HallowedJungleGrass>()];
            }
        }

        public override bool TileFrame(int i, int j, int type, ref bool resetFrame, ref bool noBreak)
        {
            if (ModContent.GetInstance<InfectedQualitiesServerConfig>().InfectedBiomes)
            {
                if (type is TileID.CorruptPlants or TileID.CrimsonPlants or TileID.HallowedPlants or TileID.HallowedPlants2 or TileID.JunglePlants or TileID.JunglePlants2)
                {
                    Tile soil = Main.tile[i, j + 1];
                    if (Main.tile[i, j + 1].TileType == ModContent.TileType<HallowedJungleGrass>() && soil.HasTile && !soil.IsHalfBlock && soil.Slope == SlopeType.Solid)
                    {
                        if (type != TileID.HallowedPlants && type != TileID.HallowedPlants2)
                        {
                            Main.tile[i, j].TileType = TileID.HallowedPlants;
                            NetMessage.SendTileSquare(-1, i, j);
                        }
                        return false;
                    }
                }
                else if (TileID.Sets.IsVine[type])
                {
                    Tile soil = Main.tile[i, j - 1];
                    if (soil.TileType == ModContent.TileType<HallowedJungleGrass>() && soil.HasTile && !soil.BottomSlope)
                    {
                        if (type != TileID.HallowedVines)
                        {
                            Main.tile[i, j].TileType = TileID.HallowedVines;
                            WorldGen.SquareTileFrame(i, j);
                            NetMessage.SendTileSquare(-1, i, j);
                        }
                        return false;
                    }
                }
                else if (type == TileID.PlanteraBulb)
                {
                    InfectedQualitiesUtilities.GetTopLeft(i, j, out int x, out int y, out short num);
                    if (num != -1)
                    {
                        for (int m = x; m < x + 2; m++)
                        {
                            for (int n = y; n < y + 2; n++)
                            {
                                if (Main.tile[m, n].HasTile && Main.tile[m, n].TileType == type)
                                {
                                    Main.tile[m, n].TileType = (ushort)ModContent.TileType<InfectedPlanteraBulb>();
                                    Main.tile[m, n].TileFrameX = (short)(num + Main.tile[m, n].TileFrameX % 36);
                                }
                            }
                        }
                        NetMessage.SendTileSquare(-1, i, j, 2);
                        return false;
                    }
                }
                else if (type == TileID.LifeFruit)
                {
                    InfectedQualitiesUtilities.GetTopLeft(i, j, out int x, out int y, out short num);
                    if (num != -1)
                    {
                        for (int m = x; m < x + 2; m++)
                        {
                            for (int n = y; n < y + 2; n++)
                            {
                                if (Main.tile[m, n].HasTile && Main.tile[m, n].TileType == type)
                                {
                                    Main.tile[m, n].TileType = (ushort)ModContent.TileType<InfectedLifeFruit>();
                                    Main.tile[m, n].TileFrameY = (short)(num + Main.tile[m, n].TileFrameY % 36);
                                }
                            }
                        }
                        NetMessage.SendTileSquare(-1, i, j, 2);
                        return false;
                    }
                }
            }
            return true;
        }

        public override void RandomUpdate(int i, int j, int type)
        {
            if(type is TileID.Chlorophyte or TileID.ChlorophyteBrick)
            {
                if(NPC.downedPlantBoss)
                {
                    int x = i + Main.rand.Next(-6, 7);
                    int y = j + Main.rand.Next(-6, 7);

                    if (Main.tile[x, y].HasTile)
                    {
                        if (Main.tile[x, y].TileType == TileID.Pearlstone)
                        {
                            Main.tile[x, y].TileType = TileID.Stone;
                            WorldGen.SquareTileFrame(x, y);
                            NetMessage.SendTileSquare(-1, x, y);
                        }
                        else if (Main.tile[x, y].TileType == TileID.Pearlsand)
                        {
                            Main.tile[x, y].TileType = TileID.Sand;
                            WorldGen.SquareTileFrame(x, y);
                            NetMessage.SendTileSquare(-1, x, y);
                        }
                        else if (Main.tile[x, y].TileType == TileID.HallowHardenedSand)
                        {
                            Main.tile[x, y].TileType = TileID.HardenedSand;
                            WorldGen.SquareTileFrame(x, y);
                            NetMessage.SendTileSquare(-1, x, y);
                        }
                        else if (Main.tile[x, y].TileType == TileID.HallowSandstone)
                        {
                            Main.tile[x, y].TileType = TileID.Sandstone;
                            WorldGen.SquareTileFrame(x, y);
                            NetMessage.SendTileSquare(-1, x, y);
                        }
                        else if (ModContent.GetInstance<InfectedQualitiesServerConfig>().InfectedBiomes)
                        {
                            if (Main.tile[x, y].TileType == ModContent.TileType<HallowedJungleGrass>())
                            {
                                Main.tile[x, y].TileType = TileID.JungleGrass;
                                WorldGen.SquareTileFrame(x, y);
                                NetMessage.SendTileSquare(-1, x, y);
                            }
                            else if (Main.tile[x, y].TileType == ModContent.TileType<CorruptSnow>() || Main.tile[x, y].TileType == ModContent.TileType<CrimsonSnow>() || Main.tile[x, y].TileType == ModContent.TileType<HallowedSnow>())
                            {
                                Main.tile[x, y].TileType = TileID.SnowBlock;
                                WorldGen.SquareTileFrame(x, y);
                                NetMessage.SendTileSquare(-1, x, y);
                            }
                            else if (Main.tile[x, y].TileType == ModContent.TileType<HallowedThorns>())
                            {
                                WorldGen.KillTile(x, y);
                                NetMessage.SendTileSquare(-1, x, y);
                            }
                        }
                    }

                    if(Main.tile[x, y].WallType is WallID.GrassUnsafe or WallID.CorruptGrassUnsafe or WallID.CrimsonGrassUnsafe or WallID.HallowedGrassUnsafe) 
                    {
                        Main.tile[x, y].WallType = WallID.JungleUnsafe;
                        WorldGen.SquareWallFrame(x, y);
                        NetMessage.SendTileSquare(-1, x, y);
                    }
                    else if (Main.tile[x, y].WallType is WallID.CaveUnsafe or WallID.CorruptionUnsafe1 or WallID.CrimsonUnsafe1 or WallID.HallowUnsafe1)
                    {
                        Main.tile[x, y].WallType = WallID.JungleUnsafe1;
                        WorldGen.SquareWallFrame(x, y);
                        NetMessage.SendTileSquare(-1, x, y);
                    }
                    else if (Main.tile[x, y].WallType is WallID.Cave2Unsafe or WallID.CorruptionUnsafe2 or WallID.CrimsonUnsafe2 or WallID.HallowUnsafe2)
                    {
                        Main.tile[x, y].WallType = WallID.JungleUnsafe2;
                        WorldGen.SquareWallFrame(x, y);
                        NetMessage.SendTileSquare(-1, x, y);
                    }
                    else if (Main.tile[x, y].WallType is WallID.Cave3Unsafe or WallID.CorruptionUnsafe3 or WallID.CrimsonUnsafe3 or WallID.HallowUnsafe3)
                    {
                        Main.tile[x, y].WallType = WallID.JungleUnsafe3;
                        WorldGen.SquareWallFrame(x, y);
                        NetMessage.SendTileSquare(-1, x, y);
                    }
                    else if (Main.tile[x, y].WallType is WallID.Cave4Unsafe or WallID.CorruptionUnsafe4 or WallID.CrimsonUnsafe4 or WallID.HallowUnsafe4)
                    {
                        Main.tile[x, y].WallType = WallID.JungleUnsafe4;
                        WorldGen.SquareWallFrame(x, y);
                        NetMessage.SendTileSquare(-1, x, y);
                    }
                    else if (Main.tile[x, y].WallType is WallID.EbonstoneUnsafe or WallID.CrimstoneUnsafe or WallID.PearlstoneBrickUnsafe)
                    {
                        Main.tile[x, y].WallType = WallID.Stone;
                        WorldGen.SquareWallFrame(x, y);
                        NetMessage.SendTileSquare(-1, x, y);
                    }
                    else if (Main.tile[x, y].WallType is WallID.CorruptHardenedSand or WallID.CrimsonHardenedSand or WallID.HallowHardenedSand)
                    {
                        Main.tile[x, y].WallType = WallID.HardenedSand;
                        WorldGen.SquareWallFrame(x, y);
                        NetMessage.SendTileSquare(-1, x, y);
                    }
                    else if (Main.tile[x, y].WallType is WallID.CorruptSandstone or WallID.CrimsonSandstone or WallID.HallowSandstone)
                    {
                        Main.tile[x, y].WallType = WallID.Sandstone;
                        WorldGen.SquareWallFrame(x, y);
                        NetMessage.SendTileSquare(-1, x, y);
                    }
                }
            }

            if (ModContent.GetInstance<InfectedQualitiesServerConfig>().InfectedBiomes)
            {
                if (type is TileID.CorruptGrass or TileID.Ebonstone or TileID.Ebonsand or TileID.CorruptSandstone or TileID.CorruptHardenedSand or TileID.CorruptIce or TileID.CorruptJungleGrass or TileID.CorruptVines or TileID.CorruptPlants or TileID.CorruptThorns)
                {
                    InfectedQualitiesUtilities.FixSpreadCompability(i, j, InfectionType.Corruption);
                }
                else if (type is TileID.CrimsonGrass or TileID.Crimstone or TileID.Crimsand or TileID.CrimsonSandstone or TileID.CrimsonHardenedSand or TileID.FleshIce or TileID.CrimsonJungleGrass or TileID.CrimsonVines or TileID.CrimsonPlants or TileID.CrimsonThorns)
                {
                    InfectedQualitiesUtilities.FixSpreadCompability(i, j, InfectionType.Crimson);
                }
                else if (type is TileID.HallowedGrass or TileID.Pearlstone or TileID.Pearlsand or TileID.HallowSandstone or TileID.HallowHardenedSand or TileID.HallowedIce or TileID.GolfGrassHallowed or TileID.HallowedVines or TileID.HallowedPlants or TileID.HallowedPlants2)
                {
                    InfectedQualitiesUtilities.FixSpreadCompability(i, j, InfectionType.Hallow);
                }

                if (type is TileID.CorruptJungleGrass or TileID.CrimsonJungleGrass)
                {
                    if (j < Main.worldSurface && !Main.remixWorld && WorldGen.genRand.NextBool(500) && (!Main.tile[i, j - 1].HasTile || TileID.Sets.IgnoredByGrowingSaplings[Main.tile[i, j - 1].TileType]) && WorldGen.GrowTree(i, j) && WorldGen.PlayerLOS(i, j))
                    {
                        WorldGen.TreeGrowFXCheck(i, j - 1);
                    }
                    else if (WorldGen.genRand.NextBool(10) && !Main.tile[i, j - 1].HasTile && Main.tile[i, j - 1].LiquidAmount > 0 && Main.tile[i, j - 1].LiquidType == LiquidID.Water)
                    {
                        if (WorldGen.PlaceTile(i, j - 1, TileID.CorruptPlants) || WorldGen.PlaceTile(i, j - 1, TileID.CrimsonPlants))
                        {
                            Main.tile[i, j - 1].CopyPaintAndCoating(Main.tile[i, j]);
                        }
                    }

                    if (NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3 && WorldGen.genRand.NextBool(60))
                    {
                        InfectedQualitiesUtilities.AttemptToPlaceInfectedPlant(i, j, ModContent.TileType<InfectedPlanteraBulb>(), TileID.PlanteraBulb, 150);
                    }

                    if (NPC.downedMechBossAny && WorldGen.genRand.NextBool(Main.expertMode ? 30 : 40))
                    {
                        InfectedQualitiesUtilities.AttemptToPlaceInfectedPlant(i, j, ModContent.TileType<InfectedLifeFruit>(), TileID.LifeFruit, Main.expertMode ? 50 : 60);
                    }
                }
                else if (type == TileID.HallowedVines && WorldGen.genRand.NextBool(20) && !Main.tile[i, j + 1].HasTile && Main.tile[i, j + 1].LiquidType != LiquidID.Lava && InfectedQualitiesUtilities.RefectionMethod(i, j, "GrowMoreVines"))
                {
                    bool flag = false;
                    for (int num = j; num > j - 10 && num < 0; num--)
                    {
                        if (Main.tile[i, num].HasTile && Main.tile[i, num].TileType == ModContent.TileType<HallowedJungleGrass>() && !Main.tile[i, num].BottomSlope)
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (flag)
                    {
                        Main.tile[i, j + 1].Get<TileWallWireStateData>().HasTile = true;
                        Main.tile[i, j + 1].TileType = TileID.HallowedVines;
                        Main.tile[i, j + 1].CopyPaintAndCoating(Main.tile[i, j]);
                        WorldGen.SquareTileFrame(i, j + 1);
                        NetMessage.SendTileSquare(-1, i, j + 1);
                    }
                }
            }
        }

        public override void PostDraw(int i, int j, int type, SpriteBatch spriteBatch)
        {
            if(type == TileID.CorruptJungleGrass && Main.rand.NextBool(500))
            {
                Dust.NewDust(new Vector2(i * 16, j * 16), 16, 16, DustID.Demonite);
            }
        }

        public override void Unload()
        {
            if(ModContent.GetInstance<InfectedQualitiesServerConfig>().InfectedBiomes)
            {
                TileObjectData sunflower = TileObjectData.GetTileData(TileID.Sunflower, 0);
                sunflower.AnchorValidTiles = sunflower.AnchorValidTiles.Except([ModContent.TileType<HallowedJungleGrass>()]).ToArray();
            }
        }
    }
}