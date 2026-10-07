using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;

namespace Sigf.Content;

public class SkateSystem : ModSystem
{
    const float BaseZoom = 2.0f;

    public override void PostUpdateEverything()
    {
        if (Skate.ComboTimer > 0 && --Skate.ComboTimer == 0) Skate.Combo = 0;
        if (Skate.BigTimer > 0) Skate.BigTimer--;
        Skate.Flash = MathHelper.Max(0, Skate.Flash - 0.03f);
        Skate.ZoomPunch = MathHelper.Max(0, Skate.ZoomPunch - 0.008f);
        Main.GameZoomTarget = BaseZoom + Skate.ZoomPunch;
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        layers.Add(new LegacyGameInterfaceLayer("Sigf: Skate HUD", () => { DrawHud(Main.spriteBatch); return true; }, InterfaceScaleType.UI));
    }

    static void DrawHud(SpriteBatch sb)
    {
        var px = TextureAssets.MagicPixel.Value;
        if (Skate.Flash > 0) sb.Draw(px, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White * Skate.Flash);

        // score panel: left side, below the hotbar
        float x = 20f, y = 170f;
        Rectangle bar = new Rectangle((int)x, (int)y, 380, Skate.Combo > 0 ? 96 : 56);
        sb.Draw(px, bar, Color.Black * 0.6f);
        sb.Draw(px, new Rectangle(bar.X, bar.Y, bar.Width, 4), Color.OrangeRed);
        Utils.DrawBorderString(sb, "SKATE SCORE " + Skate.Score.ToString("N0"), new Vector2(x + 12, y + 10), Color.Orange, 1.4f);
        if (Skate.Combo > 0)
        {
            Utils.DrawBorderString(sb, $"COMBO x{Skate.Combo}", new Vector2(x + 12, y + 48), Color.Lerp(Color.Yellow, Color.HotPink, Skate.Combo / 5f), 1.4f);
            sb.Draw(px, new Rectangle(bar.X + 190, bar.Y + 60, (int)(170 * Skate.ComboTimer / 300f), 14), Color.Lerp(Color.Yellow, Color.HotPink, Skate.Combo / 5f));
        }

        // giant trick text in the upper middle of the screen
        if (Skate.BigTimer > 0)
        {
            float age = 1f - Skate.BigTimer / (float)Skate.BigLife;
            float scale = 1.0f + 0.6f * System.Math.Max(0, 1f - age * 6f);
            float alpha = Skate.BigTimer < 20 ? Skate.BigTimer / 20f : 1f;
            var size = FontAssets.DeathText.Value.MeasureString(Skate.BigText) * scale;
            Utils.DrawBorderStringBig(sb, Skate.BigText, new Vector2(Main.screenWidth / 2f - size.X / 2, Main.screenHeight * 0.22f), Skate.BigColor * alpha, scale);
        }
    }
}
