using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Sigf.Content;

public class SigfMod : ModSystem
{
    public override void OnWorldLoad()
    {
        Skate.Score = 0; Skate.Combo = 0;
        Mix.WanderTiles = 24;

        // the skate park appears next to spawn: ramps and a grind rail on both sides
        for (int j = 0; j < 11; j++)
        {
            int jj = j;
            Mix.After(1.5 + j * 0.25, () => { BuildColumn(1, jj); BuildColumn(-1, jj); Mix.Shake(4, 0.15); Mix.Sound("rise", Mix.Center + new Vector2(0, -20), 0.5f, jj * 0.05f); });
        }

        Mix.Every(3, () => { if (Mix.NpcsNear(Mix.Host.Center, 40, n => n.type == ModContent.NPCType<SkatePunk>()).Count < 5) Mix.Spawn<SkatePunk>(Mix.Ground(26)); });
        Mix.Every(22, () => { if (Mix.NpcsNear(Mix.Host.Center, 40, n => n.type == ModContent.NPCType<MallCop>()).Count < 1) Mix.Spawn<MallCop>(Mix.Ground(26)); });

        Mix.Demo(0.5, () => { Mix.Arm<Items.SkateDeck>(); Mix.Title("SKATE 3 TAKEOVER", "", 3); });
        Mix.Demo(1.5, () => { Mix.Spawn<SkatePunk>(Mix.Ahead(6)); Mix.Spawn<SkatePunk>(Mix.Ahead(10)); });
        Mix.Demo(12, () => { Mix.Spawn<MallCop>(Mix.Ahead(12)); });
        Mix.Demo(30, () => { Mix.Spawn<SkatePunk>(Mix.Ahead(10)); Mix.Spawn<SkatePunk>(Mix.Ahead(-10)); });
    }

    /// <summary>Stone ramp, a plateau topped with a grind rail, and a down ramp. side = 1 right of spawn, -1 left.</summary>
    static void BuildColumn(int side, int j)
    {
        int cx = (int)(Mix.Center.X / 16), gy = (int)(Mix.Center.Y / 16);
        int[] heights = { 1, 2, 3, 3, 3, 3, 3, 3, 3, 2, 1 };
        {
            int x = cx + side * (9 + j);
            for (int h = 1; h <= heights[j]; h++) { WorldGen.PlaceTile(x, gy - h, TileID.CopperBrick, true, true); { Tile t = Main.tile[x, gy - h]; t.TileColor = PaintID.OrangePaint; } }
            if (j < 3) WorldGen.SlopeTile(x, gy - heights[j], side == 1 ? 2 : 1);
            if (j > 8) WorldGen.SlopeTile(x, gy - heights[j], side == 1 ? 1 : 2);
            Mix.Burst(new Vector2(x * 16 + 8, (gy - 1) * 16), DustID.Torch, 12);
            if (j >= 3 && j <= 8) { WorldGen.PlaceTile(x, gy - 4, TileID.Platforms, true, true); { Tile t = Main.tile[x, gy - 4]; t.TileColor = PaintID.YellowPaint; } }
        }
    }
}
