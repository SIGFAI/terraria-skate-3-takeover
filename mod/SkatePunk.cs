using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Sigf.Content;

/// <summary>A hoodie skater who rolls at you fast and pops ollies over obstacles.</summary>
public class SkatePunk : ModNPC
{
    int tumble;

    public override void SetStaticDefaults() => Main.npcFrameCount[Type] = 2;

    public override void SetDefaults()
    {
        NPC.width = 34; NPC.height = 60;
        NPC.lifeMax = 70; NPC.damage = 18; NPC.defense = 3; NPC.knockBackResist = 0.7f; NPC.value = 120;
        NPC.aiStyle = NPCAIStyleID.Fighter; AIType = NPCID.Zombie;
        NPC.HitSound = SoundID.NPCHit1; NPC.DeathSound = Mix.Style("bail");
    }

    public override void AI()
    {
        // skate: roll toward the target far faster than a zombie walks
        NPC.TargetClosest();
        if (tumble > 0) { tumble--; NPC.rotation += 0.35f * NPC.direction; if (NPC.velocity.Y == 0 && tumble < 30) tumble = 0; return; }
        NPC.rotation = 0;
        float dir = Main.player[NPC.target].Center.X > NPC.Center.X ? 1 : -1;
        if (NPC.velocity.Y == 0) NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, dir * 4.2f, 0.12f);
        NPC.spriteDirection = NPC.direction = (int)dir;
        if (NPC.velocity.Y == 0 && Main.rand.NextBool(6))
            Dust.NewDustPerfect(NPC.Bottom + new Vector2(-dir * 10, -2), DustID.Smoke, new Vector2(-dir, -0.5f), 120, default, 0.7f);
    }

    public override void FindFrame(int frameHeight)
    {
        NPC.frameCounter++;
        NPC.frame.Y = NPC.velocity.Y != 0 ? frameHeight : (int)(NPC.frameCounter / 12 % 2) * frameHeight;
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        Mix.Burst(NPC.Center, DustID.Blood, 6);
        Mix.Burst(NPC.Center, DustID.Torch, 4);
        if (NPC.life > 0 && tumble == 0) { tumble = 40; NPC.velocity.Y = -6f; Mix.Sound("ollie", NPC.Center, 0.6f, 0.4f); }
        if (NPC.life <= 0)
        {
            Mix.Shoot<BailBoard>(NPC.Center, new Vector2(NPC.direction * 3f, -9f), 0);
            var r = Mix.Shoot<Ragdoll>(NPC.Center, new Vector2(-NPC.direction * 6f, -10f), 0);
            if (r != null) r.ai[0] = 0;
            Mix.Burst(NPC.Center, DustID.Smoke, 20);
        }
    }

    public override void OnKill()
    {
        Skate.Trick(NPC.Top, "BAIL!", 250);
        Mix.Shake(6, 0.25);
        Mix.Drop(ItemID.SilverCoin, NPC.Center, 3);
    }

    public override float SpawnChance(NPCSpawnInfo spawnInfo) => spawnInfo.Player.ZoneOverworldHeight ? 0.5f : 0f;
}
