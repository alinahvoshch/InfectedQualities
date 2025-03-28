using System;
using Terraria;
using Terraria.ModLoader;

namespace InfectedQualities.Content.Dusts
{
    public class DivinePowderDust : ModDust
    {
        public override bool Update(Dust dust)
        {
            dust.scale += 0.005f;
            dust.velocity.Y *= 0.94f;
            dust.velocity.X *= 0.94f;

            float scale = Math.Min(dust.scale * 0.8f, 1);
            Lighting.AddLight((int)(dust.position.X / 16f), (int)(dust.position.Y / 16f), scale, scale, scale * 0.9f);
            return true;
        }
    }
}
