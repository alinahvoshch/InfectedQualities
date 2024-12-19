using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;
using InfectedQualities.Core;

namespace InfectedQualities.Content.Projectiles
{
    public class DivineDust : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.PurificationPowder);
            Projectile.aiStyle = 0;
        }

        public override void AI()
        {
            Projectile.velocity *= 0.95f;
            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] == 180f)
            {
                Projectile.Kill();
            }

            if (Projectile.ai[1] == 0f)
            {
                Projectile.ai[1] = 1f;
                for (int i = 0; i < 30; i++)
                {
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.PurificationPowder, Projectile.velocity.X, Projectile.velocity.Y, 50, Color.LightBlue);
                }
            }

            if (Main.myPlayer == Projectile.owner)
            {
                int left = (int)(Projectile.position.X / 16f) - 1;
                int right = (int)((Projectile.position.X + Projectile.width) / 16f) + 2;
                int top = (int)(Projectile.position.Y / 16f) - 1;
                int bottom = (int)((Projectile.position.Y + Projectile.height) / 16f) + 2;

                for (int j = left; j < right; j++)
                {
                    for (int k = top; k < bottom; k++)
                    {
                        if (Projectile.position.X + Projectile.width > j * 16 && Projectile.position.X < j * 16 && Projectile.position.Y + Projectile.height > k * 16 && Projectile.position.Y < k * 16)
                        {
                            WorldGen.Convert(j, k, BiomeConversionID.Hallow, 1);
                        }
                    }
                }
            }
        }

        public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.PurificationPowder}";

        public override bool IsLoadingEnabled(Mod mod) => ModContent.GetInstance<InfectedQualitiesServerConfig>().DivinePowder;
    }
}
