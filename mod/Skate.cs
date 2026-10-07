using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Sigf.Content;

/// <summary>Skate score: every trick adds points, chained tricks stack a combo multiplier, and big moments flash on screen.</summary>
public static class Skate
{
    public static int Score;
    public static int Combo;
    public static int ComboTimer;
    public static string BigText = "";
    public static Color BigColor = Color.White;
    public static int BigTimer;
    public static int BigLife = 1;
    public static float Flash;
    public static float ZoomPunch;
    static readonly Color[] Colors = { Color.Orange, Color.Yellow, Color.LimeGreen, Color.Cyan, Color.HotPink };

    public static void Trick(Vector2 pos, string name, int points, bool announce = true)
    {
        Combo = Math.Min(Combo + 1, 5);
        ComboTimer = 300;
        int p = points * Combo;
        Score += p;
        if (announce)
        {
            Big($"{name} +{p}", Colors[Main.rand.Next(Colors.Length)], 70);
            if (Combo >= 2) Mix.Sound("combo", pos, 0.6f, 0.05f * Combo);
        }
    }

    public static void Big(string text, Color color, int ticks)
    {
        BigText = text; BigColor = color; BigTimer = BigLife = ticks;
    }

    /// <summary>The big moment: huge text, white flash, screen shake and a camera punch-in.</summary>
    public static void HallOfMeat(Vector2 pos, string text, int points)
    {
        Trick(pos, text, points, false);
        Big(text + $" +{points * Combo}", Color.Red, 110);
        Flash = 0.7f; ZoomPunch = 0.35f;
        Mix.Shake(14, 0.5);
        Mix.Sound("meat", pos);
    }
}
