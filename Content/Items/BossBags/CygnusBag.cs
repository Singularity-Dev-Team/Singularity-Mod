using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using SingularityMod.Content.Items.Ores;

namespace SingularityMod.Content.Items.BossBags
{
    public class CygnusBag : ModItem
    {
        public override string Texture => "SingularityMod/Content/Assets/Items/BossBags/CygnusBag";

        public override void SetStaticDefaults()
        {
            ItemID.Sets.BossBag[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;

            Item.maxStack = 9999;
            Item.consumable = true;

            Item.rare = ItemRarityID.Expert;
        }

        public override bool CanRightClick()
        {
            return true;
        }

        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            itemLoot.Add(ItemDropRule.Common(
                ModContent.ItemType<ChangesiteBar>(),
                1,
                5,
                20
            ));
            
        }
    }
}