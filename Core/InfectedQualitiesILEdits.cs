﻿using InfectedQualities.Content.Tiles;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using Terraria;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using InfectedQualities.Content.Tiles.Plants;
using Terraria.Enums;
using InfectedQualities.Common;

namespace InfectedQualities.Core
{
    public class InfectedQualitiesILEdits : ILoadable
    {
        public void Load(Mod mod)
        {
            On_Gore.NewGore_IEntitySource_Vector2_Vector2_int_float += Gore_NewGore;
            On_Dust.NewDust += Dust_NewDust;

            IL_WorldGen.GERunner += WorldGen_GERunner;
            On_WorldGen.Convert += WorldGen_Convert;

            On_TileDrawing.GetTreeVariant += TileDrawing_GetTreeVariant;
            On_WorldGen.GetTreeLeaf += WorldGen_GetTreeLeaf;
            IL_WorldGen.GetCommonTreeFoliageData += WorldGen_GetCommonTreeFoliageData;

            IL_WorldGen.CheckAlch += WorldGen_CheckAlch;
            IL_WorldGen.PlaceAlch += WorldGen_PlaceAlch;
            IL_WorldGen.PlantAlch += WorldGen_PlantAlch;

            IL_WorldGen.ScoreRoom += WorldGen_ScoreRoom;
            On_WorldGen.GetTileTypeCountByCategory += WorldGen_GetTileTypeCountByCategory;

            IL_WorldGen.UpdateWorld_Inner += WorldGen_UpdateWorld_Inner;
            IL_WorldGen.UpdateWorld_GrassGrowth += WorldGen_UpdateWorld_GrassGrowth;
            IL_WorldGen.hardUpdateWorld += WorldGen_hardUpdateWorld;
            On_WorldGen.nearbyChlorophyte += WorldGen_nearbyChlorophyte;
            On_WorldGen.ChlorophyteDefense += WorldGen_ChlorophyteDefense;

            IL_WorldGen.CheckLilyPad += WorldGen_CheckLilyPad;
            IL_WorldGen.CheckCatTail += WorldGen_CheckCatTail;
            IL_WorldGen.GetDesiredStalagtiteStyle += WorldGen_GetDesiredStalagtiteStyle;
        }

        private static int Gore_NewGore(On_Gore.orig_NewGore_IEntitySource_Vector2_Vector2_int_float orig, IEntitySource source, Vector2 Position, Vector2 Velocity, int Type, float Scale)
        {
            string planteraType = InfectedQualitiesUtilities.GetPlanteraType();
            if (planteraType != null)
            {
                if (Type >= 378 && Type <= 380)
                {
                    Type = ModContent.GetInstance<InfectedQualities>().Find<ModGore>(planteraType + "Plantera_1_" + (Type - 378)).Type;
                }
                else if (Type >= 381 && Type <= 387)
                {
                    Type = ModContent.GetInstance<InfectedQualities>().Find<ModGore>(planteraType + "Plantera_2_" + (Type - 381)).Type;
                }
                else if (Type >= 388 && Type <= 389)
                {
                    Type = ModContent.GetInstance<InfectedQualities>().Find<ModGore>(planteraType + "Plantera_Tentacle_" + (Type - 388)).Type;
                }
                else if (Type >= 390 && Type <= 391)
                {
                    Type = ModContent.GetInstance<InfectedQualities>().Find<ModGore>(planteraType + "Plantera_Hook_" + (Type - 390)).Type;
                }
            }
            return orig(source, Position, Velocity, Type, Scale);
        }

        private static int Dust_NewDust(On_Dust.orig_NewDust orig, Vector2 Position, int Width, int Height, int Type, float SpeedX, float SpeedY, int Alpha, Color newColor, float Scale)
        {
            if (Type == DustID.Plantera_Green)
            {
                switch (InfectedQualitiesUtilities.GetPlanteraType())
                {
                    case "Corrupt":
                        Type = DustID.CorruptPlants;
                        break;
                    case "Crimson":
                        Type = DustID.CrimsonPlants;
                        break;
                    case "Hallowed":
                        Type = DustID.HallowedPlants;
                        break;
                }
            }
            else if (Type == DustID.Plantera_Pink)
            {
                switch (InfectedQualitiesUtilities.GetPlanteraType())
                {
                    case "Corrupt":
                        Type = DustID.Corruption;
                        break;
                    case "Crimson":
                        Type = DustID.Crimson;
                        break;
                    case "Hallowed":
                        newColor.MultiplyRGB(Color.Magenta);
                        break;
                }
            }
            return orig(Position, Width, Height, Type, SpeedX, SpeedY, Alpha, newColor, Scale);
        }

