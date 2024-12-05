using InfectedQualities.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace InfectedQualities.Content.Items.Placables
{
    public class CrimsonSnow : ModItem
    {
        public override string LocalizationCategory => "Items.Placables";

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.SnowBlock);
            Item.createTile = ModContent.TileType<Tiles.CrimsonSnow>();
            ItemID.Sets.ExtractinatorMode[Type] = ModContent.ItemType<CorruptSnow>();
        }

        public override bool IsLoadingEnabled(Mod mod) => ModContent.GetInstance<InfectedQualitiesServerConfig>().InfectedBiomes;
    }
}
