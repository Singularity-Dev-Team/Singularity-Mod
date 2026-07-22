using SingularityMod.Content.Items.CraftingStations;
using SingularityMod.Content.Items.Ores;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SingularityMod.Content.Items.Summoners
{
    public class OrbitalBeacon : ModItem
    {
        public override string Texture => "SingularityMod/Content/Assets/Items/Summoners/OrbitalBeacon";
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 3; // Gotta add this to every item
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 28;

            Item.maxStack = 20;
            Item.consumable = true;

            Item.useAnimation = 45;
            Item.useTime = 45;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.UseSound = SoundID.Roar;

            Item.rare = ItemRarityID.Blue;
        }

        public override bool CanUseItem(Player player)
        {
            return !NPC.AnyNPCs(ModContent.NPCType<Content.NPCs.Bosses.Cygnus.CygnusHead>());
        }

        public override bool? UseItem(Player player)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                NPC.SpawnOnPlayer(
                    player.whoAmI,
                    ModContent.NPCType<Content.NPCs.Bosses.Cygnus.CygnusHead>()
                );
            }

            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<ChangesiteBar>(), 8)
                .AddIngredient(ItemID.FallenStar, 5)
                .AddTile(ModContent.TileType<Content.Tiles.ChangesiteAnvilTile>())
                .Register();
        }
    }
}