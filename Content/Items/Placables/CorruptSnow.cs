using InfectedQualities.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace InfectedQualities.Content.Items.Placables
{
    public class CorruptSnow : ModItem, ILocalizedModType
    {
        string ILocalizedModType.LocalizationCategory => "Items.Placables";

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.SnowBlock);
            Item.createTile = ModContent.TileType<Tiles.CorruptSnow>();
            ItemID.Sets.ExtractinatorMode[Type] = Type;
        }

        public override void ExtractinatorUse(int extractinatorBlockType, ref int resultType, ref int resultStack)
        {
            if(extractinatorBlockType == TileID.ChlorophyteExtractinator)
            {
                resultType = ItemID.SnowBlock;
                resultStack = 1;
            }
        }

        public override bool IsLoadingEnabled(Mod mod) => ModContent.GetInstance<InfectedQualitiesServerConfig>().InfectedBiomes;
    }
}