        private static void WorldGen_GERunner(ILContext il)
        {
            ILCursor cursor = new(il);
            if(cursor.TryGotoNext(i => i.MatchLdarg(4)))
            {
                cursor.Emit(OpCodes.Ldloc, 15);
                cursor.Emit(OpCodes.Ldloc, 16);
                cursor.Emit(OpCodes.Ldarg, 4);
                cursor.EmitDelegate<Action<int, int, bool>>((m, n, good) =>
                {
                    if (good)
                    {
                        if (ModContent.GetInstance<InfectedQualitiesServerConfig>().InfectedBiomes)
                        {
                            if(Main.tile[m, n].WallType is WallID.JungleUnsafe or WallID.Jungle)
                            {
                                Main.tile[m, n].WallType = WallID.HallowedGrassUnsafe;
                            }

                            if (Main.tile[m, n].TileType is TileID.JungleGrass or TileID.CorruptJungleGrass or TileID.CrimsonJungleGrass)
                            {
                                Main.tile[m, n].TileType = (ushort)ModContent.TileType<HallowedJungleGrass>();
                                WorldGen.SquareTileFrame(m, n);
                            }
                            else if (Main.tile[m, n].TileType == TileID.SnowBlock)
                            {
                                Main.tile[m, n].TileType = (ushort)ModContent.TileType<HallowedSnow>();
                                WorldGen.SquareTileFrame(m, n);
                            }
                            else if (Main.tile[m, n].TileType == TileID.JungleThorns)
                            {
                                Main.tile[m, n].TileType = (ushort)ModContent.TileType<HallowedThorns>();
                                WorldGen.SquareTileFrame(m, n);
                            }
                        }

                        if (Main.tile[m, n].WallType == WallID.Stone)
                        {
                            Main.tile[m, n].WallType = WallID.PearlstoneBrickUnsafe;
                        }

                        if (Main.tileMoss[Main.tile[m, n].TileType]) 
                        {
                            Main.tile[m, n].TileType = TileID.Pearlstone;
                            WorldGen.SquareTileFrame(m, n);
                        }
                        else if (Main.tile[m, n].TileType is TileID.CorruptThorns or TileID.CrimsonThorns)
                        {
                            WorldGen.KillTile(m, n);
                        }
                    }
                    else if(WorldGen.crimson)
                    {
                        if (ModContent.GetInstance<InfectedQualitiesServerConfig>().InfectedBiomes)
                        {
                            if (Main.tile[m, n].WallType is WallID.JungleUnsafe or WallID.Jungle)
                            {
                                Main.tile[m, n].WallType = WallID.CrimsonGrassUnsafe;
                            }

                            if (Main.tile[m, n].TileType == TileID.SnowBlock)
                            {
                                Main.tile[m, n].TileType = (ushort)ModContent.TileType<CrimsonSnow>();
                                WorldGen.SquareTileFrame(m, n);
                            }
                            else if (Main.tile[m, n].TileType == ModContent.TileType<HallowedThorns>())
                            {
                                Main.tile[m, n].TileType = TileID.CrimsonThorns;
                                WorldGen.SquareTileFrame(m, n);
                            }
                        }

                        if (Main.tile[m, n].WallType is WallID.Stone or WallID.EbonstoneUnsafe or WallID.PearlstoneBrickUnsafe)
                        {
                            Main.tile[m, n].WallType = WallID.CrimstoneUnsafe;
                        }

                        if (Main.tileMoss[Main.tile[m, n].TileType])
                        {
                            Main.tile[m, n].TileType = TileID.Crimstone;
                            WorldGen.SquareTileFrame(m, n);
                        }
                        else if (Main.tile[m, n].TileType is TileID.CorruptThorns or TileID.JungleThorns)
                        {
                            Main.tile[m, n].TileType = TileID.CrimsonThorns;
                            WorldGen.SquareTileFrame(m, n);
                        }
                    }
                    else
                    {
                        if (ModContent.GetInstance<InfectedQualitiesServerConfig>().InfectedBiomes)
                        {
                            if (Main.tile[m, n].WallType is WallID.JungleUnsafe or WallID.Jungle)
                            {
                                Main.tile[m, n].WallType = WallID.CorruptGrassUnsafe;
                            }

                            if (Main.tile[m, n].TileType == TileID.SnowBlock)
                            {
                                Main.tile[m, n].TileType = (ushort)ModContent.TileType<CorruptSnow>();
                                WorldGen.SquareTileFrame(m, n);
                            }
                            else if (Main.tile[m, n].TileType == ModContent.TileType<HallowedThorns>())
                            {
                                Main.tile[m, n].TileType = TileID.CorruptThorns;
                                WorldGen.SquareTileFrame(m, n);
                            }
                        }

                        if (Main.tile[m, n].WallType is WallID.Stone or WallID.CrimstoneUnsafe or WallID.PearlstoneBrickUnsafe)
                        {
                            Main.tile[m, n].WallType = WallID.EbonstoneUnsafe;
                        }

                        if (Main.tileMoss[Main.tile[m, n].TileType])
                        {
                            Main.tile[m, n].TileType = TileID.Ebonstone;
                            WorldGen.SquareTileFrame(m, n);
                        }
                        else if (Main.tile[m, n].TileType is TileID.CrimsonThorns or TileID.JungleThorns)
                        {
                            Main.tile[m, n].TileType = TileID.CorruptThorns;
                            WorldGen.SquareTileFrame(m, n);
                        }
                    }
                });
            }
        }

        private static void WorldGen_Convert(On_WorldGen.orig_Convert orig, int i, int j, int conversionType, int size)
        {
            if (ModContent.GetInstance<InfectedQualitiesServerConfig>().InfectedBiomes)
            {
                for (int k = i - size; k <= i + size; k++)
                {
                    for (int l = j - size; l <= j + size; l++)
                    {
                        if (!WorldGen.InWorld(k, l, 1) || Math.Abs(k - i) + Math.Abs(l - j) >= 6)
                        {
                            continue;
                        }

                        if (conversionType == BiomeConversionID.Purity && Main.tile[k, l].TileType == ModContent.TileType<HallowedThorns>())
                        {
                            Main.tile[k, l].TileType = TileID.JungleThorns;
                            WorldGen.SquareTileFrame(k, l);
                            NetMessage.SendTileSquare(-1, k, l);
                        }
                        else if (TileID.Sets.Conversion.Snow[Main.tile[k, l].TileType])
                        {
                            int snowBlock = -1;
                            switch (conversionType)
                            {
                                case BiomeConversionID.Purity:
                                    snowBlock = TileID.SnowBlock;
                                    break;
                                case BiomeConversionID.Corruption:
                                    snowBlock = ModContent.TileType<CorruptSnow>();
                                    break;
                                case BiomeConversionID.Crimson:
                                    snowBlock = ModContent.TileType<CrimsonSnow>();
                                    break;
                                case BiomeConversionID.Hallow:
                                    snowBlock = ModContent.TileType<HallowedSnow>();
                                    break;
                            }

                            if (snowBlock != -1 && Main.tile[k, l].TileType != snowBlock)
                            {
                                WorldGen.TryKillingTreesAboveIfTheyWouldBecomeInvalid(k, l, snowBlock);
                                Main.tile[k, l].TileType = (ushort)snowBlock;
                                WorldGen.SquareTileFrame(k, l);
                                NetMessage.SendTileSquare(-1, k, l);
                            }
                        }
                        else if (conversionType == BiomeConversionID.Hallow)
                        {
                            //Shitty workaround for blue solution destroying any kind of thorn it comes across, I am NOT going to make a seperate IL edit just to prevent hallowed thorns from breaking.
                            TileID.Sets.Conversion.Thorn[ModContent.TileType<HallowedThorns>()] = false;

                            if (TileID.Sets.Conversion.JungleGrass[Main.tile[k, l].TileType] && Main.tile[k, l].TileType != ModContent.TileType<HallowedJungleGrass>())
                            {
                                WorldGen.TryKillingTreesAboveIfTheyWouldBecomeInvalid(k, l, ModContent.TileType<HallowedJungleGrass>());
                                Main.tile[k, l].TileType = (ushort)ModContent.TileType<HallowedJungleGrass>();
                                WorldGen.SquareTileFrame(k, l);
                                NetMessage.SendTileSquare(-1, k, l);
                            }
                            else if (Main.tile[k, l].TileType == TileID.JungleThorns)
                            {
                                Main.tile[k, l].TileType = (ushort)ModContent.TileType<HallowedThorns>();
                                WorldGen.SquareTileFrame(k, l);
                                NetMessage.SendTileSquare(-1, k, l);
                            }
                        }
                    }
                }
            }
            orig(i, j, conversionType, size);

            //Undo the shitty workaround so that it can be affected by other solutions.
            if (ModContent.GetInstance<InfectedQualitiesServerConfig>().InfectedBiomes && conversionType == BiomeConversionID.Hallow) TileID.Sets.Conversion.Thorn[ModContent.TileType<HallowedThorns>()] = true;
        }

