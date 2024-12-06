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
    public class HallowedTree : ModTree, ILoadable
    {
        public override TreeTypes CountsAsTreeType => TreeTypes.Hallowed;

        public override TreePaintingSettings TreeShaderSettings => new()
        {
            UseSpecialGroups = true,
            SpecialGroupMinimalHueValue = 11f / 72f,
            SpecialGroupMaximumHueValue = 0.25f,
            SpecialGroupMinimumSaturationValue = 0.88f,
            SpecialGroupMaximumSaturationValue = 1f
        };

        public override void SetStaticDefaults() => GrowsOnTileId = [ModContent.TileType<HallowedJungleGrass>(), InfectedQualitiesUtilities.GetSnowType(InfectionType.Hallowed)];

        public override int DropWood() => ItemID.Pearlwood;

        public override int CreateDust() => DustID.t_PearlWood;

        public override int SaplingGrowthType(ref int style)
        {
            style = 5;
            return ModContent.TileType<InfectedSapling>();
        }

        public override Asset<Texture2D> GetBranchTextures() => null;

        public override Asset<Texture2D> GetTexture() => null;

        public override Asset<Texture2D> GetTopTextures() => null;

        public override void SetTreeFoliageSettings(Tile tile, ref int xoffset, ref int treeFrame, ref int floorY, ref int topTextureFrameWidth, ref int topTextureFrameHeight)
        {
        }

        bool ILoadable.IsLoadingEnabled(Mod mod) => ModContent.GetInstance<InfectedQualitiesServerConfig>().InfectedBiomes;
    }
}
