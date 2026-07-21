using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using SingularityMod.Content.Tiles;

namespace SingularityMod.Content.Items.Ores
{
    public class ChangesiteOre : ModItem
    {
        public override string Texture => "SingularityMod/Content/Assets/Items/Ores/ChangesiteOre";
        
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(
                ModContent.TileType<ChangesiteTile>() // Make it placeable
            );

            Item.width = 12;
            Item.height = 12;
            Item.maxStack = 9999;
            Item.value = Item.sellPrice(silver: 5);
            Item.rare = ItemRarityID.Blue;
        }
    }
}