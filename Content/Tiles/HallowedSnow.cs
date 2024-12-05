using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using InfectedQualities.Core;
using InfectedQualities.Common;

namespace InfectedQualities.Content.Tiles
{
    public class HallowedSnow : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileShine[Type] = 9000;
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileBrick[Type] = true;

            Main.tileMergeDirt[Type] = true;
            Main.tileMerge[TileID.Dirt][Type] = true;
            InfectedQualitiesUtilities.TileMerge(Type, TileID.HallowedIce);
            InfectedQualitiesUtilities.TileMerge(Type, TileID.Slush);

            TileID.Sets.CanGrowCrystalShards[Type] = true;
            TileID.Sets.CanBeDugByShovel[Type] = true;
            TileID.Sets.CanBeClearedDuringOreRunner[Type] = true;
            TileID.Sets.Snow[Type] = true;
            TileID.Sets.IcesSnow[Type] = true;
            TileID.Sets.ChecksForMerge[Type] = true;

            TileID.Sets.Hallow[Type] = true;
            TileID.Sets.HallowBiome[Type] = 1;
            TileID.Sets.HallowBiomeSight[Type] = true;
            TileID.Sets.HallowCountCollection.Add(Type);

            TileID.Sets.SnowBiome[Type] = 1;
            TileID.Sets.Conversion.Snow[Type] = true;

            MineResist = 0.5f;
            HitSound = SoundID.Item50;
            DustType = DustID.SnowBlock;
            RegisterItemDrop(ModContent.ItemType<Items.Placables.HallowedSnow>());
            AddMapEntry(new(247, 228, 233));
        }

        public override bool HasWalkDust() => true;

        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;

        public override void RandomUpdate(int i, int j) => InfectedQualitiesUtilities.DefaultInfectionSpread(i, j, InfectionType.Hallowed, TileID.SnowBlock);

        public override void ModifyFrameMerge(int i, int j, ref int up, ref int down, ref int left, ref int right, ref int upLeft, ref int upRight, ref int downLeft, ref int downRight)
        {
            if (down == TileID.Stalactite) down = Type;
        }

        public override bool IsLoadingEnabled(Mod mod) => ModContent.GetInstance<InfectedQualitiesServerConfig>().InfectedBiomes;

    }
}
