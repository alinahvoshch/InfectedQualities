using InfectedQualities.Content.Extras.Tiles;
using InfectedQualities.Core;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace InfectedQualities.Content.Tiles.Plants
{
    [Autoload(false)]
    public class InfectedSnowTree(InfectionType infectionType) : ModTree, ILoadable
    {
        private Asset<Texture2D> TreeTexture { get; set; } = null;
        private Asset<Texture2D> BranchTexture { get; set; } = null;
        private Asset<Texture2D> TopTexture { get; set; } = null;

        public override TreeTypes CountsAsTreeType => infectionType switch
        {
            InfectionType.Corrupt => TreeTypes.Corrupt,
            InfectionType.Crimson => TreeTypes.Crimson,
            InfectionType.Hallowed => TreeTypes.Hallowed,
            _ => TreeTypes.Snow,
        };

        public override TreePaintingSettings TreeShaderSettings => new()
        {
            UseSpecialGroups = true,
            SpecialGroupMinimalHueValue = 11f / 72f,
            SpecialGroupMaximumHueValue = 0.25f,
            SpecialGroupMinimumSaturationValue = 0.88f,
            SpecialGroupMaximumSaturationValue = 1
        };

        public override int DropWood() => infectionType switch
        {
            InfectionType.Corrupt => ItemID.Ebonwood,
            InfectionType.Crimson => ItemID.Shadewood,
            InfectionType.Hallowed => ItemID.Pearlwood,
            _ => ItemID.BorealWood,
        };

        public override int CreateDust() => infectionType switch
        {
            InfectionType.Corrupt => DustID.Ebonwood,
            InfectionType.Crimson => DustID.Shadewood_Tree,
            InfectionType.Hallowed => DustID.t_PearlWood,
            _ => DustID.WoodFurniture,
        };

        public override int TreeLeaf() => infectionType switch
        {
            InfectionType.Corrupt => GoreID.TreeLeaf_Corruption,
            InfectionType.Crimson => GoreID.TreeLeaf_Crimson,
            InfectionType.Hallowed => 921,
            _ => -1,
        };

        public override int SaplingGrowthType(ref int style)
        {
            style = (int)infectionType + 3;
            return ModContent.TileType<InfectedSapling>();
        }

        public override Asset<Texture2D> GetBranchTextures() => BranchTexture;

        public override Asset<Texture2D> GetTexture() => TreeTexture;

        public override Asset<Texture2D> GetTopTextures() => TopTexture;

        public override void SetStaticDefaults()
        {
            GrowsOnTileId = [TileUtilities.GetSnowType(infectionType)];

            //Adding this here so that the hallowed jungle trees can find their anchor tile. This class doesn't actually handle hallowed jungle trees, I'm just killing 2 birds with 1 stone.
            if(infectionType == InfectionType.Hallowed)
            {
                GrowsOnTileId = [.. GrowsOnTileId, ModContent.TileType<HallowedJungleGrass>()];
            }

            BranchTexture = ModContent.Request<Texture2D>("InfectedQualities/Content/Tiles/Plants/" + infectionType.ToString() + "SnowTree_Branch");
            TreeTexture = ModContent.Request<Texture2D>("InfectedQualities/Content/Tiles/Plants/" + infectionType.ToString() + "SnowTree");
            TopTexture = ModContent.Request<Texture2D>("InfectedQualities/Content/Tiles/Plants/" + infectionType.ToString() + "SnowTree_Top");
        }

        public override void SetTreeFoliageSettings(Tile tile, ref int xoffset, ref int treeFrame, ref int floorY, ref int topTextureFrameWidth, ref int topTextureFrameHeight) 
        {
            if(infectionType == InfectionType.Hallowed)
            {
                topTextureFrameHeight = 140;
            }
        }

        bool ILoadable.IsLoadingEnabled(Mod mod) => ModContent.GetInstance<InfectedQualitiesServerConfig>().InfectedBiomes;
    }
}
