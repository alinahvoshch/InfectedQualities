using InfectedQualities.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace InfectedQualities.Common
{
    public class InfectedQualitiesGlobalWall : GlobalWall
    {
        public override void SetStaticDefaults()
        {
            for(int i = 0; i < 4; i++)
            {
                WallID.Sets.Corrupt[WallID.CorruptionUnsafe1 + i] = true;
                WallID.Sets.Crimson[WallID.CrimsonUnsafe1 + i] = true;
                WallID.Sets.Hallow[WallID.HallowUnsafe1 + i] = true;
            }
        }

        public override void RandomUpdate(int i, int j, int type)
        {
            if (WallID.Sets.Corrupt[type] && !(Main.tile[i, j].HasTile && TileID.Sets.Corrupt[Main.tile[i, j].TileType]))
            {
                WallSpread(i, j, InfectionType.Corruption);
            }
            else if (WallID.Sets.Crimson[type] && !(Main.tile[i, j].HasTile && TileID.Sets.Crimson[Main.tile[i, j].TileType]))
            {
                WallSpread(i, j, InfectionType.Crimson);
            }
            else if (WallID.Sets.Hallow[type] && !(Main.tile[i, j].HasTile && TileID.Sets.Hallow[Main.tile[i, j].TileType]))
            {
                WallSpread(i, j, InfectionType.Hallow);
            }
        }

        public override void ModifyLight(int i, int j, int type, ref float r, ref float g, ref float b)
        {
            if (GetBiomeSightColor(i, j, type, out Color sightColor))
            {
                sightColor *= 0.5f;
                r = sightColor.R / 255f;
                g = sightColor.G / 255f;
                b = sightColor.B / 255f;
            }
        }

        public override void PostDraw(int i, int j, int type, SpriteBatch spriteBatch)
        {
            if (Main.rand.NextBool(480) && GetBiomeSightColor(i, j, type, out Color sightColor))
            {
                sightColor *= 0.75f;
                Dust dust = Main.dust[Dust.NewDust(new Vector2(i * 16, j * 16), 16, 16, DustID.RainbowMk2, 0f, 0f, 150, sightColor, 0.3f)];
                dust.noGravity = true;
                dust.fadeIn = 1f;
                dust.velocity *= 0.1f;
                dust.noLightEmittence = true;
            }
        }

        private static bool GetBiomeSightColor(int i, int j, int type, out Color sightColor)
        {
            sightColor = InfectedQualitiesModSupport.ModWallBiomeSight[type];
            if (Main.LocalPlayer.biomeSight && !WorldGen.SolidTile(i, j, true) && (Main.tile[i, j].LiquidAmount == 0 || Main.tile[i, j].LiquidType == LiquidID.Water))
            {
                if(sightColor == default)
                {
                    if (WallID.Sets.Corrupt[type])
                    {
                        sightColor = new(200, 100, 240);
                        return true;
                    }
                    else if (WallID.Sets.Crimson[type])
                    {
                        sightColor = new(255, 100, 100);
                        return true;
                    }
                    else if (WallID.Sets.Hallow[type])
                    {
                        sightColor = new(255, 160, 240);
                        return true;
                    }
                    return false;
                }
                return true;
            }
            return false;
        }

        private static void WallSpread(int i, int j, InfectionType infectionType)
        {
            if (InfectedQualitiesUtilities.RefectionMethod(i, j, "nearbyChlorophyte") && WorldGen.AllowedToSpreadInfections)
            {
                InfectedQualitiesUtilities.ChlorophyteWallDefense(i, j);
            }
            else
            {
                InfectedQualitiesUtilities.SpreadInfection(i, j, infectionType);
            }
        }
    }
}
