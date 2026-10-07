using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Sigf.Content.Items;

public class SkateDeck : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 40; Item.height = 12;
        Item.damage = 32; Item.DamageType = DamageClass.Ranged; Item.knockBack = 6f;
        Item.useTime = 22; Item.useAnimation = 22; Item.useStyle = ItemUseStyleID.Swing;
        Item.noMelee = true; Item.noUseGraphic = true; Item.autoReuse = true;
        Item.UseSound = Mix.Style("ollie", 0.7f);
        Item.rare = ItemRarityID.Orange; Item.value = 5000;
        Item.shoot = ModContent.ProjectileType<DeckShot>(); Item.shootSpeed = 13f;
    }
}
