using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using InfectedQualities.Content.Tiles.Plants;
using InfectedQualities.Core;
using InfectedQualities.Common;

namespace InfectedQualities.Content.Tiles
{
    public class HallowedJungleGrass : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileShine[Type] = 9000;
            Main.tileLighted[Type] = true;
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileBrick[Type] = true;

            TileID.Sets.CanGrowCrystalShards[Type] = true;
            TileID.Sets.CanBeDugByShovel[Type] = true;
            TileID.Sets.ResetsHalfBrickPlacementAttempt[Type] = false;
            TileID.Sets.DoesntPlaceWithTileReplacement[Type] = true;
            TileID.Sets.GrassSpecial[Type] = true;
            TileID.Sets.ChecksForMerge[Type] = true;
            TileID.Sets.SpreadOverground[Type] = true;
            TileID.Sets.SpreadUnderground[Type] = true;

            TileID.Sets.Hallow[Type] = true;
            TileID.Sets.HallowBiome[Type] = 1;
            TileID.Sets.HallowBiomeSight[Type] = true;
            TileID.Sets.HallowCountCollection.Add(Type);

            TileID.Sets.AddJungleTile(Type);
            TileID.Sets.Conversion.JungleGrass[Type] = true;

            HitSound = SoundID.Grass;
            DustType = DustID.HallowedPlants;
            RegisterItemDrop(ItemID.MudBlock);
            AddMapEntry(new(78, 193, 227));
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            if (InfectedQualitiesUtilities.TileExposedToLava(i, j))
            {
                Main.tile[i, j].TileType = TileID.Mud;
                WorldGen.SquareTileFrame(i, j);
                NetMessage.SendTileSquare(-1, i, j);
            }

            return true;
        }

        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;

        public override void RandomUpdate(int i, int j)
        {
            InfectedQualitiesUtilities.DefaultInfectionSpread(i, j, InfectionType.Hallowed, TileID.JungleGrass);

            Tile currentTile = Main.tile[i, j];
            Tile aboveTile = Main.tile[i, j - 1];
            Tile bellowTile = Main.tile[i, j + 1];

            if (WorldGen.genRand.NextBool(10) && !aboveTile.HasTile && !currentTile.IsHalfBlock && currentTile.Slope == SlopeType.Solid)
            {
                if (WallID.Sets.AllowsPlantsToGrow[aboveTile.WallType] && WallID.Sets.AllowsPlantsToGrow[currentTile.WallType])
                {
                    aboveTile.HasTile = true;
                    if(j > Main.worldSurface && WorldGen.genRand.NextBool(16))
                    {
                        aboveTile.TileType = (ushort)ModContent.TileType<HallowedThorns>();
                    }
                    else
                    {
                        aboveTile.TileType = TileID.HallowedPlants;
                        if (WorldGen.genRand.NextBool(50) && (j > Main.rockLayer || Main.remixWorld))
                        {
                            aboveTile.TileFrameX = 144;
                        }
                        else if (WorldGen.genRand.NextBool(35) || aboveTile.WallType >= WallID.GrassUnsafe && aboveTile.WallType <= WallID.HallowedGrassUnsafe)
                        {
                            aboveTile.TileFrameX = (short)(WorldGen.genRand.NextFromList(6, 7, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20) * 18);
                        }
                        else
                        {
                            aboveTile.TileFrameX = (short)(WorldGen.genRand.Next(6) * 18);
                        }
                    }
                    WorldGen.SquareTileFrame(i, j - 1);
                    aboveTile.CopyPaintAndCoating(currentTile);
                }
            }

            if (WorldGen.genRand.NextBool(60) && !bellowTile.HasTile && !currentTile.BottomSlope && bellowTile.LiquidType != LiquidID.Lava && InfectedQualitiesUtilities.RefectionMethod(i, j, "GrowMoreVines"))
            {
                bellowTile.HasTile = true;
                bellowTile.TileType = TileID.HallowedVines;
                bellowTile.CopyPaintAndCoating(currentTile);
                WorldGen.SquareTileFrame(i, j + 1);
            }

            for (int num21 = i - 1; num21 < i + 2; num21++)
            {
                for (int num22 = j - 1; num22 < j + 2; num22++)
                {
                    Tile tile = Main.tile[num21, num22];
                    if ((WorldGen.AllowedToSpreadInfections && WorldGen.CountNearBlocksTypes(num21, num22, 2, 1, [TileID.Sunflower]) == 0 && tile.TileType == TileID.JungleGrass) || tile.TileType == TileID.Mud)
                    {
                        WorldGen.SpreadGrass(num21, num22, tile.TileType, Type, false, currentTile.BlockColorAndCoating());
                        WorldGen.SquareTileFrame(num21, num22);
                        NetMessage.SendTileSquare(-1, i, j);
                    }
                }
            }

            if (j < Main.worldSurface && WorldGen.genRand.NextBool(500) && (!aboveTile.HasTile || TileID.Sets.IgnoredByGrowingSaplings[aboveTile.TileType]) && WorldGen.GrowTree(i, j) && WorldGen.PlayerLOS(i, j))
            {
                WorldGen.TreeGrowFXCheck(i, j - 1);
            }

            if(NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3 && WorldGen.genRand.NextBool(60))
            {
                InfectedQualitiesUtilities.AttemptToPlaceInfectedPlant(i, j, ModContent.TileType<InfectedPlanteraBulb>(), TileID.PlanteraBulb, 150);
            }

            if (NPC.downedMechBossAny && WorldGen.genRand.NextBool(Main.expertMode ? 30 : 40))
            {
                InfectedQualitiesUtilities.AttemptToPlaceInfectedPlant(i, j, ModContent.TileType<InfectedLifeFruit>(), TileID.LifeFruit, Main.expertMode ? 50 : 60);
            }
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (fail && !effectOnly)
            {
                noItem = true;
                Main.tile[i, j].TileType = TileID.Mud;
                WorldGen.SquareTileFrame(i, j);
                NetMessage.SendTileSquare(-1, i, j);
            }
        }

        public override bool IsLoadingEnabled(Mod mod) => ModContent.GetInstance<InfectedQualitiesServerConfig>().InfectedBiomes;
    }
}