using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ObjectData;

namespace SingularityMod.Content.Tiles
{
    public class ChangesiteAnvilTile : ModTile
    {
        public override string Texture => "SingularityMod/Content/Assets/Tiles/ChangesiteAnvil";
        
        public override void SetStaticDefaults()
        {
            Main.tileSolidTop[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = false;
            Main.tileFrameImportant[Type] = true;
            AdjTiles = new int[] { TileID.Anvils };

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x1);
            TileObjectData.newTile.CoordinateHeights = new[] { 18 };
            TileObjectData.addTile(Type);

            AddMapEntry(new Color(125, 127, 130));

            DustType = DustID.Silver;

            MineResist = 5.0f;
            MinPick = 55; // Gold / Platinum pickaxe
        }
    }
}