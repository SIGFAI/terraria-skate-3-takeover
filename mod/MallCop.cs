using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Sigf.Content;

/// <summary>Angry security guard who chases the skaters off the plaza. Slower, tougher.</summary>
public class MallCop : ModNPC
{
    public override void SetStaticDefaults() => Main.npcFrameCount[Type] = 2;

    public override void SetDefaults()
    {
        NPC.width = 36; NPC.height = 60;
        NPC.lifeMax = 140; NPC.damage = 24; NPC.defense = 6; NPC.knockBackResist = 0.4f; NPC.value = 250;
        NPC.aiStyle = NPCAIStyleID.Fighter; AIType = NPCID.Zombie;
        NPC.HitSound = SoundID.NPCHit1; NPC.DeathSound = Mix.Style("bail");
    }

    public override void AI()
    {
        NPC.TargetClosest();
        float dir = Main.player[NPC.target].Center.X > NPC.Center.X ? 1 : -1;
        if (NPC.velocity.Y == 0) NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, dir * 2.6f, 0.1f);
        // flashlight glow
        Mix.Light(NPC.Center + new Vector2(dir * 18, 0), Color.LightYellow);
    }

    public override void FindFrame(int frameHeight)
    {
        NPC.frameCounter++;
        NPC.frame.Y = (int)(NPC.frameCounter / 9 % 2) * frameHeight;
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        Mix.Burst(NPC.Center, DustID.Blood, 8);
        Mix.Burst(NPC.Center, DustID.Electric, 4);
        if (NPC.life <= 0)
        {
            Mix.Burst(NPC.Center, DustID.Smoke, 25);
            var r = Mix.Shoot<Ragdoll>(NPC.Center, new Vector2(-NPC.direction * 5f, -12f), 0);
            if (r != null) r.ai[0] = 1;
        }
    }

    public override void OnKill()
    {
        Skate.HallOfMeat(NPC.Top, "HALL OF MEAT!", 600);
        Mix.Drop(ItemID.GoldCoin, NPC.Center, 1);
        Mix.Drop(ItemID.SilverCoin, NPC.Center, 20);
    }
}
