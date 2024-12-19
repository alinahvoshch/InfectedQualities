using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using InfectedQualities.Core;

namespace InfectedQualities.Common
{
    public class InfectedQualitiesGlobalProjectile : GlobalProjectile
    {
        public override void AI(Projectile projectile)
        {
            int left = (int)(projectile.position.X / 16f) - 1;
            int right = (int)((projectile.position.X + projectile.width) / 16f) + 2;
            int top = (int)(projectile.position.Y / 16f) - 1;
            int bottom = (int)((projectile.position.Y + projectile.height) / 16f) + 2;

            int conversionID = projectile.type switch
            {
                ProjectileID.PurificationPowder => BiomeConversionID.Purity,
                ProjectileID.VilePowder => BiomeConversionID.Corruption,
                ProjectileID.ViciousPowder => BiomeConversionID.Crimson,
                _ => -1
            };

            for (int j = left; j < right; j++)
            {
                for (int k = top; k < bottom; k++)
                {
                    if (projectile.position.X + projectile.width > j * 16 && projectile.position.X < j * 16 && projectile.position.Y + projectile.height > k * 16 && projectile.position.Y < k * 16 + 16f)
                    {
                        if (!TileID.Sets.Conversion.MushroomGrass[Main.tile[j, k].TileType] && Main.tile[j, k].WallType != WallID.MushroomUnsafe)
                        {
                            if (conversionID == BiomeConversionID.Purity && TileID.Sets.Hallow[Main.tile[j, k].TileType] && !ModContent.GetInstance<InfectedQualitiesServerConfig>().DivinePowder)
                            {
                                continue;
                            }
                            WorldGen.Convert(j, k, conversionID, 1);
                        }
                    }
                }
            }
        }

        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation) => entity.type is ProjectileID.PurificationPowder or ProjectileID.VilePowder or ProjectileID.ViciousPowder;
    }
}
