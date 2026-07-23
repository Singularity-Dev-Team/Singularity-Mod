using Microsoft.Xna.Framework;
using SingularityMod.Content.Projectiles;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace SingularityMod.Content.Items.Weapons
{
    public class Deneb : ModItem
    {
        public override string Texture => "SingularityMod/Content/Assets/Items/Weapons/Deneb";

        public override void SetDefaults()
        {
            Item.width = 35;
            Item.height = 35;

            Item.damage = 16;
            Item.DamageType = DamageClass.Ranged;

            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;

            Item.noMelee = true;
            Item.noUseGraphic = true;

            Item.knockBack = 2f;

            Item.shoot = ModContent.ProjectileType<DenebProjectile>();
            Item.shootSpeed = 12f;

            Item.UseSound = SoundID.Item1;

            Item.autoReuse = true;

            Item.value = Item.sellPrice(0,0, 30, 0);
            Item.rare = ItemRarityID.Orange;
        }

        public override bool CanUseItem(Player player)
        {
            return true;
        }

    }
}