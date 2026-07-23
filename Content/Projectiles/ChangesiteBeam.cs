using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.Audio;

namespace SingularityMod.Content.Projectiles
{
    public class ChangesiteBeam : ModProjectile
    {
        public override string Texture => "SingularityMod/Content/Assets/Projectiles/ChangesiteBeam";

        public bool Homes { get; set; } = false;

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.DamageType = DamageClass.Magic;

            Projectile.penetrate = 1;
            Projectile.timeLeft = 100;
            Projectile.tileCollide = true;

            Projectile.aiStyle = -1;
            Projectile.scale = 1f;
            Projectile.alpha = 0;
            AIType = ProjectileID.Bullet;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            lightColor = Color.White;
            return true;
        }

        public override void OnKill(int timeLeft)
        {
            // Play sound
            SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);

            // Spawn dust
            for (int i = 0; i < 20; i++)
            {
                Dust dust = Dust.NewDustDirect(
                    Projectile.position,
                    Projectile.width,
                    Projectile.height,
                    DustID.BlueTorch
                );

                dust.velocity *= 2f;
                dust.noGravity = true;
            }
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            Projectile.Kill();
        }
        public override void AI()
        {
            if (Homes)
            {
                Player target = Main.player[Player.FindClosest(Projectile.Center, Projectile.width, Projectile.height)];

                float speed = 10f;
                float inertia = 20f;

                Vector2 desiredVelocity = Projectile.DirectionTo(target.Center) * speed;
                Projectile.velocity = (Projectile.velocity * (inertia - 1) + desiredVelocity) / inertia;
            }
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            Lighting.AddLight(Projectile.Center, 0.2f, 0.6f, 1.2f);

        }
    }
}