        private static int TileDrawing_GetTreeVariant(On_TileDrawing.orig_GetTreeVariant orig, int x, int y)
        {
            if(ModContent.GetInstance<InfectedQualitiesServerConfig>().InfectedBiomes)
            {
                if (Main.tile[x, y].TileType == ModContent.TileType<CorruptSnow>())
                {
                    return 0;
                }
                else if (Main.tile[x, y].TileType == ModContent.TileType<CrimsonSnow>())
                {
                    return 4;
                }
                else if (Main.tile[x, y].TileType == ModContent.TileType<HallowedJungleGrass>() || Main.tile[x, y].TileType == ModContent.TileType<HallowedSnow>())
                {
                    return 2;
                }
            }
            return orig(x, y);
        }

        private static void WorldGen_GetTreeLeaf(On_WorldGen.orig_GetTreeLeaf orig, int x, Tile topTile, Tile t, ref int treeHeight, out int treeFrame, out int passStyle)
        {
            if (ModContent.GetInstance<InfectedQualitiesServerConfig>().InfectedBiomes && (t.TileType == ModContent.TileType<HallowedJungleGrass>() || t.TileType == ModContent.TileType<HallowedSnow>()))
            {
                treeFrame = 0;
                passStyle = 917;
                if (WorldGen.GetHollowTreeFoliageStyle() != 20)
                {
                    treeFrame += x % 3 * 3;
                    switch (treeFrame)
                    {
                        case 0:
                            passStyle += 2;
                            break;
                        case 1:
                            passStyle += 1;
                            break;
                        case 2:
                            passStyle += 7;
                            break;
                        case 3:
                            passStyle += 4;
                            break;
                        case 4:
                            passStyle += 5;
                            break;
                        case 5:
                            passStyle += 6;
                            break;
                        case 6:
                            passStyle += 3;
                            break;
                        case 7:
                            passStyle += 8;
                            break;
                    }
                }
                else
                {
                    passStyle += 196;
                    treeFrame += x % 6 * 3;
                    switch (treeFrame)
                    {
                        case 3:
                        case 5:
                            passStyle += 1;
                            break;
                        case 4:
                            passStyle += 2;
                            break;
                        case 6:
                            passStyle += 3;
                            break;
                        case 7:
                            passStyle += 4;
                            break;
                        case 8:
                            passStyle += 5;
                            break;
                        case 9:
                        case 10:
                        case 11:
                            passStyle += 6;
                            break;
                        case 12:
                        case 13:
                        case 14:
                            passStyle += 7;
                            break;
                        case 15:
                        case 16:
                        case 17:
                            passStyle += 8;
                            break;
                    }
                }
                treeHeight += 5;
            }
            else
            {
                orig(x, topTile, t, ref treeHeight, out treeFrame, out passStyle);
            }
        }

        private static void WorldGen_GetCommonTreeFoliageData(ILContext il)
        {
            ILCursor cursor = new(il);
            ILLabel label = cursor.DefineLabel();

            if (cursor.TryGotoNext(MoveType.After, i => i.MatchLdcI4(TileID.GolfGrassHallowed), i => i.MatchBeq(out label)))
            {
                cursor.Emit(OpCodes.Ldloc, 5);
                cursor.EmitDelegate(() =>
                {
                    if(ModContent.GetInstance<InfectedQualitiesServerConfig>().InfectedBiomes)
                    {
                        return ModContent.TileType<HallowedJungleGrass>();
                    }
                    return -1;
                });
                cursor.Emit(OpCodes.Beq, label);

                cursor.Emit(OpCodes.Ldloc, 5);
                cursor.EmitDelegate(() =>
                {
                    if (ModContent.GetInstance<InfectedQualitiesServerConfig>().InfectedBiomes)
                    {
                        return ModContent.TileType<HallowedSnow>();
                    }
                    return -1;
                });
                cursor.Emit(OpCodes.Beq, label);
            }
        }

