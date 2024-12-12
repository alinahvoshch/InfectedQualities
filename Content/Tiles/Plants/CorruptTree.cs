using Terraria;
using Terraria.ID;
using Terraria.GameContent;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.Enums;
using InfectedQualities.Core;
using InfectedQualities.Content.Extras.Tiles;

namespace InfectedQualities.Content.Tiles.Plants
{
    public class CorruptTree : ModTree, ILoadable
    {
        public override TreeTypes CountsAsTreeType => TreeTypes.Corrupt;

        public override TreePaintingSettings TreeShaderSettings => new()
        {
            UseSpecialGroups = true,
            SpecialGroupMinimalHueValue = 11f / 72f,
            SpecialGroupMaximumHueValue = 0.25f,
            SpecialGroupMinimumSaturationValue = 0.88f,
            SpecialGroupMaximumSaturationValue = 1f
        };

        public override void SetStaticDefaults() => GrowsOnTileId = [TileUtilities.GetSnowType(InfectionType.Corrupt)];

        public override int DropWood() => ItemID.Ebonwood;

        public override int TreeLeaf() => GoreID.TreeLeaf_Corruption;

        public override int CreateDust() => DustID.Ebonwood;

        public override int SaplingGrowthType(ref int style)
        {
            style = 3;
            return ModContent.TileType<InfectedSapling>();
        }

        public override Asset<Texture2D> GetBranchTextures() => TextureAssets.TreeBranch[1];

        public override Asset<Texture2D> GetTexture() => null;

        public override Asset<Texture2D> GetTopTextures() => TextureAssets.TreeTop[1];

        public override void SetTreeFoliageSettings(Tile tile, ref int xoffset, ref int treeFrame, ref int floorY, ref int topTextureFrameWidth, ref int topTextureFrameHeight)
        {
        }

        bool ILoadable.IsLoadingEnabled(Mod mod) => ModContent.GetInstance<InfectedQualitiesServerConfig>().InfectedBiomes;
    }
}
