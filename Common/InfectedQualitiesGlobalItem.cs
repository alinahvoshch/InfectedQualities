using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using InfectedQualities.Content.Tiles;
using InfectedQualities.Core;
using InfectedQualities.Content.Extras;
using Terraria.Audio;
using Terraria.DataStructures;

namespace InfectedQualities.Common
{
    public class InfectedQualitiesGlobalItem : GlobalItem
    {
        public override bool? UseItem(Item item, Player player)
        {
            if (ModContent.GetInstance<InfectedQualitiesServerConfig>().InfectedBiomes && item.type == ItemID.HallowedSeeds && player.IsInTileInteractionRange(Player.tileTargetX, Player.tileTargetY, TileReachCheckSettings.Simple) && WorldGen.TileType(Player.tileTargetX, Player.tileTargetY) == TileID.Mud)
            {
                Main.tile[Player.tileTargetX, Player.tileTargetY].TileType = (ushort)ModContent.TileType<HallowedJungleGrass>();
                WorldGen.SquareTileFrame(Player.tileTargetX, Player.tileTargetY);
                SoundEngine.PlaySound(SoundID.Dig, player.position);
                player.ConsumeItem(item.type);
                return true;
            }
            else if(ModContent.GetInstance<InfectedQualitiesServerConfig>().InfectedMosses)
            {
                MossType? mossType = item.type switch
                {
                    ItemID.GreenMoss => MossType.Green,
                    ItemID.BrownMoss => MossType.Brown,
                    ItemID.RedMoss => MossType.Red,
                    ItemID.BlueMoss => MossType.Blue,
                    ItemID.PurpleMoss => MossType.Purple,
                    ItemID.LavaMoss => MossType.Lava,
                    ItemID.XenonMoss => MossType.Xenon,
                    ItemID.ArgonMoss => MossType.Argon,
                    ItemID.VioletMoss => MossType.Neon,
                    ItemID.RainbowMoss => MossType.Helium,
                    _ => null
                };
                
                if(mossType.HasValue && player.IsInTileInteractionRange(Player.tileTargetX, Player.tileTargetY, TileReachCheckSettings.Simple))
                {
                    InfectionType? infectionType = WorldGen.TileType(Player.tileTargetX, Player.tileTargetY) switch
                    {
                        TileID.Ebonstone => InfectionType.Corrupt,
                        TileID.Crimstone => InfectionType.Crimson,
                        TileID.Pearlstone => InfectionType.Hallowed,
                        _ => null
                    };

                    if (infectionType.HasValue)
                    {
                        Main.tile[Player.tileTargetX, Player.tileTargetY].TileType = InfectedQualitiesUtilities.GetMossType(infectionType, mossType.Value);
                        WorldGen.SquareTileFrame(Player.tileTargetX, Player.tileTargetY);
                        SoundEngine.PlaySound(SoundID.Dig, player.position);
                        player.ConsumeItem(item.type);
                        return true;
                    }
                }
            }
            return null;
        }

        public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type is ItemID.HallowedSeeds or ItemID.GreenMoss or ItemID.BrownMoss or ItemID.RedMoss or ItemID.BlueMoss or ItemID.PurpleMoss or ItemID.LavaMoss or ItemID.XenonMoss or ItemID.ArgonMoss or ItemID.VioletMoss or ItemID.RainbowMoss;
    }
}