        private static void WorldGen_CheckAlch(ILContext il)
        {
            ILCursor cursor = new(il);
            ILLabel label = cursor.DefineLabel();

            if (cursor.TryGotoNext(MoveType.After, i => i.MatchLdcI4(TileID.JungleGrass), i => i.MatchBeq(out label)))
            {
                cursor.Emit(OpCodes.Ldsflda, typeof(Main).GetField("tile"));
                cursor.Emit(OpCodes.Ldarg_0);
                cursor.Emit(OpCodes.Ldarg_1);
                cursor.Emit(OpCodes.Ldc_I4_1);
                cursor.Emit(OpCodes.Add);
                cursor.Emit(OpCodes.Call, typeof(Tilemap).GetMethod("get_Item", [typeof(int), typeof(int)]));
                cursor.Emit(OpCodes.Stloc_2);
                cursor.Emit(OpCodes.Ldloca, 2);
                cursor.Emit(OpCodes.Call, typeof(Tile).GetMethod("get_TileType"));
                cursor.Emit(OpCodes.Ldind_U2);
                cursor.EmitDelegate(() =>
                {
                    if (ModContent.GetInstance<InfectedQualitiesServerConfig>().InfectedBiomes)
                    {
                        return ModContent.TileType<HallowedJungleGrass>();
                    }
                    return -1;
                });
                cursor.Emit(OpCodes.Beq, label);
            }

            label = cursor.DefineLabel();
            if (cursor.TryGotoNext(MoveType.After, i => i.MatchLdcI4(TileID.CorruptJungleGrass), i => i.MatchBeq(out label)))
            {
                cursor.Emit(OpCodes.Ldsflda, typeof(Main).GetField("tile"));
                cursor.Emit(OpCodes.Ldarg_0);
                cursor.Emit(OpCodes.Ldarg_1);
                cursor.Emit(OpCodes.Ldc_I4_1);
                cursor.Emit(OpCodes.Add);
                cursor.Emit(OpCodes.Call, typeof(Tilemap).GetMethod("get_Item", [typeof(int), typeof(int)]));
                cursor.Emit(OpCodes.Stloc_2);
                cursor.Emit(OpCodes.Ldloca, 2);
                cursor.Emit(OpCodes.Call, typeof(Tile).GetMethod("get_TileType"));
                cursor.Emit(OpCodes.Ldind_U2);
                cursor.Emit(OpCodes.Ldc_I4, TileID.CorruptIce);
                cursor.Emit(OpCodes.Beq, label);

                cursor.Emit(OpCodes.Ldsflda, typeof(Main).GetField("tile"));
                cursor.Emit(OpCodes.Ldarg_0);
                cursor.Emit(OpCodes.Ldarg_1);
                cursor.Emit(OpCodes.Ldc_I4_1);
                cursor.Emit(OpCodes.Add);
                cursor.Emit(OpCodes.Call, typeof(Tilemap).GetMethod("get_Item", [typeof(int), typeof(int)]));
                cursor.Emit(OpCodes.Stloc_2);
                cursor.Emit(OpCodes.Ldloca, 2);
                cursor.Emit(OpCodes.Call, typeof(Tile).GetMethod("get_TileType"));
                cursor.Emit(OpCodes.Ldind_U2);
                cursor.EmitDelegate(() =>
                {
                    if (ModContent.GetInstance<InfectedQualitiesServerConfig>().InfectedBiomes)
                    {
                        return ModContent.TileType<CorruptSnow>();
                    }
                    return -1;
                });
                cursor.Emit(OpCodes.Beq, label);

                cursor.Emit(OpCodes.Ldsflda, typeof(Main).GetField("tile"));
                cursor.Emit(OpCodes.Ldarg_0);
                cursor.Emit(OpCodes.Ldarg_1);
                cursor.Emit(OpCodes.Ldc_I4_1);
                cursor.Emit(OpCodes.Add);
                cursor.Emit(OpCodes.Call, typeof(Tilemap).GetMethod("get_Item", [typeof(int), typeof(int)]));
                cursor.Emit(OpCodes.Stloc_2);
                cursor.Emit(OpCodes.Ldloca, 2);
                cursor.Emit(OpCodes.Call, typeof(Tile).GetMethod("get_TileType"));
                cursor.Emit(OpCodes.Ldind_U2);
                cursor.Emit(OpCodes.Ldc_I4, TileID.FleshIce);
                cursor.Emit(OpCodes.Beq, label);

                cursor.Emit(OpCodes.Ldsflda, typeof(Main).GetField("tile"));
                cursor.Emit(OpCodes.Ldarg_0);
                cursor.Emit(OpCodes.Ldarg_1);
                cursor.Emit(OpCodes.Ldc_I4_1);
                cursor.Emit(OpCodes.Add);
                cursor.Emit(OpCodes.Call, typeof(Tilemap).GetMethod("get_Item", [typeof(int), typeof(int)]));
                cursor.Emit(OpCodes.Stloc_2);
                cursor.Emit(OpCodes.Ldloca, 2);
                cursor.Emit(OpCodes.Call, typeof(Tile).GetMethod("get_TileType"));
                cursor.Emit(OpCodes.Ldind_U2);
                cursor.EmitDelegate(() =>
                {
                    if (ModContent.GetInstance<InfectedQualitiesServerConfig>().InfectedBiomes)
                    {
                        return ModContent.TileType<CrimsonSnow>();
                    }
                    return -1;
                });
                cursor.Emit(OpCodes.Beq, label);
            }

            if (cursor.TryGotoNext(MoveType.After, i => i.MatchLdcI4(TileID.CorruptIce)))
            {
                cursor.Emit(OpCodes.Pop);
                cursor.Emit(OpCodes.Ldc_I4_M1);
            }

            if (cursor.TryGotoNext(MoveType.After, i => i.MatchLdcI4(TileID.FleshIce)))
            {
                cursor.Emit(OpCodes.Pop);
                cursor.EmitDelegate(() =>
                {
                    if (ModContent.GetInstance<InfectedQualitiesServerConfig>().InfectedBiomes)
                    {
                        return ModContent.TileType<HallowedSnow>();
                    }
                    return -1;
                });
            }
        }

