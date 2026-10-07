using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace Sigf.Content;

/// <summary>Harmless flying board a knocked-out skater leaves behind.</summary>
public class BailBoard : ModProjectile
{
    public override void SetDefaults()
    {
        Projectile.width = 26; Projectile.height = 14; Projectile.timeLeft = 70;
        Projectile.friendly = false; Projectile.hostile = false; Projectile.tileCollide = false;
    }
    public override void AI()
    {
        Projectile.rotation += 0.5f;
        Projectile.velocity.Y += 0.35f;
        Projectile.alpha = (int)(255 * (1 - Projectile.timeLeft / 70f) * 0.6f);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
        Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Color.White * (1f - Projectile.alpha / 255f), Projectile.rotation, tex.Size() / 2, 1.5f, SpriteEffects.None, 0);
        return false;
    }
}
