using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Sigf.Content;

/// <summary>Everyone rides a skateboard: faster, slippery, airtime tricks and rail grinds.</summary>
public class SkaterPlayer : ModPlayer
{
    public int AirTime;
    public int GrindTick;
    static readonly string[] Tricks = { "KICKFLIP", "HEELFLIP", "360 FLIP", "MCTWIST", "POP SHOVE-IT", "NOLLIE" };

    public override void PostUpdateRunSpeeds()
    {
        Player.maxRunSpeed = 8.5f;
        Player.accRunSpeed = 8.5f;
        Player.runAcceleration = 0.3f;
        Player.runSlowdown = 0.25f;
    }

    bool OnRail()
    {
        int x = (int)(Player.Center.X / 16), y = (int)((Player.Bottom.Y + 2) / 16);
        Tile t = Framing.GetTileSafely(x, y);
        return t.HasTile && t.TileType == TileID.Platforms;
    }

    public override void PostUpdate()
    {
        Player.gfxOffY = -6;
        Player.fullRotationOrigin = Player.Size / 2;
        Player.fullRotation = AirTime > 10 ? Player.direction * (AirTime - 10) * 0.28f : 0f;
        if (Math.Abs(Player.velocity.X) > 3f)
        {
            Dust d = Dust.NewDustPerfect(Player.Bottom + new Vector2(-Player.direction * 24, -4), DustID.RainbowMk2, new Vector2(-Player.direction * 1.5f, 0), 0, Main.hslToRgb((Main.GameUpdateCount % 60) / 60f, 1f, 0.6f), 1.5f);
            d.noGravity = true;
        }
        bool air = Player.velocity.Y != 0;
        if (air)
        {
            AirTime++;
            if (AirTime == 1) { Mix.Sound("ollie", Player.Center, 0.8f); Mix.Burst(Player.Bottom, DustID.Smoke, 8); }
        }
        else
        {
            Player.fullRotation = 0f;
            if (AirTime > 30) Land();
            AirTime = 0;
        }
        if (!air && Math.Abs(Player.velocity.X) > 3f && OnRail())
        {
            Dust.NewDustPerfect(Player.Bottom + new Vector2(-Player.direction * 8, 0), DustID.Torch, new Vector2(-Player.direction * 2, -2.5f), 0, default, 1.2f).noGravity = true;
            Dust.NewDustPerfect(Player.Bottom, DustID.Smoke, Vector2.Zero, 100, default, 0.8f);
            if (++GrindTick % 30 == 1) { Mix.Sound("grind", Player.Center, 0.7f); }
            if (GrindTick % 45 == 20) Skate.Trick(Player.Top, "50-50 GRIND", 100);
        }
        else GrindTick = 0;
    }

    void Land()
    {
        Skate.Trick(Player.Top, Tricks[Main.rand.Next(Tricks.Length)], 150 + AirTime * 2);
        Mix.Burst(Player.Bottom, DustID.Smoke, 10);
        Mix.Sound("ollie", Player.Bottom, 1f, -0.2f);
    }
}

public class BoardLayer : PlayerDrawLayer
{
    public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.Shoes);

    protected override void Draw(ref PlayerDrawSet drawInfo)
    {
        Player p = drawInfo.drawPlayer;
        if (p.dead || p.ghost) return;
        var sp = p.GetModPlayer<SkaterPlayer>();
        Texture2D tex = ModContent.Request<Texture2D>(ModContent.GetInstance<DeckShot>().Texture).Value;
        float t = sp.AirTime;
        float scaleY = t > 4 ? 1.5f * Math.Abs((float)Math.Cos(t * 0.25f)) + 0.1f : 1.4f;
        float lift = t > 0 ? -Math.Min(t, 10) * 0.4f : 0f;
        Vector2 pos = p.Bottom + new Vector2(0, -3 + lift) + new Vector2(0, p.gfxOffY) - Main.screenPosition + new Vector2(0, 6);
        pos = new Vector2((int)pos.X, (int)pos.Y);
        float rot = t > 0 ? p.direction * p.velocity.Y * -0.02f : 0f;
        var fx = p.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
        Color c = Lighting.GetColor(p.Center.ToTileCoordinates());
        drawInfo.DrawDataCache.Add(new DrawData(tex, pos, null, c, rot, tex.Size() / 2, new Vector2(1.4f, scaleY * 1.3f / 1.4f * 1.0f), fx, 0));
    }
}