        private static void WorldGen_PlantAlch(ILContext il)
        {
            ILCursor cursor = new(il);
            ILLabel label = cursor.DefineLabel();

            if (cursor.TryGotoNext(MoveType.After, i => i.MatchLdcI4(TileID.JungleGrass), i => i.MatchBeq(out label)))
            {
                cursor.Emit(OpCodes.Ldsflda, typeof(Main).GetField("tile"));
                cursor.Emit(OpCodes.Ldloc_0);
                cursor.Emit(OpCodes.Ldloc_1);
                cursor.Emit(OpCodes.Call, typeof(Tilemap).GetMethod("get_Item", [typeof(int), typeof(int)]));
                cursor.Emit(OpCodes.Stloc, 8);
                cursor.Emit(OpCodes.Ldloca, 8);
                cursor.Emit(OpCodes.Call, typeof(Tile).GetMethod("get_TileType"));
                cursor.Emit(OpCodes.Ldind_U2);
                cursor.EmitDelegate(() =>
                {
                    if (ModContent.GetInstance<InfectedQualitiesServerConfig>().InfectedBiomes)
                    {
                        return ModContent.TileType<HallowedJungleGrass>();
                    }
                    return -1;
                });
                cursor.Emit(OpCodes.Beq, label);
            }

            label = cursor.DefineLabel();
            if (cursor.TryGotoNext(MoveType.After, i => i.MatchLdcI4(TileID.CrimsonGrass), i => i.MatchBeq(out label)))
            {
                cursor.Emit(OpCodes.Ldsflda, typeof(Main).GetField("tile"));
                cursor.Emit(OpCodes.Ldloc_0);
                cursor.Emit(OpCodes.Ldloc_1);
                cursor.Emit(OpCodes.Call, typeof(Tilemap).GetMethod("get_Item", [typeof(int), typeof(int)]));
                cursor.Emit(OpCodes.Stloc, 8);
                cursor.Emit(OpCodes.Ldloca, 8);
                cursor.Emit(OpCodes.Call, typeof(Tile).GetMethod("get_TileType"));
                cursor.Emit(OpCodes.Ldind_U2);
                cursor.Emit(OpCodes.Ldc_I4, TileID.CorruptIce);
                cursor.Emit(OpCodes.Beq, label);

                cursor.Emit(OpCodes.Ldsflda, typeof(Main).GetField("tile"));
                cursor.Emit(OpCodes.Ldloc_0);
                cursor.Emit(OpCodes.Ldloc_1);
                cursor.Emit(OpCodes.Call, typeof(Tilemap).GetMethod("get_Item", [typeof(int), typeof(int)]));
                cursor.Emit(OpCodes.Stloc, 8);
                cursor.Emit(OpCodes.Ldloca, 8);
                cursor.Emit(OpCodes.Call, typeof(Tile).GetMethod("get_TileType"));
                cursor.Emit(OpCodes.Ldind_U2);
                cursor.EmitDelegate(() =>
                {
                    if (ModContent.GetInstance<InfectedQualitiesServerConfig>().InfectedBiomes)
                    {
                        return ModContent.TileType<CorruptSnow>();
                    }
                    return -1;
                });
                cursor.Emit(OpCodes.Beq, label);

                cursor.Emit(OpCodes.Ldsflda, typeof(Main).GetField("tile"));
                cursor.Emit(OpCodes.Ldloc_0);
                cursor.Emit(OpCodes.Ldloc_1);
                cursor.Emit(OpCodes.Call, typeof(Tilemap).GetMethod("get_Item", [typeof(int), typeof(int)]));
                cursor.Emit(OpCodes.Stloc, 8);
                cursor.Emit(OpCodes.Ldloca, 8);
                cursor.Emit(OpCodes.Call, typeof(Tile).GetMethod("get_TileType"));
                cursor.Emit(OpCodes.Ldind_U2);
                cursor.Emit(OpCodes.Ldc_I4, TileID.FleshIce);
                cursor.Emit(OpCodes.Beq, label);

                cursor.Emit(OpCodes.Ldsflda, typeof(Main).GetField("tile"));
                cursor.Emit(OpCodes.Ldloc_0);
                cursor.Emit(OpCodes.Ldloc_1);
                cursor.Emit(OpCodes.Call, typeof(Tilemap).GetMethod("get_Item", [typeof(int), typeof(int)]));
                cursor.Emit(OpCodes.Stloc, 8);
                cursor.Emit(OpCodes.Ldloca, 8);
                cursor.Emit(OpCodes.Call, typeof(Tile).GetMethod("get_TileType"));
                cursor.Emit(OpCodes.Ldind_U2);
                cursor.EmitDelegate(() =>
                {
                    if (ModContent.GetInstance<InfectedQualitiesServerConfig>().InfectedBiomes)
                    {
                        return ModContent.TileType<CrimsonSnow>();
                    }
                    return -1;
                });
                cursor.Emit(OpCodes.Beq, label);
            }

            if (cursor.TryGotoNext(MoveType.After, i => i.MatchLdcI4(TileID.CorruptIce)))
            {
                cursor.Emit(OpCodes.Pop);
                cursor.Emit(OpCodes.Ldc_I4_M1);
            }

            if (cursor.TryGotoNext(MoveType.After, i => i.MatchLdcI4(TileID.FleshIce)))
            {
                cursor.Emit(OpCodes.Pop);
                cursor.EmitDelegate(() =>
                {
                    if (ModContent.GetInstance<InfectedQualitiesServerConfig>().InfectedBiomes)
                    {
                        return ModContent.TileType<HallowedSnow>();
                    }
                    return -1;
                });
            }
        }

