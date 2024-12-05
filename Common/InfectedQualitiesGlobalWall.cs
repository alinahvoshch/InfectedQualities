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
                InfectedQualitiesUtilities.WallSpread(i, j, InfectionType.Corrupt);
            }
            else if (WallID.Sets.Crimson[type] && !(Main.tile[i, j].HasTile && TileID.Sets.Crimson[Main.tile[i, j].TileType]))
            {
                InfectedQualitiesUtilities.WallSpread(i, j, InfectionType.Crimson);
            }
            else if (WallID.Sets.Hallow[type] && !(Main.tile[i, j].HasTile && TileID.Sets.Hallow[Main.tile[i, j].TileType]))
            {
                InfectedQualitiesUtilities.WallSpread(i, j, InfectionType.Hallowed);
            }
        }

        public override void ModifyLight(int i, int j, int type, ref float r, ref float g, ref float b)
        {
            if (InfectedQualitiesUtilities.GetBiomeSightColor(i, j, type, out Color sightColor))
            {
                sightColor *= 0.5f;
                r = sightColor.R / 255f;
                g = sightColor.G / 255f;
                b = sightColor.B / 255f;
            }
        }

        public override void PostDraw(int i, int j, int type, SpriteBatch spriteBatch)
        {
            if (Main.rand.NextBool(480) && InfectedQualitiesUtilities.GetBiomeSightColor(i, j, type, out Color sightColor))
            {
                sightColor *= 0.75f;
                Dust dust = Main.dust[Dust.NewDust(new Vector2(i * 16, j * 16), 16, 16, DustID.RainbowMk2, 0f, 0f, 150, sightColor, 0.3f)];
                dust.noGravity = true;
                dust.fadeIn = 1f;
                dust.velocity *= 0.1f;
                dust.noLightEmittence = true;
            }
        }
    }
}
