using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using SingularityMod.Content.Tiles;
using SingularityMod.Content.Items.Ores;
namespace SingularityMod.Content.Items.CraftingStations
{
    public class ChangesiteAnvil : ModItem
    {
        public override string Texture => "SingularityMod/Content/Assets/Items/CraftingStations/ChangesiteAnvil";
        
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(
                ModContent.TileType<ChangesiteAnvilTile>() // Make it placeable
            );

            Item.width = 12;
            Item.height = 12;
            Item.maxStack = 9999;
            Item.value = Item.sellPrice(silver: 15);
            Item.rare = ItemRarityID.Blue;
        }

        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ModContent.ItemType<ChangesiteBar>(),12).AddTile(TileID.Anvils).Register();
        }
    }
}