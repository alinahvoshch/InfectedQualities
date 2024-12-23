using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using InfectedQualities.Content.Items;

namespace InfectedQualities.Core
{
    public class InfectedQualitiesPlayer : ModPlayer
    {
        private int LastChest = -1;

        public override void PreUpdateBuffs()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (Main.hardMode && Player.chest == -1 && LastChest >= 0)
                {
                    Chest lastChest = Main.chest[LastChest];
                    if (lastChest != null && Chest.FindChest(lastChest.x, lastChest.y) != 0)
                    {
                        int x = lastChest.x;
                        int y = lastChest.y;
                        int keyOfNaught = 0;
                        bool other = false;
                        if (TileID.Sets.BasicChest[Main.tile[x, y].TileType] && (Main.tile[x, y].TileFrameX / 36 < 5 || Main.tile[x, y].TileFrameX / 36 > 6))
                        {
                            for (int i = 0; i < 40; i++)
                            {
                                if (lastChest.item[i].type == ModContent.ItemType<KeyOfNaught>())
                                {
                                    keyOfNaught += lastChest.item[i].stack;
                                    if (keyOfNaught > 1) break;
                                }
                                else if (lastChest.item[i].type != ItemID.None)
                                {
                                    other = true;
                                    break;
                                }
                            }
                        }

                        if (keyOfNaught == 1 && !other)
                        {
                            if (TileID.Sets.BasicChest[Main.tile[x, y].TileType])
                            {
                                if (Main.tile[x, y].TileFrameX % 36 != 0) x--;
                                if (Main.tile[x, y].TileFrameY % 36 != 0) y--;

                                for (int l = 0; l < 40; l++) lastChest.item[l] = new Item();

                                Chest.DestroyChest(x, y);
                                for (int j = x; j <= x + 1; j++)
                                {
                                    for (int k = y; k <= y + 1; k++)
                                    {
                                        if (TileID.Sets.BasicChest[Main.tile[j, k].TileType])
                                        {
                                            Main.tile[j, k].ClearTile();
                                        }
                                    }
                                }

                                int chestID = 1;
                                if (Main.tile[x, y].TileType == TileID.Containers2) chestID = 5;
                                else if (Main.tile[x, y].TileType >= TileID.Count) chestID = 101;

                                NetMessage.SendData(MessageID.ChestUpdates, -1, -1, null, chestID, x, y, 0, Chest.FindChest(x, y));
                                NetMessage.SendTileSquare(-1, x, y, 3);
                            }

                            short mimicID = WorldGen.crimson ? NPCID.BigMimicCorruption : NPCID.BigMimicCrimson;
                            int npcIndex = NPC.NewNPC(Entity.GetSource_TileInteraction(x, y), x * 16, y * 16 + 32, mimicID);
                            Main.npc[npcIndex].whoAmI = npcIndex;
                            NetMessage.SendData(MessageID.SyncNPC, number : npcIndex);
                            Main.npc[npcIndex].BigMimicSpawnSmoke();
                        }
                    }
                }

                LastChest = Player.chest;
            }
        }

        public override void UpdateAutopause() => LastChest = Player.chest;

        public override bool IsLoadingEnabled(Mod mod) => ModContent.GetInstance<InfectedQualitiesServerConfig>().KeyOfNaught;
    }
}
