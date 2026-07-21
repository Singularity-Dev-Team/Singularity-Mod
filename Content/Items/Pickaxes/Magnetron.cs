using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using SingularityMod.Content.Tiles;
using Microsoft.Xna.Framework;

namespace SingularityMod.Content.Items.Pickaxes
{
    public class Magnetron : ModItem
    {
        public override string Texture => "SingularityMod/Content/Assets/Items/Pickaxes/Magnetron";

        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.MeleeNoSpeed;

            Item.useTime = 18;
            Item.useAnimation = 18;

            Item.useStyle = ItemUseStyleID.Shoot; // Make the magnetron be pointed

            Item.knockBack = 2f; // Regular knockback
            Item.UseSound = SoundID.Item32; // Placeholder

            Item.autoReuse = true;
            Item.noMelee = true;

            Item.pick = 55;
            Item.channel = true;

            Item.width = 32;
            Item.height = 32;
            Item.rare = ItemRarityID.Blue;
        }

        public override void HoldItem(Player player)
        {
            // Rotation

            Vector2 direction = Main.MouseWorld - player.MountedCenter;

            player.itemRotation = direction.ToRotation();

            if (direction.X < 0)
                player.ChangeDir(-1);
            else
                player.ChangeDir(1);

            if (player.direction == -1)
                player.itemRotation += MathHelper.Pi;

        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2f, 0); // Fix the item's pos
        }
    }
}