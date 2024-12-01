using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using InfectedQualities.Content.Tiles;
using InfectedQualities.Core;

namespace InfectedQualities.Common
{
    public class InfectedQualitiesGlobalItem : GlobalItem
    {
        public override bool? UseItem(Item item, Player player)
        {
            if (player.IsTargetTileInItemRange(item))
            {
                if (WorldGen.TileType(Player.tileTargetX, Player.tileTargetY) == TileID.Mud)
                {
                    WorldGen.PlaceTile(Player.tileTargetX, Player.tileTargetY, ModContent.TileType<HallowedJungleGrass>(), forced: true);
                    player.ConsumeItem(item.type);
                    return true;
                }
            }
            return null;
        }

        public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.HallowedSeeds;

        public override bool IsLoadingEnabled(Mod mod) => ModContent.GetInstance<InfectedQualitiesServerConfig>().InfectedBiomes;
    }
}
