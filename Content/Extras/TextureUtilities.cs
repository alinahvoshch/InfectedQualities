using InfectedQualities.Common;
using InfectedQualities.Content.Extras.Tiles;
using InfectedQualities.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.GameContent.TilePaintSystemV2;

namespace InfectedQualities.Content.Extras
{
    public static class TextureUtilities
    {
        internal static Asset<Texture2D> PylonCrystalHighlightTexture { get; set; } = null;
        private static readonly Dictionary<TexturePaintkey, TileRenderTargetHolder> textureRenders = [];

        public static void ReplacePlanteraType(InfectionType infectionType)
        {
            TextureAssets.Npc[NPCID.Plantera] = infectionType switch
            {
                InfectionType.Corrupt => InfectedQualitiesGlobalNPC.CorruptPlantera,
                InfectionType.Crimson => InfectedQualitiesGlobalNPC.CrimsonPlantera,
                InfectionType.Hallowed => InfectedQualitiesGlobalNPC.HallowedPlantera,
                _ => TextureAssets.Npc[NPCID.Plantera]
            };

            string planteraType = infectionType.ToString();
            TextureAssets.Npc[NPCID.PlanterasHook] = ModContent.Request<Texture2D>("InfectedQualities/Content/NPCs/" + planteraType + "Plantera_Hook");
            TextureAssets.Chain26 = ModContent.Request<Texture2D>("InfectedQualities/Content/Extras/" + planteraType + "Plantera_Hook_Vine");
            TextureAssets.Npc[NPCID.PlanterasTentacle] = ModContent.Request<Texture2D>("InfectedQualities/Content/NPCs/" + planteraType + "Plantera_Tentacle");
            TextureAssets.Chain27 = ModContent.Request<Texture2D>("InfectedQualities/Content/Extras/" + planteraType + "Plantera_Tentacle_Vine");
            TextureAssets.Npc[NPCID.Spore] = ModContent.Request<Texture2D>("InfectedQualities/Content/NPCs/" + planteraType + "Plantera_Spore");
            TextureAssets.Projectile[ProjectileID.SeedPlantera] = ModContent.Request<Texture2D>("InfectedQualities/Content/Projectiles/" + planteraType + "Plantera_Seed");
        }

        public static InfectionType? GetPlanteraType()
        {
            if (TextureAssets.Npc[NPCID.Plantera] == InfectedQualitiesGlobalNPC.CorruptPlantera)
            {
                return InfectionType.Corrupt;
            }
            else if (TextureAssets.Npc[NPCID.Plantera] == InfectedQualitiesGlobalNPC.CrimsonPlantera)
            {
                return InfectionType.Crimson;
            }
            else if (TextureAssets.Npc[NPCID.Plantera] == InfectedQualitiesGlobalNPC.HallowedPlantera)
            {
                return InfectionType.Hallowed;
            }
            return null;
        }

        public static RenderTarget2D RequestPaintTexture(string name, int color)
        {
            TexturePaintkey tileVariationkey = new()
            {
                TextureName = name,
                PaintColor = color
            };
            textureRenders.TryGetValue(tileVariationkey, out var value);

            if (value != default && value.IsReady)
            {
                return value.Target;
            }

            if (value == default)
            {
                value = new TileRenderTargetHolder
                {
                    Key = tileVariationkey
                };

                textureRenders.Add(tileVariationkey, value);
            }

            if (!value.IsReady)
            {
                List<ARenderTargetHolder> renderRequests = (List<ARenderTargetHolder>)typeof(TilePaintSystemV2).GetField("_requests", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public | BindingFlags.Instance).GetValue(Main.instance.TilePaintSystem);
                renderRequests.Add(value);
            }
            return null;
        }

