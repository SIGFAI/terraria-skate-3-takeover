using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Sigf.Content;

/// <summary>A thrown skateboard: spins with a fiery afterimage trail, bounces off blocks with sparks, smacks enemies.</summary>
public class DeckShot : ModProjectile
{
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Type] = 8;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }

    public override void SetDefaults()
    {
        Projectile.width = 36; Projectile.height = 36; Projectile.friendly = true;
        Projectile.DamageType = DamageClass.Ranged; Projectile.penetrate = 5; Projectile.timeLeft = 120;
        Projectile.usesLocalNPCImmunity = true; Projectile.localNPCHitCooldown = 12;
    }

    public override void AI()
    {
        Projectile.rotation += 0.5f * (Projectile.velocity.X >= 0 ? 1 : -1);
        Projectile.velocity.Y += 0.2f;
        Dust d = Dust.NewDustPerfect(Projectile.Center, Main.rand.NextBool() ? DustID.Torch : DustID.YellowTorch, Main.rand.NextVector2Circular(1, 1), 0, default, 1.7f);
        d.noGravity = true;
        Mix.Light(Projectile.Center, Color.OrangeRed);
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        if (Projectile.velocity.X != oldVelocity.X) Projectile.velocity.X = -oldVelocity.X * 0.8f;
        if (Projectile.velocity.Y != oldVelocity.Y) Projectile.velocity.Y = -oldVelocity.Y * 0.8f;
        Mix.Burst(Projectile.Center, DustID.Torch, 20);
        Mix.Burst(Projectile.Center, DustID.YellowStarDust, 10);
        Mix.Sound("grind", Projectile.Center, 0.5f, 0.3f);
        Projectile.penetrate--;
        if (Projectile.penetrate <= 0) Projectile.Kill();
        return false;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        Mix.Burst(target.Center, DustID.Torch, 20);
        Mix.Burst(target.Center, DustID.YellowStarDust, 10);
        Skate.Trick(target.Top, "BOARD SMACK", 60, false);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
        for (int i = Projectile.oldPos.Length - 1; i >= 0; i--)
        {
            if (Projectile.oldPos[i] == Vector2.Zero) continue;
            float f = 1f - i / (float)Projectile.oldPos.Length;
            Main.EntitySpriteDraw(tex, Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition, null, Color.OrangeRed * (f * 0.5f), Projectile.oldRot[i], tex.Size() / 2, 1.5f * f + 0.3f, SpriteEffects.None, 0);
        }
        Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, tex.Size() / 2, 1.5f, SpriteEffects.None, 0);
        return false;
    }
}
