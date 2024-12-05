using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using InfectedQualities.Core;
using InfectedQualities.Content.Extras;

namespace InfectedQualities.Content.Tiles
{
    public class CrimsonSnow : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileBrick[Type] = true;

            Main.tileMergeDirt[Type] = true;
            Main.tileMerge[TileID.Dirt][Type] = true;
            InfectedQualitiesUtilities.TileMerge(Type, TileID.FleshIce);
            InfectedQualitiesUtilities.TileMerge(Type, TileID.Slush);

            TileID.Sets.CanBeDugByShovel[Type] = true;
            TileID.Sets.CanBeClearedDuringOreRunner[Type] = true;
            TileID.Sets.Snow[Type] = true;
            TileID.Sets.IcesSnow[Type] = true;
            TileID.Sets.ChecksForMerge[Type] = true;

            TileID.Sets.Crimson[Type] = true;
            TileID.Sets.AddCrimsonTile(Type);
            TileID.Sets.CrimsonBiomeSight[Type] = true;
            TileID.Sets.CrimsonCountCollection.Add(Type);

            TileID.Sets.SnowBiome[Type] = 1;
            TileID.Sets.Conversion.Snow[Type] = true;

            MineResist = 0.5f;
            HitSound = SoundID.Item50;
            DustType = DustID.SnowBlock;
            RegisterItemDrop(ModContent.ItemType<Items.Placables.CrimsonSnow>());
            AddMapEntry(new(234, 210, 205));
        }

        public override bool HasWalkDust() => true;

        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;

        public override void RandomUpdate(int i, int j) => InfectedQualitiesUtilities.DefaultInfectionSpread(i, j, InfectionType.Crimson, TileID.SnowBlock);

        public override void ModifyFrameMerge(int i, int j, ref int up, ref int down, ref int left, ref int right, ref int upLeft, ref int upRight, ref int downLeft, ref int downRight)
        {
            if (down == TileID.Stalactite) down = Type;
        }

        public override bool IsLoadingEnabled(Mod mod) => ModContent.GetInstance<InfectedQualitiesServerConfig>().InfectedBiomes;
    }
}