        private static void WorldGen_PlaceAlch(ILContext il)
        {
            ILCursor cursor = new(il);
            ILLabel label = cursor.DefineLabel();
            if (cursor.TryGotoNext(MoveType.After, i => i.MatchLdcI4(TileID.JungleGrass), i => i.MatchBeq(out label)))
            {
                cursor.Emit(OpCodes.Ldsflda, typeof(Main).GetField("tile"));
                cursor.Emit(OpCodes.Ldarg_0);
                cursor.Emit(OpCodes.Ldarg_1);
                cursor.Emit(OpCodes.Ldc_I4_1);
                cursor.Emit(OpCodes.Add);
                cursor.Emit(OpCodes.Call, typeof(Tilemap).GetMethod("get_Item", [typeof(int), typeof(int)]));
                cursor.Emit(OpCodes.Stloc_0);
                cursor.Emit(OpCodes.Ldloca, 0);
                cursor.Emit(OpCodes.Call, typeof(Tile).GetMethod("get_TileType"));
                cursor.Emit(OpCodes.Ldind_U2);
                cursor.EmitDelegate(() =>
                {
                    if (ModContent.GetInstance<InfectedQualitiesServerConfig>().InfectedBiomes)
                    {
                        return ModContent.TileType<HallowedJungleGrass>();
                    }
                    return -1;
                });
                cursor.Emit(OpCodes.Beq, label);
            }

            label = cursor.DefineLabel();
            if (cursor.TryGotoNext(MoveType.After, i => i.MatchLdcI4(TileID.CorruptJungleGrass), i => i.MatchBeq(out label)))
            {
                cursor.Emit(OpCodes.Ldsflda, typeof(Main).GetField("tile"));
                cursor.Emit(OpCodes.Ldarg_0);
                cursor.Emit(OpCodes.Ldarg_1);
                cursor.Emit(OpCodes.Ldc_I4_1);
                cursor.Emit(OpCodes.Add);
                cursor.Emit(OpCodes.Call, typeof(Tilemap).GetMethod("get_Item", [typeof(int), typeof(int)]));
                cursor.Emit(OpCodes.Stloc_0);
                cursor.Emit(OpCodes.Ldloca, 0);
                cursor.Emit(OpCodes.Call, typeof(Tile).GetMethod("get_TileType"));
                cursor.Emit(OpCodes.Ldind_U2);
                cursor.Emit(OpCodes.Ldc_I4, TileID.CorruptIce);
                cursor.Emit(OpCodes.Beq, label);

                cursor.Emit(OpCodes.Ldsflda, typeof(Main).GetField("tile"));
                cursor.Emit(OpCodes.Ldarg_0);
                cursor.Emit(OpCodes.Ldarg_1);
                cursor.Emit(OpCodes.Ldc_I4_1);
                cursor.Emit(OpCodes.Add);
                cursor.Emit(OpCodes.Call, typeof(Tilemap).GetMethod("get_Item", [typeof(int), typeof(int)]));
                cursor.Emit(OpCodes.Stloc_0);
                cursor.Emit(OpCodes.Ldloca, 0);
                cursor.Emit(OpCodes.Call, typeof(Tile).GetMethod("get_TileType"));
                cursor.Emit(OpCodes.Ldind_U2);
                cursor.EmitDelegate(() =>
                {
                    if (ModContent.GetInstance<InfectedQualitiesServerConfig>().InfectedBiomes)
                    {
                        return ModContent.TileType<CorruptSnow>();
                    }
                    return -1;
                });
                cursor.Emit(OpCodes.Beq, label);

                cursor.Emit(OpCodes.Ldsflda, typeof(Main).GetField("tile"));
                cursor.Emit(OpCodes.Ldarg_0);
                cursor.Emit(OpCodes.Ldarg_1);
                cursor.Emit(OpCodes.Ldc_I4_1);
                cursor.Emit(OpCodes.Add);
                cursor.Emit(OpCodes.Call, typeof(Tilemap).GetMethod("get_Item", [typeof(int), typeof(int)]));
                cursor.Emit(OpCodes.Stloc_0);
                cursor.Emit(OpCodes.Ldloca, 0);
                cursor.Emit(OpCodes.Call, typeof(Tile).GetMethod("get_TileType"));
                cursor.Emit(OpCodes.Ldind_U2);
                cursor.Emit(OpCodes.Ldc_I4, TileID.FleshIce);
                cursor.Emit(OpCodes.Beq, label);

                cursor.Emit(OpCodes.Ldsflda, typeof(Main).GetField("tile"));
                cursor.Emit(OpCodes.Ldarg_0);
                cursor.Emit(OpCodes.Ldarg_1);
                cursor.Emit(OpCodes.Ldc_I4_1);
                cursor.Emit(OpCodes.Add);
                cursor.Emit(OpCodes.Call, typeof(Tilemap).GetMethod("get_Item", [typeof(int), typeof(int)]));
                cursor.Emit(OpCodes.Stloc_0);
                cursor.Emit(OpCodes.Ldloca, 0);
                cursor.Emit(OpCodes.Call, typeof(Tile).GetMethod("get_TileType"));
                cursor.Emit(OpCodes.Ldind_U2);
                cursor.EmitDelegate(() =>
                {
                    if (ModContent.GetInstance<InfectedQualitiesServerConfig>().InfectedBiomes)
                    {
                        return ModContent.TileType<CrimsonSnow>();
                    }
                    return -1;
                });
                cursor.Emit(OpCodes.Beq, label);
            }

            if (cursor.TryGotoNext(MoveType.After, i => i.MatchLdcI4(TileID.CorruptIce)))
            {
                cursor.Emit(OpCodes.Pop);
                cursor.Emit(OpCodes.Ldc_I4_M1);
            }

            if (cursor.TryGotoNext(MoveType.After, i => i.MatchLdcI4(TileID.FleshIce)))
            {
                cursor.Emit(OpCodes.Pop);
                cursor.EmitDelegate(() =>
                {
                    if (ModContent.GetInstance<InfectedQualitiesServerConfig>().InfectedBiomes)
                    {
                        return ModContent.TileType<HallowedJungleGrass>();
                    }
                    return -1;
                });
            }
        }

        private static void WorldGen_ScoreRoom(ILContext il)
        {
            ILCursor cursor = new(il);
            ILLabel label = cursor.DefineLabel();

            if (cursor.TryGotoNext(MoveType.After, i => i.MatchStloc(6)))
            {
                cursor.EmitDelegate(() => ModContent.GetInstance<InfectedQualitiesServerConfig>().AllowNPCsInEvilBiomes);
                cursor.Emit(OpCodes.Brfalse, label);
                cursor.Emit(OpCodes.Ldc_I4_0);
                cursor.Emit(OpCodes.Stloc, 6);

                cursor.MarkLabel(label);
            }
        }

        private static int WorldGen_GetTileTypeCountByCategory(On_WorldGen.orig_GetTileTypeCountByCategory orig, int[] tileTypeCounts, TileScanGroup group)
        {
            return group switch
            {
                TileScanGroup.Corruption => orig(tileTypeCounts, group) + tileTypeCounts[TileID.CorruptVines] + tileTypeCounts[TileID.CorruptJungleGrass] + (ModContent.GetInstance<InfectedQualitiesServerConfig>().InfectedBiomes ? tileTypeCounts[ModContent.TileType<CorruptSnow>()] : 0),
                TileScanGroup.Crimson => orig(tileTypeCounts, group) + tileTypeCounts[TileID.CrimsonPlants] + tileTypeCounts[TileID.CrimsonVines] + tileTypeCounts[TileID.CrimsonJungleGrass] + (ModContent.GetInstance<InfectedQualitiesServerConfig>().InfectedBiomes ? tileTypeCounts[ModContent.TileType<CrimsonSnow>()] : 0),
                TileScanGroup.Hallow => orig(tileTypeCounts, group) + tileTypeCounts[TileID.HallowedVines] + tileTypeCounts[TileID.GolfGrassHallowed] + (ModContent.GetInstance<InfectedQualitiesServerConfig>().InfectedBiomes ? tileTypeCounts[ModContent.TileType<HallowedThorns>()] + tileTypeCounts[ModContent.TileType<HallowedSnow>()] + tileTypeCounts[ModContent.TileType<HallowedJungleGrass>()] : 0),
                _ => orig(tileTypeCounts, group)
            };
        }

