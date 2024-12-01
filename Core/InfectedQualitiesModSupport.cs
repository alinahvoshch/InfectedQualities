using Microsoft.Xna.Framework;
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

        internal static Color[] ModWallBiomeSight = WallID.Sets.Factory.CreateCustomSet(default(Color));

        private static readonly string[] ContagionTiles = [
            "Chunkstone",
            "HardenedSnotsand",
            "Snotsandstone",
            "Ickgrass",
            "ContagionJungleGrass",
            "Snotsand",
            "YellowIce"
        ];

        private static readonly string[] ContagionWalls = [
            "ContagionGrassWall",
            "ChunkstoneWall",
            "HardenedSnotsandWallUnsafe",
            "SnotsandstoneWallUnsafe",
            "ContagionLumpWallUnsafe",
            "ContagionMouldWallUnsafe",
            "ContagionCystWallUnsafe",
            "ContagionBoilWallUnsafe"
        ];

        public static void PostSetupContent()
        {
            if (CalamityMod != null)
            {
                ModWallBiomeSight[CalamityMod.Find<ModWall>("AstralGrassWall").Type] = Color.Cyan;
                ModWallBiomeSight[CalamityMod.Find<ModWall>("HardenedAstralSandWall").Type] = Color.Cyan;
                ModWallBiomeSight[CalamityMod.Find<ModWall>("AstralSandstoneWall").Type] = Color.Cyan;
                ModWallBiomeSight[CalamityMod.Find<ModWall>("AstralStoneWall").Type] = Color.Cyan;
                ModWallBiomeSight[CalamityMod.Find<ModWall>("AstralDirtWall").Type] = Color.Cyan;
                ModWallBiomeSight[CalamityMod.Find<ModWall>("AstralSnowWall").Type] = Color.Cyan;
                ModWallBiomeSight[CalamityMod.Find<ModWall>("CelestialRemainsWall").Type] = Color.Cyan;
                ModWallBiomeSight[CalamityMod.Find<ModWall>("AstralIceWall").Type] = Color.Cyan;
                ModWallBiomeSight[CalamityMod.Find<ModWall>("AstralMonolithWall").Type] = Color.Cyan;
            }

            if (ConfectionRebaked != null)
            {
                Color confectionGlow = new(210, 196, 145);

                ModWallBiomeSight[ConfectionRebaked.Find<ModWall>("CreamGrassWall").Type] = confectionGlow;
                ModWallBiomeSight[ConfectionRebaked.Find<ModWall>("HardenedCreamsandWall").Type] = confectionGlow;
                ModWallBiomeSight[ConfectionRebaked.Find<ModWall>("CreamsandstoneWall").Type] = confectionGlow;
                ModWallBiomeSight[ConfectionRebaked.Find<ModWall>("CreamstoneWall").Type] = confectionGlow;
                ModWallBiomeSight[ConfectionRebaked.Find<ModWall>("Creamstone2Wall").Type] = confectionGlow;
                ModWallBiomeSight[ConfectionRebaked.Find<ModWall>("Creamstone3Wall").Type] = confectionGlow;
                ModWallBiomeSight[ConfectionRebaked.Find<ModWall>("Creamstone4Wall").Type] = confectionGlow;
                ModWallBiomeSight[ConfectionRebaked.Find<ModWall>("Creamstone5Wall").Type] = confectionGlow;
                ModWallBiomeSight[ConfectionRebaked.Find<ModWall>("CookieWall").Type] = confectionGlow;
                ModWallBiomeSight[ConfectionRebaked.Find<ModWall>("CookieStonedWall").Type] = confectionGlow;
                ModWallBiomeSight[ConfectionRebaked.Find<ModWall>("PinkFairyFlossWall").Type] = confectionGlow;
                ModWallBiomeSight[ConfectionRebaked.Find<ModWall>("BlueIceWall").Type] = confectionGlow;
                ModWallBiomeSight[ConfectionRebaked.Find<ModWall>("CreamWall").Type] = confectionGlow;
            }

            if (ExxoAvalon != null)
            {
                Color contagionGlow = new(170, 255, 0);
                foreach (string wallName in ContagionWalls)
                {
                    ModWallBiomeSight[ExxoAvalon.Find<ModWall>(wallName).Type] = contagionGlow;
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
                foreach (string tileName in ContagionTiles)
                {
                    contagionTileCount += sceneMetrics.GetTileCount(ExxoAvalon.Find<ModTile>(tileName).Type);
                }

                if (contagionTileCount >= 300)
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
                foreach (string wallName in ContagionWalls)
                {
                    if (Main.tile[i, j].WallType == ExxoAvalon.Find<ModWall>(wallName).Type)
                    {
                        return true;
                    }
                }

                foreach (string tileName in ContagionTiles)
                {
                    if (Main.tile[i, j].TileType == ExxoAvalon.Find<ModTile>(tileName).Type)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
