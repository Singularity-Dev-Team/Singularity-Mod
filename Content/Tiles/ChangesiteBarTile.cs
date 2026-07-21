using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ObjectData;

namespace SingularityMod.Content.Tiles
{
    public class ChangesiteBarTile : ModTile
    {
        public override string Texture => "SingularityMod/Content/Assets/Tiles/ChangesiteBarTile";
        
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileFrameImportant[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.CoordinateHeights = new[] { 18 };
            TileObjectData.addTile(Type);

            AddMapEntry(new Color(125, 127, 130));

            DustType = DustID.Silver;

            MineResist = 5.0f;
            MinPick = 55; // Gold / Platinum pickaxe
        }
    }
}