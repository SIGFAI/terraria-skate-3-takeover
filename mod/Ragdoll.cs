using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Sigf.Content;

/// <summary>The bailed body: a copy of the knocked-out enemy that cartwheels through the air and bounces. ai[0] 0 = punk, 1 = cop.</summary>
public class Ragdoll : ModProjectile
{
    public override void SetDefaults()
    {
        Projectile.width = 30; Projectile.height = 30; Projectile.timeLeft = 110;
        Projectile.friendly = false; Projectile.hostile = false;
    }

    public override void AI()
    {
        Projectile.rotation += 0.3f * (Projectile.velocity.X >= 0 ? 1 : -1);
        Projectile.velocity.Y += 0.35f;
        if (Projectile.timeLeft % 4 == 0) Dust.NewDustPerfect(Projectile.Center, DustID.Smoke, Vector2.Zero, 100, default, 1.2f).noGravity = true;
        if (Projectile.timeLeft % 8 == 0) Dust.NewDustPerfect(Projectile.Center, DustID.YellowStarDust, Main.rand.NextVector2Circular(2, 2), 0, default, 1.1f);
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        if (Projectile.velocity.Y != oldVelocity.Y) Projectile.velocity.Y = -oldVelocity.Y * 0.55f;
        Projectile.velocity.X *= 0.9f;
        if (System.Math.Abs(oldVelocity.Y) > 3) { Mix.Burst(Projectile.Bottom, DustID.Dirt, 6); Mix.Sound(SoundID.NPCHit1, Projectile.Center); }
        return false;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D tex = ModContent.Request<Texture2D>(Projectile.ai[0] == 1 ? ModContent.GetInstance<MallCop>().Texture : ModContent.GetInstance<SkatePunk>().Texture).Value;
        Rectangle src = new Rectangle(0, 0, tex.Width, tex.Height / 2);
        float alpha = Projectile.timeLeft < 25 ? Projectile.timeLeft / 25f : 1f;
        Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, src, lightColor * alpha, Projectile.rotation, src.Size() / 2, 1f, SpriteEffects.None, 0);
        return false;
    }
}
