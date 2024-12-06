using InfectedQualities.Content.Extras;
using InfectedQualities.Core;
using Terraria.ID;
using Terraria.ModLoader;

namespace InfectedQualities.Content.Items.Placables
{
    public class CorruptSnow : ModItem
    {
        public override string LocalizationCategory => "Items.Placables";

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.SnowBlock);
            Item.createTile = InfectedQualitiesUtilities.GetSnowType(InfectionType.Corrupt);
            ItemID.Sets.ExtractinatorMode[Type] = Type;
        }

        public override void ExtractinatorUse(int extractinatorBlockType, ref int resultType, ref int resultStack)
        {
            if (extractinatorBlockType == TileID.ChlorophyteExtractinator)
            {
                resultType = ItemID.SnowBlock;
                resultStack = 1;
            }
        }

        public override bool IsLoadingEnabled(Mod mod) => ModContent.GetInstance<InfectedQualitiesServerConfig>().InfectedBiomes;
    }

    public class CrimsonSnow : ModItem
    {
        public override string LocalizationCategory => "Items.Placables";

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.SnowBlock);
            Item.createTile = InfectedQualitiesUtilities.GetSnowType(InfectionType.Crimson);
            ItemID.Sets.ExtractinatorMode[Type] = ModContent.ItemType<CorruptSnow>();
        }

        public override bool IsLoadingEnabled(Mod mod) => ModContent.GetInstance<InfectedQualitiesServerConfig>().InfectedBiomes;
    }

    public class HallowedSnow : ModItem
    {
        public override string LocalizationCategory => "Items.Placables";

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.SnowBlock);
            Item.createTile = InfectedQualitiesUtilities.GetSnowType(InfectionType.Hallowed);
            ItemID.Sets.ExtractinatorMode[Type] = ModContent.ItemType<CorruptSnow>();
        }

        public override bool IsLoadingEnabled(Mod mod) => ModContent.GetInstance<InfectedQualitiesServerConfig>().InfectedBiomes;
    }
}
