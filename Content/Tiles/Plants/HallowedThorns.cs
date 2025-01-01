using InfectedQualities.Content.Extras;
using InfectedQualities.Content.Extras.Tiles;
using InfectedQualities.Core;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace InfectedQualities.Content.Tiles.Plants
{
    public class HallowedThorns : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileShine[Type] = 9000;
            Main.tileCut[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileNoFail[Type] = true;

            TileID.Sets.Conversion.Thorn[Type] = true;
            TileID.Sets.TileCutIgnore.IgnoreDontHurtNature[Type] = false;
            TileID.Sets.TouchDamageDestroyTile[Type] = true;
            TileID.Sets.TouchDamageImmediate[Type] = 17;
            TileID.Sets.IgnoredByGrowingSaplings[Type] = true;

            TileID.Sets.Hallow[Type] = true;
            TileID.Sets.HallowBiome[Type] = 1;
            TileID.Sets.HallowBiomeSight[Type] = true;
            TileID.Sets.AddJungleTile(Type);

            DustType = DustID.HallowedPlants;
            HitSound = SoundID.Grass;

            AddMapEntry(new(29, 160, 247), Language.GetText("MapObject.Thorn"));
        }

        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;

        public override bool IsTileDangerous(int i, int j, Player player) => true;

        public override void RandomUpdate(int i, int j)
        {
            WorldGen.GrowSpike(i, j, Type, (ushort)ModContent.TileType<HallowedJungleGrass>());
            if (InfectedQualitiesUtilities.RefectionMethod(i, j, "nearbyChlorophyte"))
            {
                if (WorldGen.AllowedToSpreadInfections && Main.remixWorld)
                {
                    WorldGen.KillTile(i, j);
                    NetMessage.SendTileSquare(-1, i, j);
                }
            }
            else
            {
                TileUtilities.SpreadInfection(i, j, InfectionType.Hallowed);
            }
        }

        public override void ModifyFrameMerge(int i, int j, ref int up, ref int down, ref int left, ref int right, ref int upLeft, ref int upRight, ref int downLeft, ref int downRight)
        {
            if (down == ModContent.TileType<HallowedJungleGrass>())
            {
                down = Type;
            }
        }

        public override bool IsLoadingEnabled(Mod mod) => ModContent.GetInstance<InfectedQualitiesServerConfig>().InfectedBiomes;
    }
}