        private static void WorldGen_UpdateWorld_Inner(ILContext il)
        {
            ILCursor cursor = new(il);
            if (cursor.TryGotoNext(MoveType.After, i => i.MatchCeq(), i => i.MatchStsfld(typeof(WorldGen).GetField("AllowedToSpreadInfections"))))
            {
                cursor.EmitDelegate(() =>
                {
                    if (ModContent.GetInstance<InfectedQualitiesServerConfig>().DisableInfectionSpread || WorldGen.IsGeneratingHardMode)
                    {
                        WorldGen.AllowedToSpreadInfections = false;
                    }
                });
            }
        }

        private static void WorldGen_UpdateWorld_GrassGrowth(ILContext il)
        {
            ILCursor cursor = new(il);
            ILLabel label = cursor.DefineLabel();

            if (cursor.TryGotoNext(MoveType.After, i => i.MatchLdcI4(TileID.PlanteraBulb), i => i.MatchBneUn(out label)))
            {
                cursor.Emit(OpCodes.Ldsflda, typeof(Main).GetField("tile"));
                cursor.Emit(OpCodes.Ldloc, 17);
                cursor.Emit(OpCodes.Ldloc, 18);
                cursor.Emit(OpCodes.Call, typeof(Tilemap).GetMethod("get_Item", [typeof(int), typeof(int)]));
                cursor.Emit(OpCodes.Stloc, 9);
                cursor.Emit(OpCodes.Ldloca, 9);
                cursor.Emit(OpCodes.Call, typeof(Tile).GetMethod("get_TileType"));
                cursor.Emit(OpCodes.Ldind_U4);
                cursor.EmitDelegate(() =>
                {
                    if (ModContent.GetInstance<InfectedQualitiesServerConfig>().InfectedBiomes)
                    {
                        return ModContent.TileType<InfectedPlanteraBulb>();
                    }
                    return -1;
                });
                cursor.Emit(OpCodes.Bne_Un, label);
            }

            label = cursor.DefineLabel();
            if (cursor.TryGotoNext(MoveType.After, i => i.MatchLdcI4(TileID.LifeFruit), i => i.MatchBneUn(out label)))
            {
                cursor.Emit(OpCodes.Ldsflda, typeof(Main).GetField("tile"));
                cursor.Emit(OpCodes.Ldloc, 21);
                cursor.Emit(OpCodes.Ldloc, 22);
                cursor.Emit(OpCodes.Call, typeof(Tilemap).GetMethod("get_Item", [typeof(int), typeof(int)]));
                cursor.Emit(OpCodes.Stloc, 9);
                cursor.Emit(OpCodes.Ldloca, 9);
                cursor.Emit(OpCodes.Call, typeof(Tile).GetMethod("get_TileType"));
                cursor.Emit(OpCodes.Ldind_U4);
                cursor.EmitDelegate(() =>
                {
                    if (ModContent.GetInstance<InfectedQualitiesServerConfig>().InfectedBiomes)
                    {
                        return ModContent.TileType<InfectedLifeFruit>();
                    }
                    return -1;
                });
                cursor.Emit(OpCodes.Bne_Un, label);
            }
        }

        private static void WorldGen_hardUpdateWorld(ILContext il)
        {
            ILCursor cursor = new(il);
            ILLabel label = cursor.DefineLabel();
            ILLabel nextLabel = cursor.DefineLabel();

            if (cursor.TryGotoNext(i => i.MatchLdcI4(1), i => i.MatchStloc(15), i => i.MatchBr(out label)))
            {
                cursor.EmitDelegate(() => NPC.downedPlantBoss);
                cursor.Emit(OpCodes.Brtrue, label);
                cursor.Emit(OpCodes.Ldc_I4_0);
                cursor.Emit(OpCodes.Stloc, 15);
            }

            label = cursor.DefineLabel();
            if (cursor.TryGotoNext(MoveType.After, i => i.MatchLdcI4(TileID.CorruptVines), i => i.MatchBeq(out label)))
            {
                cursor.Emit(OpCodes.Ldloc_0);
                cursor.Emit(OpCodes.Ldc_I4, TileID.CorruptPlants);
                cursor.Emit(OpCodes.Beq, label);
            }

            label = cursor.DefineLabel();
            if (cursor.TryGotoNext(MoveType.After, i => i.MatchLdloc(22), i => i.MatchLdloc(23), i => i.MatchLdcI4(10), i => i.MatchCall(typeof(WorldGen).GetMethod("InWorld")), i => i.MatchBrfalse(out label)))
            {
                cursor.Emit(OpCodes.Ldloc, 22);
                cursor.Emit(OpCodes.Ldloc, 23);
                cursor.Emit(OpCodes.Call, typeof(WorldGen).GetMethod("nearbyChlorophyte", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static, [typeof(int), typeof(int)]));
                cursor.Emit(OpCodes.Brfalse, nextLabel);

                cursor.Emit(OpCodes.Ldloc, 22);
                cursor.Emit(OpCodes.Ldloc, 23);
                cursor.Emit(OpCodes.Call, typeof(WorldGen).GetMethod("ChlorophyteDefense", [typeof(int), typeof(int)]));
                cursor.Emit(OpCodes.Br, label);

                cursor.MarkLabel(nextLabel);
            }
        }

        private static bool WorldGen_nearbyChlorophyte(On_WorldGen.orig_nearbyChlorophyte orig, int i, int j)
        {
            if (!NPC.downedPlantBoss)
            {
                return false;
            }
            return orig(i, j);
        }

        private static void WorldGen_ChlorophyteDefense(On_WorldGen.orig_ChlorophyteDefense orig, int x, int y)
        {
            orig(x, y);

            if (Main.tile[x, y].TileType == TileID.HallowedGrass)
            {
                Main.tile[x, y].TileType = TileID.JungleGrass;
                WorldGen.SquareTileFrame(x, y);
                NetMessage.SendTileSquare(-1, x, y);
            }
            else if (Main.tile[x, y].TileType == TileID.Pearlstone)
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
            else if (Main.tile[x, y].TileType == TileID.HallowSandstone)
            {
                Main.tile[x, y].TileType = TileID.Sandstone;
                WorldGen.SquareTileFrame(x, y);
                NetMessage.SendTileSquare(-1, x, y);
            }
            else if (Main.tile[x, y].TileType == TileID.HallowedIce)
            {
                Main.tile[x, y].TileType = TileID.IceBlock;
                WorldGen.SquareTileFrame(x, y);
                NetMessage.SendTileSquare(-1, x, y);
            }
            else if (Main.tile[x, y].TileType is TileID.HallowedPlants or TileID.HallowedPlants2 or TileID.HallowedVines)
            {
                WorldGen.KillTile(x, y);
                NetMessage.SendTileSquare(-1, x, y);
            }
        }

