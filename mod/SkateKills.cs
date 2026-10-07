using Terraria;
using Terraria.ModLoader;

namespace Sigf.Content;

/// <summary>Any vanilla enemy you beat also scores skate points.</summary>
public class SkateKills : GlobalNPC
{
    public override void OnKill(NPC npc)
    {
        if (npc.ModNPC != null || npc.friendly || npc.lifeMax < 5 || npc.townNPC) return;
        Skate.Trick(npc.Top, "WIPEOUT", 80);
    }
}
