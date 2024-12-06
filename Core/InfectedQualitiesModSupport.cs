using InfectedQualities.Content.Extras;
using Microsoft.Xna.Framework;
using System;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace InfectedQualities.Core
{
    public static class InfectedQualitiesModSupport
    {
        internal static readonly Mod CalamityMod = ModLoader.TryGetMod("CalamityMod", out Mod result) ? result : null;
        internal static readonly Mod ConfectionRebaked = ModLoader.TryGetMod("TheConfectionRebirth", out Mod result) ? result : null;
        internal static readonly Mod SpiritMod = ModLoader.TryGetMod("SpiritMod", out Mod result) ? result : null;
        internal static readonly Mod ExxoAvalon = ModLoader.TryGetMod("Avalon", out Mod result) ? result : null;
        internal static readonly Mod TerrariaOrigins = ModLoader.TryGetMod("Origins", out Mod result) ? result : null;

        internal static Color[] ModWallBiomeSight = WallID.Sets.Factory.CreateCustomSet(default(Color));

        private static readonly string[][] ModBlocks =
        [
            ["AstralGrassWall", "HardenedAstralSandWall", "AstralSandstoneWall", "AstralStoneWall", "AstralDirtWall", "AstralSnowWall", "CelestialRemainsWall", "AstralIceWall", "AstralMonolithWall"],
            ["CreamGrassWall", "HardenedCreamsandWall", "CreamsandstoneWall", "CreamstoneWall", "Creamstone2Wall", "Creamstone3Wall", "Creamstone4Wall", "Creamstone5Wall", "CookieWall", "CookieStonedWall", "PinkFairyFlossWall", "BlueIceWall", "CreamWall"],
            ["Chunkstone", "HardenedSnotsand", "Snotsandstone", "Ickgrass", "ContagionJungleGrass", "Snotsand", "YellowIce"],
            ["ContagionGrassWall", "ChunkstoneWall", "HardenedSnotsandWallUnsafe", "SnotsandstoneWallUnsafe", "ContagionLumpWallUnsafe", "ContagionMouldWallUnsafe", "ContagionCystWallUnsafe", "ContagionBoilWallUnsafe"],
            ["Defiled_Stone", "Defiled_Grass", "Defiled_Sand", "Defiled_Sandstone", "Hardened_Defiled_Sand", "Defiled_Ice", "Defiled_Jungle_Grass"],
            ["Defiled_Stone_Wall", "Defiled_Sandstone_Wall", "Hardened_Defiled_Sand_Wall", "Defiled_Grass_Wall_Natural"],
            ["Riven_Flesh", "Riven_Grass", "Silica", "Brittle_Quartz", "Quartz", "Primordial_Permafrost", "Riven_Jungle_Grass"],
            ["Riven_Flesh_Wall", "Quartz_Wall", "Brittle_Quartz_Wall", "Riven_Grass_Wall_Natural"],
            ["GreenCreamMoss", "BrownCreamMoss", "RedCreamMoss", "BlueCreamMoss", "PurpleCreamMoss", "LavaCreamMoss", "KryptonCreamMoss", "XenomCreamMoss"]
        ];

        public static void PostSetupContent()
        {
            if (CalamityMod != null)
            {
                foreach (string wallName in ModBlocks[0])
                {
                    ModWallBiomeSight[CalamityMod.Find<ModWall>(wallName).Type] = Color.Cyan;
                }
            }

            if (ConfectionRebaked != null)
            {
                Color confectionGlow = new(210, 196, 145);
                foreach (string wallName in ModBlocks[1])
                {
                    ModWallBiomeSight[ConfectionRebaked.Find<ModWall>(wallName).Type] = confectionGlow;
                }
            }

            if (ExxoAvalon != null)
            {
                Color contagionGlow = new(170, 255, 0);
                foreach (string wallName in ModBlocks[3])
                {
                    ModWallBiomeSight[ExxoAvalon.Find<ModWall>(wallName).Type] = contagionGlow;
                }
            }

            if(TerrariaOrigins != null)
            {
                foreach(string wallName in ModBlocks[5])
                {
                    ModWallBiomeSight[TerrariaOrigins.Find<ModWall>(wallName).Type] = Color.White;
                }

                foreach (string wallName in ModBlocks[7])
                {
                    ModWallBiomeSight[TerrariaOrigins.Find<ModWall>(wallName).Type] = Color.Cyan;
                }
            }
        }

        public static void HandleRecipeGroups()
        {
            if (RecipeGroup.recipeGroupIDs.TryGetValue("TeleportationPylons", out int pylonIndex))
            {
                if (CalamityMod != null)
                {
                    RecipeGroup.recipeGroups[pylonIndex].ValidItems.Add(CalamityMod.Find<ModItem>("AstralPylon").Type);
                    RecipeGroup.recipeGroups[pylonIndex].ValidItems.Add(CalamityMod.Find<ModItem>("CragsPylon").Type);
                    RecipeGroup.recipeGroups[pylonIndex].ValidItems.Add(CalamityMod.Find<ModItem>("SulphurPylon").Type);
                    RecipeGroup.recipeGroups[pylonIndex].ValidItems.Add(CalamityMod.Find<ModItem>("SunkenPylon").Type);
                }

                if (SpiritMod != null)
                {
                    RecipeGroup.recipeGroups[pylonIndex].ValidItems.Add(SpiritMod.Find<ModItem>("AsteroidPylonItem").Type);
                    RecipeGroup.recipeGroups[pylonIndex].ValidItems.Add(SpiritMod.Find<ModItem>("BriarPylonItem").Type);
                    RecipeGroup.recipeGroups[pylonIndex].ValidItems.Add(SpiritMod.Find<ModItem>("SpiritPylonItem").Type);
                }
            }
        }

        public static bool EnoughTilesForAltEvilBiome(SceneMetrics sceneMetrics)
        {
            if (ExxoAvalon != null)
            {
                int contagionTileCount = 0;
                foreach (string tileName in ModBlocks[2])
                {
                    contagionTileCount += sceneMetrics.GetTileCount(ExxoAvalon.Find<ModTile>(tileName).Type);
                }

                if (contagionTileCount >= 300)
                {
                    return true;
                }
            }

            if (TerrariaOrigins != null)
            {
                int defiledTiles = 0;
                foreach (string tileName in ModBlocks[4])
                {
                    defiledTiles += sceneMetrics.GetTileCount(TerrariaOrigins.Find<ModTile>(tileName).Type);
                }

                if(defiledTiles > 200)
                {
                    return true;
                }

                int rivenTiles = 0;
                foreach (string tileName in ModBlocks[6])
                {
                    rivenTiles += sceneMetrics.GetTileCount(TerrariaOrigins.Find<ModTile>(tileName).Type);
                }

                if (rivenTiles > 200)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsAltEvilBlock(int i, int j)
        {
            if (ExxoAvalon != null)
            {
                foreach (string wallName in ModBlocks[3])
                {
                    if (Main.tile[i, j].WallType == ExxoAvalon.Find<ModWall>(wallName).Type)
                    {
                        return true;
                    }
                }

                foreach (string tileName in ModBlocks[2])
                {
                    if (Main.tile[i, j].TileType == ExxoAvalon.Find<ModTile>(tileName).Type)
                    {
                        return true;
                    }
                }
            }

            if (TerrariaOrigins != null)
            {
                foreach (string wallName in ModBlocks[5])
                {
                    if (Main.tile[i, j].WallType == TerrariaOrigins.Find<ModWall>(wallName).Type)
                    {
                        return true;
                    }
                }

                foreach (string tileName in ModBlocks[4])
                {
                    if (Main.tile[i, j].TileType == TerrariaOrigins.Find<ModTile>(tileName).Type)
                    {
                        return true;
                    }
                }

                foreach (string wallName in ModBlocks[7])
                {
                    if (Main.tile[i, j].WallType == TerrariaOrigins.Find<ModWall>(wallName).Type)
                    {
                        return true;
                    }
                }

                foreach (string tileName in ModBlocks[6])
                {
                    if (Main.tile[i, j].TileType == TerrariaOrigins.Find<ModTile>(tileName).Type)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool IsAltMoss(int i, int j, MossType mossType)
        {
            if(ConfectionRebaked != null && mossType < MossType.Neon)
            {
                foreach (string tileName in ModBlocks[8])
                {
                    if (Main.tile[i, j].TileType == ConfectionRebaked.Find<ModTile>(tileName).Type)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool AltLibraryInfection(bool good)
        {
            if(ModLoader.TryGetMod("AltLibrary", out Mod altLib))
            {
                Type biomeManager = altLib.Code.GetType("AltLibrary.Common.Systems.WorldBiomeManager");
                BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Static;
                if (good)
                {
                    return (string)biomeManager.GetField("worldHallowName", flags).GetValue(null) != "";
                }
                else
                {
                    return (string)biomeManager.GetField("worldEvilName", flags).GetValue(null) != "";
                }
            }
            return false;
        }
    }
}
