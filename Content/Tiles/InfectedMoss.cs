using InfectedQualities.Content.Extras;
using InfectedQualities.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace InfectedQualities.Content.Tiles
{
    [Autoload(false)]
    public class InfectedMoss(InfectionType infectionType, MossType mossType) : ModTile
    {
        private Asset<Texture2D> MossTexture { get; set; } = TextureAssets.GlowMask[GlowMaskID.RainbowMoss];

        private Color MossColor { get; set; }

        private ushort InfectedStoneType => infectionType switch
        {
            InfectionType.Corrupt => TileID.Ebonstone,
            InfectionType.Crimson => TileID.Crimstone,
            InfectionType.Hallowed => TileID.Pearlstone,
            _ => TileID.Stone
        };

        public override void SetStaticDefaults()
        {
            Main.tileBrick[Type] = true;
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;

            TileID.Sets.ResetsHalfBrickPlacementAttempt[Type] = false;
            TileID.Sets.Conversion.Moss[Type] = true;
            TileID.Sets.NeedsGrassFramingDirt[Type] = InfectedStoneType;
            InfectedQualitiesUtilities.TileMerge(Type, TileID.Dirt);
            InfectedQualitiesUtilities.TileMerge(Type, InfectedStoneType);

            MinPick = 65;
            MineResist = 2;
            HitSound = SoundID.Grass;

            switch(infectionType)
            {
                case InfectionType.Corrupt:
                    TileID.Sets.Corrupt[Type] = true;
                    TileID.Sets.AddCorruptionTile(Type);
                    TileID.Sets.CorruptBiomeSight[Type] = true;
                    TileID.Sets.CorruptCountCollection.Add(Type);
                    RegisterItemDrop(ItemID.EbonstoneBlock);
                    break;
                case InfectionType.Crimson:
                    TileID.Sets.Crimson[Type] = true;
                    TileID.Sets.AddCrimsonTile(Type);
                    TileID.Sets.CrimsonBiomeSight[Type] = true;
                    TileID.Sets.CrimsonCountCollection.Add(Type);
                    RegisterItemDrop(ItemID.CrimstoneBlock);
                    break;
                case InfectionType.Hallowed:
                    Main.tileShine[Type] = 9000;
                    TileID.Sets.Hallow[Type] = true;
                    TileID.Sets.HallowBiome[Type] = 1;
                    TileID.Sets.HallowBiomeSight[Type] = true;
                    TileID.Sets.HallowCountCollection.Add(Type);
                    RegisterItemDrop(ItemID.PearlstoneBlock);
                    break;
            }

            switch (mossType)
            {
                case MossType.Green:
                    MossColor = new (49, 134, 114);
                    AddMapEntry(MossColor);
                    DustType = DustID.GreenMoss;
                    break;
                case MossType.Brown:
                    MossColor = new (126, 134, 49);
                    AddMapEntry(MossColor);
                    DustType = DustID.BrownMoss;
                    break;
                case MossType.Red:
                    MossColor = new (134, 59, 49);
                    AddMapEntry(MossColor);
                    DustType = DustID.RedMoss;
                    break;
                case MossType.Blue:
                    MossColor = new (43, 86, 140);
                    AddMapEntry(MossColor);
                    DustType = DustID.BlueMoss;
                    break;
                case MossType.Purple:
                    MossColor = new (121, 49, 134);
                    AddMapEntry(MossColor);
                    DustType = DustID.PurpleMoss;
                    break;
                case MossType.Lava:
                    MossColor = new(150, 100, 50, 0);
                    MossTexture = TextureAssets.GlowMask[GlowMaskID.LavaMoss];
                    AddMapEntry(new(254, 121, 2));
                    DustType = DustID.LavaMoss;
                    break;
                case MossType.Krypton:
                    MossColor = new (0, 200, 0, 0);
                    MossTexture = TextureAssets.GlowMask[GlowMaskID.KryptonMoss];
                    AddMapEntry(new(114, 254, 2));
                    DustType = DustID.KryptonMoss;
                    break;
                case MossType.Xenon:
                    MossColor = new (0, 180, 250, 0);
                    MossTexture = TextureAssets.GlowMask[GlowMaskID.XenonMoss];
                    AddMapEntry(new(0, 197, 208));
                    DustType = DustID.XenonMoss;
                    break;
                case MossType.Argon:
                    MossColor = new (225, 0, 125, 0);
                    MossTexture = TextureAssets.GlowMask[GlowMaskID.ArgonMoss];
                    AddMapEntry(new(208, 0, 126));
                    DustType = DustID.ArgonMoss;
                    break;
                case MossType.Neon:
                    MossColor = new (150, 0, 250, 0);
                    MossTexture = TextureAssets.GlowMask[GlowMaskID.VioletMoss];
                    AddMapEntry(new(220, 12, 237));
                    DustType = DustID.VioletMoss;
                    break;
                case MossType.Helium:
                    MossColor = Main.DiscoColor;
                    AddMapEntry(new(255, 76, 76));
                    AddMapEntry(new(255, 195, 76));
                    AddMapEntry(new(195, 255, 76));
                    AddMapEntry(new(76, 255, 76));
                    AddMapEntry(new(76, 255, 195));
                    AddMapEntry(new(76, 195, 255));
                    AddMapEntry(new(77, 76, 255));
                    AddMapEntry(new(196, 76, 255));
                    AddMapEntry(new(255, 76, 195));
                    break;
            }
        }

        public override ushort GetMapOption(int i, int j)
        {
            if (mossType == MossType.Helium)
            {
                return (ushort)((i + j) % 9);
            }
            return 0;
        }

        public override void RandomUpdate(int i, int j)
        {
            InfectedQualitiesUtilities.DefaultInfectionSpread(i, j, infectionType, InfectedQualitiesUtilities.GetMossType(null, mossType));
            if (WorldGen.genRand.NextDouble() < 0.5)
            {
                for (int x = i - 1; x < i + 2; x++)
                {
                    for (int y = j - 1; y < j + 2; y++)
                    {
                        if ((i != x || j != y) && Main.tile[x, y].HasTile)
                        {
                            if(Main.tile[x, y].TileType == TileID.Ebonstone)
                            {
                                WorldGen.SpreadGrass(x, y, Main.tile[x, y].TileType, InfectedQualitiesUtilities.GetMossType(InfectionType.Corrupt, mossType), repeat: false, Main.tile[i, j].BlockColorAndCoating());
                                WorldGen.SquareTileFrame(x, y);
                                NetMessage.SendTileSquare(-1, i, j, 3);
                            }
                            else if (Main.tile[x, y].TileType == TileID.Crimstone)
                            {
                                WorldGen.SpreadGrass(x, y, Main.tile[x, y].TileType, InfectedQualitiesUtilities.GetMossType(InfectionType.Crimson, mossType), repeat: false, Main.tile[i, j].BlockColorAndCoating());
                                WorldGen.SquareTileFrame(x, y);
                                NetMessage.SendTileSquare(-1, i, j, 3);
                            }
                            else if (Main.tile[x, y].TileType == TileID.Pearlstone)
                            {
                                WorldGen.SpreadGrass(x, y, Main.tile[x, y].TileType, InfectedQualitiesUtilities.GetMossType(InfectionType.Hallowed, mossType), repeat: false, Main.tile[i, j].BlockColorAndCoating());
                                WorldGen.SquareTileFrame(x, y);
                                NetMessage.SendTileSquare(-1, i, j, 3);
                            }
                            else if (Main.tile[x, y].TileType == TileID.Stone)
                            {
                                WorldGen.SpreadGrass(x, y, Main.tile[x, y].TileType, InfectedQualitiesUtilities.GetMossType(null, mossType), repeat: false, Main.tile[i, j].BlockColorAndCoating());
                                WorldGen.SquareTileFrame(x, y);
                                NetMessage.SendTileSquare(-1, i, j, 3);
                            }
                        }
                    }
                }

                if (WorldGen.genRand.NextBool(6))
                {
                    int x = i;
                    int y = j;
                    switch (WorldGen.genRand.Next(4))
                    {
                        case 0:
                            x--;
                            break;
                        case 1:
                            x++;
                            break;
                        case 2:
                            y--;
                            break;
                        default:
                            y++;
                            break;
                    }

                    if (!Main.tile[x, y].HasTile && WorldGen.SolidTile(i, j))
                    {
                        Main.tile[x, y].Get<TileWallWireStateData>().HasTile = true;
                        Main.tile[x, y].TileType = TileID.LongMoss;
                        Main.tile[x, y].TileFrameX = (short)((int)mossType * 18);
                        Main.tile[x, y].TileFrameY = (short)(WorldGen.genRand.Next(3) * 18);
                        WorldGen.SquareTileFrame(x, y);
                        NetMessage.SendTileSquare(-1, x, y);
                    }
                }
            }
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            if(mossType == MossType.Helium)
            {
                Dust dust = Main.dust[Dust.NewDust(new Vector2(i * 16, j * 16), 16, 16, DustID.RainbowMk2, 0f, 0f, 0, Main.DiscoColor)];
                dust.noGravity = true;
                dust.noLightEmittence = true;
                return false;
            }
            return true;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            if (!Main.tile[i, j].IsTileInvisible)
            {
                if (Main.tile[i, j].TileFrameY == 18 && Main.tile[i, j].TileFrameX is 18 or 36 or 54)
                {
                    if (WorldGen.TileType(i, j - 1) == Type || WorldGen.TileType(i, j + 1) == Type || WorldGen.TileType(i - 1, j) == Type || WorldGen.TileType(i + 1, j) == Type)
                    {
                        return;
                    }
                }

                Color mossColor;
                if (mossType < MossType.Lava) mossColor = Lighting.GetColorClamped(i, j, MossColor);
                else mossColor = MossColor;

                Vector2 offscreenVector = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
                Vector2 drawVector = new Vector2(i * 16, j * 16) + offscreenVector - Main.screenPosition;
                Rectangle frame = new(Main.tile[i, j].TileFrameX, Main.tile[i, j].TileFrameY, 16, 16);

                if (Main.tile[i, j].Slope == SlopeType.Solid && !Main.tile[i, j].IsHalfBlock)
                {
                    spriteBatch.Draw(MossTexture.Value, drawVector, frame, mossColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                }
                else if(Main.tile[i, j].IsHalfBlock)
                {
                    drawVector += new Vector2(0, 8);
                    frame.Height = 8;
                    spriteBatch.Draw(MossTexture.Value, drawVector, frame, mossColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
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
                        spriteBatch.Draw(MossTexture.Value, drawVector + new Vector2(xOffset, q * width + num), frame, mossColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    }

                    int slopeOffset = (Main.tile[i, j].Slope <= SlopeType.SlopeDownRight) ? 14 : 0;
                    frame = new(Main.tile[i, j].TileFrameX, Main.tile[i, j].TileFrameY + slopeOffset, 16, 2);
                    spriteBatch.Draw(MossTexture.Value, drawVector + new Vector2(0, slopeOffset), frame, mossColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                }
            }
        }

        public override void ModifyFrameMerge(int i, int j, ref int up, ref int down, ref int left, ref int right, ref int upLeft, ref int upRight, ref int downLeft, ref int downRight)
        {
            if (down == TileID.Stalactite && Main.tile[i, j + 1].TileFrameY is 0 or 72)
            {
                down = Type;
            }
            else if (up == TileID.Stalactite && Main.tile[i, j - 1].TileFrameY is 54 or 90)
            {
                up = Type;
            }
        }

        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : mossType == MossType.Helium ? 5 : 3;

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if(fail && !effectOnly)
            {
                noItem = true;
                Main.tile[i, j].TileType = InfectedStoneType;
                WorldGen.SquareTileFrame(i, j);
                NetMessage.SendTileSquare(-1, i, j);
            }
        }
        public override string Name => infectionType.ToString() + mossType.ToString() + "Moss";

        public override string Texture => $"Terraria/Images/Tiles_{InfectedStoneType}";

        public override bool IsLoadingEnabled(Mod mod) => ModContent.GetInstance<InfectedQualitiesServerConfig>().InfectedMosses;
    }
}
