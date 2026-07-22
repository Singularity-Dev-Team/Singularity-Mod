using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace SingularityMod.Content.Tiles
{
    public class CygnusRelicTile : ModTile
    {
        public override string Texture => "SingularityMod/Content/Assets/Items/Relics/RelicPedestal";
        public const int FrameWidth = 18 * 3;
        public const int FrameHeight = 18 * 4;

        public Asset<Texture2D> RelicTexture;
        public virtual string RelicTextureName => "SingularityMod/Content/Assets/Items/Relics/Cygnus/CygnusRelicFloating";

        public override void Load()
        {
            RelicTexture = ModContent.Request<Texture2D>(RelicTextureName);
        }

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileShine[Type] = 400;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
            TileObjectData.newTile.DrawYOffset = 2;

            TileObjectData.addTile(Type);

            AddMapEntry(new Microsoft.Xna.Framework.Color(80, 150, 255));
        }


        public override void DrawEffects(
            int i,
            int j,
            SpriteBatch spriteBatch,
            ref TileDrawInfo drawData)
        {
            if (drawData.tileFrameX % FrameWidth == 0 &&
                drawData.tileFrameY % FrameHeight == 0)
            {
                Main.instance.TilesRenderer.AddSpecialPoint(
                    i,
                    j,
                    Terraria.GameContent.Drawing.TileDrawing.TileCounterType.CustomNonSolid
                );
            }
        }


        public override void SpecialDraw(
            int i,
            int j,
            SpriteBatch spriteBatch)
        {
            Tile tile = Main.tile[i, j];

            if (!tile.HasTile)
                return;


            Texture2D texture = RelicTexture.Value;

            Rectangle frame = texture.Frame();

            Vector2 origin = frame.Size() / 2f;


            Vector2 worldPos = new Vector2(
                i * 16 + 24,
                j * 16 + 64
            );


            Color color = Lighting.GetColor(i, j);

            float bob = (float)Math.Sin(
                Main.GlobalTimeWrappedHourly * MathHelper.TwoPi / 5f
            );


            Vector2 drawPos =
                worldPos
                - Main.screenPosition
                + new Vector2(0, -40 + bob * 4);


            spriteBatch.Draw(
                texture,
                drawPos,
                frame,
                color,
                0f,
                origin,
                1f,
                SpriteEffects.None,
                0f
            );
        }
    }
}