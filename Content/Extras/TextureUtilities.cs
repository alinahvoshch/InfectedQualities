using InfectedQualities.Common;
using InfectedQualities.Content.Extras.Tiles;
using InfectedQualities.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace InfectedQualities.Content.Extras
{
    public static class TextureUtilities
    {
        internal static Asset<Texture2D> PylonCrystalHighlightTexture { get; set; } = null;

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
            if (ModContent.GetInstance<InfectedQualitiesClientConfig>().InfectedPlantera)
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
            }
            return null;
        }

        public static Color GetGlowColor(int i, int j)
        {
            Color color = Lighting.GetColor(i, j);
            if(TileDrawing.IsTileDangerous(i, j, Main.LocalPlayer))
            {
                color.R = byte.MaxValue;
                color.G = Math.Max((byte)50, color.G);
                color.B = Math.Max((byte)50, color.B);
            }

            if(Main.LocalPlayer.findTreasure && Main.IsTileSpelunkable(i, j))
            {
                color.R = Math.Max((byte)200, color.R);
                color.G = Math.Max((byte)170, color.G);
            }

            if(Main.LocalPlayer.biomeSight)
            {
                Color sightColor = default;
                if(Main.IsTileBiomeSightable(i, j, ref sightColor))
                {
                    color.R = Math.Max(sightColor.R, color.R);
                    color.G = Math.Max(sightColor.G, color.G);
                    color.B = Math.Max(sightColor.B, color.B);
                }
            }

            return color;
        }

        public static void TileDraw(int i, int j, Asset<Texture2D> texture, Color color, SpriteBatch spriteBatch, Point? frames = null)
        {
            Vector2 offscreenVector = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
            Vector2 drawVector = new Vector2(i * 16, j * 16) + offscreenVector - Main.screenPosition;
            if (!frames.HasValue)
            {
                frames = new (Main.tile[i, j].TileFrameX, Main.tile[i, j].TileFrameY);
            }

            Rectangle frame = new(frames.Value.X, frames.Value.Y, 16, 16);
            if (Main.tile[i, j].Slope == SlopeType.Solid && !Main.tile[i, j].IsHalfBlock)
            {
                spriteBatch.Draw(texture.Value, drawVector, frame, color, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            }
            else if (Main.tile[i, j].IsHalfBlock)
            {
                drawVector += new Vector2(0, 8);
                frame.Height = 8;
                spriteBatch.Draw(texture.Value, drawVector, frame, color, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
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
                    spriteBatch.Draw(texture.Value, drawVector + new Vector2(xOffset, q * width + num), frame, color, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                }

                int slopeOffset = (Main.tile[i, j].Slope <= SlopeType.SlopeDownRight) ? 14 : 0;
                frame = new(frames.Value.X, frames.Value.Y + slopeOffset, 16, 2);
                spriteBatch.Draw(texture.Value, drawVector + new Vector2(0, slopeOffset), frame, color, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            }
        }
    }
}
