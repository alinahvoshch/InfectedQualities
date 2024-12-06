using Terraria;
using Terraria.ID;
using Terraria.GameContent;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.Enums;
using InfectedQualities.Core;
using InfectedQualities.Content.Extras;

namespace InfectedQualities.Content.Tiles.Plants
{
    public class CrimsonTree : ModTree, ILoadable
    {
        public override TreeTypes CountsAsTreeType => TreeTypes.Crimson;

        public override TreePaintingSettings TreeShaderSettings => new()
        {
            UseSpecialGroups = true,
            SpecialGroupMinimalHueValue = 11f / 72f,
            SpecialGroupMaximumHueValue = 0.25f,
            SpecialGroupMinimumSaturationValue = 0.88f,
            SpecialGroupMaximumSaturationValue = 1f
        };

        public override void SetStaticDefaults() => GrowsOnTileId = [InfectedQualitiesUtilities.GetSnowType(InfectionType.Crimson)];

        public override int DropWood() => ItemID.Shadewood;

        public override int TreeLeaf() => GoreID.TreeLeaf_Crimson;

        public override int CreateDust() => DustID.Shadewood;

        public override int SaplingGrowthType(ref int style)
        {
            style = 4;
            return ModContent.TileType<InfectedSapling>();
        }

        public override Asset<Texture2D> GetBranchTextures() => TextureAssets.TreeBranch[5];

        public override Asset<Texture2D> GetTexture() => null;

        public override Asset<Texture2D> GetTopTextures() => TextureAssets.TreeTop[5];

        public override void SetTreeFoliageSettings(Tile tile, ref int xoffset, ref int treeFrame, ref int floorY, ref int topTextureFrameWidth, ref int topTextureFrameHeight)
        {
        }

        bool ILoadable.IsLoadingEnabled(Mod mod) => ModContent.GetInstance<InfectedQualitiesServerConfig>().InfectedBiomes;
    }
}