        public static Color TileDrawColor(int i, int j, bool emitDust = false, Color baseColor = default)
        {
            if (baseColor == default)
            {
                baseColor = Color.White;
            }

            Color color;
            if (Main.tile[i, j].IsTileFullbright) color = baseColor;
            else
            {
                if(baseColor == Color.White)
                {
                    color = Lighting.GetColor(i, j);
                }
                else
                {
                    color = Lighting.GetColor(i, j, baseColor);
                }
            }
            ActuatedColor(i, j, ref color);

            if (Main.LocalPlayer.dangerSense && TileDrawing.IsTileDangerous(i, j, Main.LocalPlayer))
            {
                color.R = byte.MaxValue;
                color.G = Math.Max((byte)50, color.G);
                color.B = Math.Max((byte)50, color.B);

                if(emitDust && Main.rand.NextBool(30))
                {
                    Dust dust = Main.dust[Dust.NewDust(new Vector2(i * 16, j * 16), 16, 16, DustID.RedTorch, 0f, 0f, 100, default, 0.3f)];
                    dust.fadeIn = 1;
                    dust.velocity *= 0.1f;
                    dust.noLight = true;
                    dust.noGravity = true;
                }
            }

            if(Main.LocalPlayer.findTreasure && Main.IsTileSpelunkable(i, j))
            {
                color.R = Math.Max((byte)200, color.R);
                color.G = Math.Max((byte)170, color.G);

                if(emitDust && Main.rand.NextBool(60))
                {
                    Dust dust = Main.dust[Dust.NewDust(new Vector2(i * 16, j * 16), 16, 16, DustID.TreasureSparkle, 0f, 0f, 150, default, 0.3f)];
                    dust.fadeIn = 1;
                    dust.velocity *= 0.1f;
                    dust.noLight = true;
                }
            }

            if(Main.LocalPlayer.biomeSight)
            {
                Color sightColor = default;
                if(Main.IsTileBiomeSightable(i, j, ref sightColor))
                {
                    color.R = Math.Max(sightColor.R, color.R);
                    color.G = Math.Max(sightColor.G, color.G);
                    color.B = Math.Max(sightColor.B, color.B);

                    if(emitDust && Main.rand.NextBool(480))
                    {
                        Dust dust = Main.dust[Dust.NewDust(new Vector2(i * 16, j * 16), 16, 16, DustID.RainbowMk2, 0f, 0f, 150, sightColor, 0.3f)];
                        dust.noGravity = true;
                        dust.fadeIn = 1f;
                        dust.velocity *= 0.1f;
                        dust.noLightEmittence = true;
                    }
                }
            }

            return color;
        }

        public static void TileDraw(int i, int j, Texture2D texture, Color color, SpriteBatch spriteBatch, Point? frames = null)
        {
            if (texture == null) return;

            Vector2 offscreenVector = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
            Vector2 drawVector = new Vector2(i * 16, j * 16) + offscreenVector - Main.screenPosition;
            if (!frames.HasValue)
            {
                frames = new (Main.tile[i, j].TileFrameX, Main.tile[i, j].TileFrameY);
            }

            Rectangle frame = new(frames.Value.X, frames.Value.Y, 16, 16);
            if (Main.tile[i, j].Slope == SlopeType.Solid && !Main.tile[i, j].IsHalfBlock)
            {
                spriteBatch.Draw(texture, drawVector, frame, color, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            }
            else if (Main.tile[i, j].IsHalfBlock)
            {
                drawVector += new Vector2(0, 8);
                frame.Height = 8;
                spriteBatch.Draw(texture, drawVector, frame, color, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
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
                    frame = new(frames.Value.X + xOffset, frames.Value.Y + yOffset, width, height);
                    spriteBatch.Draw(texture, drawVector + new Vector2(xOffset, q * width + num), frame, color, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                }

                int slopeOffset = (Main.tile[i, j].Slope <= SlopeType.SlopeDownRight) ? 14 : 0;
                frame = new(frames.Value.X, frames.Value.Y + slopeOffset, 16, 2);
                spriteBatch.Draw(texture, drawVector + new Vector2(0, slopeOffset), frame, color, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            }
        }

        //Basically Tile.actColor
        public static void ActuatedColor(int i, int j, ref Color color)
        {
            if (Main.tile[i, j].IsActuated)
            {
                byte alpha = color.A;
                color *= 0.4f;
                color.A = alpha;
            }
        }

        public static Color WallBiomeColor(int i, int j, int type)
        {
            if (Main.LocalPlayer.biomeSight && ModContent.GetInstance<InfectedQualitiesClientConfig>().BiomeSightWallHighlightBrightness != 0 && !WorldGen.SolidTile(i, j, true) && (Main.tile[i, j].LiquidAmount == 0 || Main.tile[i, j].LiquidType == LiquidID.Water))
            {
                return InfectedQualitiesModSupport.ModWallBiomeSight[type];
            }
            return default;
        }

        private class TileRenderTargetHolder : ARenderTargetHolder
        {
            public TexturePaintkey Key;

            public override void Prepare()
            {
                Asset<Texture2D> asset = ModContent.Request<Texture2D>(Key.TextureName, AssetRequestMode.ImmediateLoad);
                asset.Wait?.Invoke();
                PrepareTextureIfNecessary(asset.Value);
            }

            public override void PrepareShader()
            {
                PrepareShader(Key.PaintColor, TreePaintSystemData.GetTileSettings(-1, 0));
            }
        }

        private struct TexturePaintkey
        {
            public string TextureName;
            public int PaintColor;

            public override readonly bool Equals(object obj)
            {
                if (obj is TexturePaintkey variationkey)
                {
                    return TextureName == variationkey.TextureName && PaintColor == variationkey.PaintColor;
                }
                return false;
            }

            public override readonly int GetHashCode()
            {
                int stringHash = TextureName.GetHashCode();
                return ((stringHash << 16) | (stringHash >> 16)) ^ PaintColor;
            }

            public static bool operator ==(TexturePaintkey left, TexturePaintkey right) => left.Equals(right);

            public static bool operator !=(TexturePaintkey left, TexturePaintkey right) => !left.Equals(right);
        }
    }
}
