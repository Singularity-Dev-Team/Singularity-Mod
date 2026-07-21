using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ID;

namespace SingularityMod.Tiles
{
    public class ChangesiteTile : ModTile
    {
        public override string Texture => "SingularityMod/Assets/Tiles/ChangesiteTile";
        
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;

            AddMapEntry(new Color(125, 127, 130));

            DustType = DustID.Silver;

            MineResist = 1.5f;
            MinPick = 55; // Gold / Platinum pickaxe
        }
    }
}