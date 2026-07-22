using SingularityMod.Content.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SingularityMod.Content.Items.Relics
{
    public class CygnusRelic : ModItem
    {
        public override string Texture => "SingularityMod/Content/Assets/Items/Relics/Cygnus/CygnusRelic";
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 48;

            Item.maxStack = 9999;
            Item.useTime = 10;
            Item.useAnimation = 10;

            Item.rare = ItemRarityID.Master;

            Item.useStyle = Terraria.ID.ItemUseStyleID.Swing;
            Item.createTile = ModContent.TileType<CygnusRelicTile>();
        }
    }
}