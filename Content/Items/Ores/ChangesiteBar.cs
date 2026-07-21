using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using SingularityMod.Content.Tiles;

namespace SingularityMod.Content.Items.Ores
{
    public class ChangesiteBar : ModItem
    {
        public override string Texture => "SingularityMod/Content/Assets/Items/Ores/ChangesiteBar";
        
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(
                ModContent.TileType<ChangesiteBarTile>() // Make it placeable
            );

            Item.width = 12;
            Item.height = 12;
            Item.maxStack = 9999;
            Item.value = Item.sellPrice(silver: 5);
            Item.rare = ItemRarityID.Blue;
        }

        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ModContent.ItemType<Content.Items.Ores.ChangesiteOre>(),3).AddTile(TileID.Furnaces).Register();
        }
    }
}