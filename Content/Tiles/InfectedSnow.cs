using InfectedQualities.Content.Extras;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using InfectedQualities.Core;

namespace InfectedQualities.Content.Tiles
{
    [Autoload(false)]
    public class InfectedSnow(InfectionType infectionType) : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileBrick[Type] = true;

            Main.tileMergeDirt[Type] = true;
            Main.tileMerge[TileID.Dirt][Type] = true;
            InfectedQualitiesUtilities.TileMerge(Type, TileID.Slush);

            TileID.Sets.CanBeDugByShovel[Type] = true;
            TileID.Sets.CanBeClearedDuringOreRunner[Type] = true;
            TileID.Sets.Snow[Type] = true;
            TileID.Sets.IcesSnow[Type] = true;
            TileID.Sets.ChecksForMerge[Type] = true;

            TileID.Sets.SnowBiome[Type] = 1;
            TileID.Sets.Conversion.Snow[Type] = true;

            MineResist = 0.5f;
            HitSound = SoundID.Item50;
            DustType = DustID.SnowBlock;

            switch(infectionType)
            {
                case InfectionType.Corrupt:
                    TileID.Sets.Corrupt[Type] = true;
                    TileID.Sets.AddCorruptionTile(Type);
                    TileID.Sets.CorruptBiomeSight[Type] = true;
                    TileID.Sets.CorruptCountCollection.Add(Type);
                    InfectedQualitiesUtilities.TileMerge(Type, TileID.CorruptIce);
                    RegisterItemDrop(ModContent.ItemType<Items.Placables.CorruptSnow>());
                    AddMapEntry(new(214, 203, 236));
                    break;
                case InfectionType.Crimson:
                    TileID.Sets.Crimson[Type] = true;
                    TileID.Sets.AddCrimsonTile(Type);
                    TileID.Sets.CrimsonBiomeSight[Type] = true;
                    TileID.Sets.CrimsonCountCollection.Add(Type);
                    InfectedQualitiesUtilities.TileMerge(Type, TileID.FleshIce);
                    RegisterItemDrop(ModContent.ItemType<Items.Placables.CrimsonSnow>());
                    AddMapEntry(new(234, 210, 205));
                    break;
                case InfectionType.Hallowed:
                    Main.tileShine[Type] = 9000;
                    TileID.Sets.Hallow[Type] = true;
                    TileID.Sets.HallowBiome[Type] = 1;
                    TileID.Sets.HallowBiomeSight[Type] = true;
                    TileID.Sets.HallowCountCollection.Add(Type);
                    TileID.Sets.CanGrowCrystalShards[Type] = true;
                    InfectedQualitiesUtilities.TileMerge(Type, TileID.HallowedIce);
                    RegisterItemDrop(ModContent.ItemType<Items.Placables.HallowedSnow>());
                    AddMapEntry(new(247, 228, 233));
                    break;
            }
        }

        public override bool HasWalkDust() => true;

        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;

        public override void RandomUpdate(int i, int j) => InfectedQualitiesUtilities.DefaultInfectionSpread(i, j, infectionType, TileID.SnowBlock);

        public override void ModifyFrameMerge(int i, int j, ref int up, ref int down, ref int left, ref int right, ref int upLeft, ref int upRight, ref int downLeft, ref int downRight)
        {
            if (down == TileID.Stalactite) down = Type;
        }

        public override string Name => infectionType.ToString() + "Snow";

        public override bool IsLoadingEnabled(Mod mod) => ModContent.GetInstance<InfectedQualitiesServerConfig>().InfectedBiomes;
    }
}
