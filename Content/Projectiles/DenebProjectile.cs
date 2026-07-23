using System.IO;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SingularityMod.Content.Projectiles
{
    public class DenebProjectile : ModProjectile
    {
        public override string Texture => "SingularityMod/Content/Assets/Items/Weapons/Deneb";

        public override void SetDefaults()
        {
            Projectile.width = 35;
            Projectile.height = 35;

            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.DamageType = DamageClass.Ranged;

            Projectile.penetrate = 3;
            Projectile.timeLeft = 300;

            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
        }

        private bool returning;

        public override void AI()
        {
            Projectile.rotation += 0.5f;

            if (!returning)
            {
                if (Projectile.timeLeft < 270)
                    returning = true;
            }
            else
            {
                Player player = Main.player[Projectile.owner];

                Vector2 direction = player.Center - Projectile.Center;
                float distance = direction.Length();

                direction.Normalize();

                Projectile.velocity = Vector2.Lerp(
                    Projectile.velocity,
                    direction * 26f,
                    0.15f
                );

                if (distance < 24f)
                {
                    Projectile.Kill();
                }
            }
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(returning);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            returning = reader.ReadBoolean();
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            returning = true;
            Projectile.tileCollide = false;
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            Terraria.Audio.SoundEngine.PlaySound(SoundID.Item10, Projectile.position);

            for (int i = 0; i < 12; i++)
            {
                Dust.NewDust(
                    Projectile.position,
                    Projectile.width,
                    Projectile.height,
                    DustID.BlueTorch
                );
            }
        }
    }
}