using InfectedQualities.Core;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.Metadata;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace InfectedQualities.Content.Tiles.Plants
{
    public class InfectedSapling : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;

            TileID.Sets.TreeSapling[Type] = true;
            TileID.Sets.CommonSapling[Type] = true;
            TileID.Sets.SwaysInWindBasic[Type] = true;
            TileMaterials.SetForTileId(Type, TileMaterials._materialsByName["Plant"]);

            TileObjectData.newTile.FullCopyFrom(TileObjectData.GetTileData(TileID.Saplings, 0));
            TileObjectData.newTile.AnchorValidTiles = [ModContent.TileType<CorruptSnow>(), ModContent.TileType<CrimsonSnow>(), ModContent.TileType<HallowedSnow>(), ModContent.TileType<HallowedJungleGrass>()];
            TileObjectData.addTile(Type);
            

            AdjTiles = [TileID.Saplings];
            AddMapEntry(new(151, 107, 75), Language.GetText("MapObject.Sapling"));
        }

        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;

        public override void RandomUpdate(int i, int j)
        {
            if (WorldGen.genRand.NextBool(20) && WorldGen.GrowTree(i, j) && WorldGen.PlayerLOS(i, j))
            {
                WorldGen.TreeGrowFXCheck(i, j);
            }
        }

        public override void SetSpriteEffects(int i, int j, ref SpriteEffects effects)
        {
            if (i % 2 == 1)
            {
                effects = SpriteEffects.FlipHorizontally;
            }
        }

        public override string Texture => $"Terraria/Images/Tiles_{TileID.Saplings}";

        public override bool IsLoadingEnabled(Mod mod) => ModContent.GetInstance<InfectedQualitiesServerConfig>().InfectedBiomes;
    }
}