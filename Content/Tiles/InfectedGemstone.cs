using InfectedQualities.Content.Extras;
using InfectedQualities.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace InfectedQualities.Content.Tiles
{
    [Autoload(false)]
    public class InfectedGemstone(InfectionType infectionType, GemType gemType) : ModTile
    {
        private Asset<Texture2D> GemTexture { get; set; } = null;

        public override void SetStaticDefaults()
        {
            Main.tileStone[Type] = true;
            Main.tileBrick[Type] = true;
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileShine[Type] = 900;
            Main.tileShine2[Type] = true;
            Main.tileSpelunker[Type] = true;

            MinPick = 65;
            MineResist = 2;
            HitSound = SoundID.Tink;

            switch (infectionType)
            {
                case InfectionType.Corrupt:
                    TileID.Sets.Corrupt[Type] = true;
                    TileID.Sets.AddCorruptionTile(Type);
                    TileID.Sets.CorruptBiomeSight[Type] = true;
                    TileID.Sets.CorruptCountCollection.Add(Type);
                    break;
                case InfectionType.Crimson:
                    TileID.Sets.Crimson[Type] = true;
                    TileID.Sets.AddCrimsonTile(Type);
                    TileID.Sets.CrimsonBiomeSight[Type] = true;
                    TileID.Sets.CrimsonCountCollection.Add(Type);
                    break;
                case InfectionType.Hallowed:
                    TileID.Sets.Hallow[Type] = true;
                    TileID.Sets.HallowBiome[Type] = 1;
                    TileID.Sets.HallowBiomeSight[Type] = true;
                    TileID.Sets.HallowCountCollection.Add(Type);
                    break;
            }

            switch(gemType)
            {
                case GemType.Sapphire:
                    DustType = DustID.GemSapphire;
                    AddMapEntry(new(110, 140, 182), Language.GetText("ItemName.Sapphire"));
                    break;
                case GemType.Ruby:
                    DustType = DustID.GemRuby;
                    AddMapEntry(new(196, 96, 114), Language.GetText("ItemName.Ruby"));
                    break;
                case GemType.Emerald:
                    DustType = DustID.GemEmerald;
                    AddMapEntry(new(56, 150, 97), Language.GetText("ItemName.Emerald"));
                    break;
                case GemType.Topaz:
                    DustType = DustID.GemTopaz;
                    AddMapEntry(new(160, 118, 58), Language.GetText("ItemName.Topaz"));
                    break;
                case GemType.Amethyst:
                    DustType = DustID.GemAmethyst;
                    AddMapEntry(new(140, 58, 166), Language.GetText("ItemName.Amethyst"));
                    break;
                case GemType.Diamond:
                    DustType = DustID.GemDiamond;
                    AddMapEntry(new(125, 191, 197), Language.GetText("ItemName.Diamond"));
                    break;
                case GemType.Amber:
                    DustType = DustID.GemAmber;
                    AddMapEntry(new(233, 180, 90), Language.GetText("ItemName.Amber"));
                    break;
            }

            GemTexture = ModContent.Request<Texture2D>("InfectedQualities/Content/Extras/Tiles/" + gemType.ToString() + "_Gemstone");
        }

        public override void RandomUpdate(int i, int j)
        {
            InfectedQualitiesUtilities.DefaultInfectionSpread(i, j, infectionType, InfectedQualitiesUtilities.GetGemstoneType(null, gemType));
        }

        public override IEnumerable<Item> GetItemDrops(int i, int j)
        {
            return [new Item(ItemID.Search.GetId(gemType.ToString()))];
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            if (!Main.tile[i, j].IsTileInvisible)
            {
                Color color = Lighting.GetColor(i, j);
                Vector2 offscreenVector = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
                Vector2 drawVector = new Vector2(i * 16, j * 16) + offscreenVector - Main.screenPosition;
                Rectangle frame = new(Main.tile[i, j].TileFrameX, Main.tile[i, j].TileFrameY, 16, 16);

                if (Main.tile[i, j].Slope == SlopeType.Solid && !Main.tile[i, j].IsHalfBlock)
                {
                    spriteBatch.Draw(GemTexture.Value, drawVector, frame, color, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                }
                else if (Main.tile[i, j].IsHalfBlock)
                {
                    drawVector += new Vector2(0, 8);
                    frame.Height = 8;
                    spriteBatch.Draw(GemTexture.Value, drawVector, frame, color, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                }
                else
                {
                    int width = 2;
                    for (int q = 0; q < 8; q++)
                    {
                        int num = q * -2;
                        int height = 16 - q * 2;
                        int yOffset = 16 - height;
                        int xOffset;
                        switch (Main.tile[i, j].Slope)
                        {
                            case SlopeType.SlopeDownLeft:
                                num = 0;
                                xOffset = q * 2;
                                height = 14 - q * 2;
                                yOffset = 0;
                                break;
                            case SlopeType.SlopeDownRight:
                                num = 0;
                                xOffset = 16 - q * 2 - 2;
                                height = 14 - q * 2;
                                yOffset = 0;
                                break;
                            case SlopeType.SlopeUpLeft:
                                xOffset = q * 2;
                                break;
                            default:
                                xOffset = 16 - q * 2 - 2;
                                break;
                        }
                        frame = new(Main.tile[i, j].TileFrameX + xOffset, Main.tile[i, j].TileFrameY + yOffset, width, height);
                        spriteBatch.Draw(GemTexture.Value, drawVector + new Vector2(xOffset, q * width + num), frame, color, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                    }

                    int slopeOffset = (Main.tile[i, j].Slope <= SlopeType.SlopeDownRight) ? 14 : 0;
                    frame = new(Main.tile[i, j].TileFrameX, Main.tile[i, j].TileFrameY + slopeOffset, 16, 2);
                    spriteBatch.Draw(GemTexture.Value, drawVector + new Vector2(0, slopeOffset), frame, color, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                }
            }

            if (infectionType == InfectionType.Corrupt && Main.rand.NextBool(500))
            {
                Dust.NewDust(new Vector2(i * 16, j * 16), 16, 16, DustID.Demonite);
            }
        }

        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;

        public override string Name => infectionType.ToString() + gemType.ToString() + "Gemstone";

        public override string Texture => $"Terraria/Images/Tiles_{infectionType switch
        {
            InfectionType.Corrupt => TileID.Ebonstone,
            InfectionType.Crimson => TileID.Crimstone,
            InfectionType.Hallowed => TileID.Pearlstone,
            _ => TileID.Stone
        }}";

        public override bool IsLoadingEnabled(Mod mod) => ModContent.GetInstance<InfectedQualitiesServerConfig>().InfectedGemstones;
    }
}