        private static void WorldGen_CheckLilyPad(ILContext il)
        {
            ILCursor cursor = new(il);
            ILLabel label = cursor.DefineLabel();
            ILLabel nextLabel = cursor.DefineLabel();

            if (cursor.TryGotoNext(MoveType.After, i => i.MatchStloc2()))
            {
                cursor.Emit(OpCodes.Ldloc_1);
                cursor.Emit(OpCodes.Ldc_I4, TileID.CorruptGrass);
                cursor.Emit(OpCodes.Beq, label);
                cursor.Emit(OpCodes.Ldloc_1);
                cursor.Emit(OpCodes.Ldc_I4, TileID.CorruptJungleGrass);
                cursor.Emit(OpCodes.Bne_Un, nextLabel);

                cursor.MarkLabel(label);
                cursor.Emit(OpCodes.Ldc_I4, 72);
                cursor.Emit(OpCodes.Stloc_2);
                cursor.MarkLabel(nextLabel);

                label = cursor.DefineLabel();
                nextLabel = cursor.DefineLabel();

                cursor.Emit(OpCodes.Ldloc_1);
                cursor.Emit(OpCodes.Ldc_I4, TileID.CrimsonGrass);
                cursor.Emit(OpCodes.Beq, label);
                cursor.Emit(OpCodes.Ldloc_1);
                cursor.Emit(OpCodes.Ldc_I4, TileID.CrimsonJungleGrass);
                cursor.Emit(OpCodes.Bne_Un, nextLabel);

                cursor.MarkLabel(label);
                cursor.Emit(OpCodes.Ldc_I4, 54);
                cursor.Emit(OpCodes.Stloc_2);
                cursor.MarkLabel(nextLabel);
            }

            if (cursor.TryGotoNext(MoveType.After, i => i.MatchLdcI4(TileID.HallowedGrass)))
            {
                cursor.Emit(OpCodes.Pop);
                cursor.EmitDelegate(() =>
                {
                    if (ModContent.GetInstance<InfectedQualitiesServerConfig>().InfectedBiomes)
                    {
                        return ModContent.TileType<HallowedJungleGrass>();
                    }
                    return -1;
                });
            }
        }

        private static void WorldGen_CheckCatTail(ILContext il)
        {
            ILCursor cursor = new(il);
            ILLabel label = cursor.DefineLabel();
            ILLabel nextLabel = cursor.DefineLabel();

            if (cursor.TryGotoNext(MoveType.After, i => i.MatchLdcI4(-1), i => i.MatchStloc(6)))
            {
                cursor.Emit(OpCodes.Ldloc, 5);
                cursor.Emit(OpCodes.Ldc_I4, TileID.JungleGrass);
                cursor.Emit(OpCodes.Bne_Un, label);

                cursor.Emit(OpCodes.Ldc_I4, 18);
                cursor.Emit(OpCodes.Stloc, 6);
                cursor.MarkLabel(label);
                label = cursor.DefineLabel();

                cursor.Emit(OpCodes.Ldloc, 5);
                cursor.Emit(OpCodes.Ldc_I4, TileID.HallowedGrass);
                cursor.Emit(OpCodes.Beq, label);
                cursor.Emit(OpCodes.Ldloc, 5);
                cursor.EmitDelegate(() =>
                {
                    if (ModContent.GetInstance<InfectedQualitiesServerConfig>().InfectedBiomes)
                    {
                        return ModContent.TileType<HallowedJungleGrass>();
                    }
                    return -1;
                });
                cursor.Emit(OpCodes.Beq, label);
                cursor.Emit(OpCodes.Ldloc, 5);
                cursor.Emit(OpCodes.Ldc_I4, TileID.Pearlsand);
                cursor.Emit(OpCodes.Bne_Un, nextLabel);

                cursor.MarkLabel(label);
                cursor.Emit(OpCodes.Ldc_I4, 36);
                cursor.Emit(OpCodes.Stloc, 6);
                cursor.MarkLabel(nextLabel);
            }
        }

        private static void WorldGen_GetDesiredStalagtiteStyle(ILContext il)
        {
            ILCursor cursor = new(il);

            if (cursor.TryGotoNext(MoveType.After, i => i.MatchLdcI4(TileID.FleshIce)))
            {
                ILLabel label = cursor.DefineLabel();

                cursor.Emit(OpCodes.Beq, label);
                cursor.Emit(OpCodes.Ldarg_3);
                cursor.Emit(OpCodes.Ldind_I4);
                cursor.EmitDelegate(() =>
                {
                    if (ModContent.GetInstance<InfectedQualitiesServerConfig>().InfectedBiomes)
                    {
                        return ModContent.TileType<CrimsonSnow>();
                    }
                    return -1;
                });

                cursor.Index++;
                cursor.MarkLabel(label);
            }

            if (cursor.TryGotoNext(MoveType.After, i => i.MatchLdcI4(TileID.HallowedIce)))
            {
                ILLabel label = cursor.DefineLabel();

                cursor.Emit(OpCodes.Beq, label);
                cursor.Emit(OpCodes.Ldarg_3);
                cursor.Emit(OpCodes.Ldind_I4);
                cursor.EmitDelegate(() =>
                {
                    if (ModContent.GetInstance<InfectedQualitiesServerConfig>().InfectedBiomes)
                    {
                        return ModContent.TileType<HallowedSnow>();
                    }
                    return -1;
                });

                cursor.Index++;
                cursor.MarkLabel(label);
            }

            if (cursor.TryGotoNext(MoveType.After, i => i.MatchLdcI4(TileID.CorruptIce)))
            {
                ILLabel label = cursor.DefineLabel();

                cursor.Emit(OpCodes.Beq, label);
                cursor.Emit(OpCodes.Ldarg_3);
                cursor.Emit(OpCodes.Ldind_I4);
                cursor.EmitDelegate(() =>
                {
                    if (ModContent.GetInstance<InfectedQualitiesServerConfig>().InfectedBiomes)
                    {
                        return ModContent.TileType<CorruptSnow>();
                    }
                    return -1;
                });

                cursor.Index++;
                cursor.MarkLabel(label);
            }
        }

        public void Unload() { }
    }
}