using System;
using Humanizer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SingularityMod.Content.Projectiles;
using SingularityMod.Singularity;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace SingularityMod.Content.NPCs.Bosses.Cygnus
{
    // Head
    [AutoloadBossHead]
    public class CygnusHead : ModNPC
    {
        public override string BossHeadTexture => "SingularityMod/Content/Assets/NPCs/Cygnus/Cygnus_Head";
        public override string Texture => "SingularityMod/Content/Assets/NPCs/Cygnus/CygnusHead";
        public static int CygnusLength = 30;

        int RushIntentTick = 0;
        int HoverIntentTick = 0;
        bool ChargingRush = false;
        int ChargingRushTick = 0;
        bool Rushing = false;
        int RushingTick = 0;
        bool Hovering = false;
        int HoverTicks = 0;
        int HoverX = 0;
        int HoverY = 0;

        Vector2 prevPos;

        Vector2 RushDirection;
        Vector2 HoverPos;

        public override void SetDefaults()
        {
            NPC.width = 36;
            NPC.height = 36;

            NPC.damage = 50;
            NPC.defense = 15;
            NPC.lifeMax = 5000;

            NPC.aiStyle = -1;

            NPC.boss = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.knockBackResist = 0f;

            NPC.friendly = false;

            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.Item119; // Temporal

            NPC.value = 10000;
        }

        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            if (projectile.type == ModContent.ProjectileType<ChangesiteBeam>())
            {
                return false;
            }
            return true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            //Glow 

            Texture2D texture = Terraria.GameContent.TextureAssets.Npc[NPC.type].Value;

            Texture2D Glow = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Glow").Value;

            Rectangle sourceRect = NPC.frame;

            Vector2 origin2 = sourceRect.Size() / 2;

            spriteBatch.Draw(
                texture,
                NPC.Center - screenPos,
                sourceRect,
                drawColor,
                NPC.rotation,
                origin2,
                NPC.scale,
                SpriteEffects.None,
                0f
            );

            spriteBatch.Draw(
                Glow,
                NPC.Center - screenPos,
                sourceRect,
                Color.White,
                NPC.rotation,
                origin2,
                NPC.scale,
                SpriteEffects.None,
                0f
            );

            return false;
        }

        public override bool CheckActive()
        {
            return false; // prevents the NPC from despawning
        }


        public override void OnKill()
        {
            SingularityWorld.downedCygnusBoss = true; // set to true so vodoo demons can spawn
        }

        public override void OnSpawn(IEntitySource source)
        {
            int previous = NPC.whoAmI;

            for (int i = 0; i < CygnusLength; i++)
            {
                int type = ModContent.NPCType<CygnusBody>(); // Complete with body

                if (i == CygnusLength - 1)
                    type = ModContent.NPCType<CygnusTail>(); // Last part will be the Tail

                int segment = NPC.NewNPC(
                    NPC.GetSource_FromAI(),
                    (int)NPC.Center.X,
                    (int)NPC.Center.Y,
                    type
                );
                Main.npc[segment].ai[2] = NPC.whoAmI;
                Main.npc[segment].ai[1] = previous;
                Main.npc[previous].ai[0] = segment;

                previous = segment;
            }
        }

        public override void AI()
        {
            // 60 ticks = 1 sec

            //Main.NewText($"Hovering: {Hovering}, Charging: {ChargingRush}, Rushing: {Rushing}"); // Debug

            Player target = Main.player[NPC.target];
            float healthRatioLower = (float)NPC.life / NPC.lifeMax;
            float healthRatioHigher = (float)NPC.lifeMax / NPC.life;


            if (!Rushing && !ChargingRush && !Hovering)
            {
                if (!target.active || target.dead)
                {
                    NPC.TargetClosest();
                    target = Main.player[NPC.target];
                }
                else
                {
                    NPC.timeLeft = 0;
                }

                Vector2 direction = target.Center - NPC.Center;
                direction.Normalize();
                NPC.rotation += MathHelper.WrapAngle(direction.ToRotation() + MathHelper.PiOver2 - NPC.rotation) * 0.15f; // Interpolate
                NPC.velocity = Vector2.Lerp(NPC.velocity, direction * 4f * Math.Clamp(healthRatioHigher, 1.0f, 1.5f), 0.08f);

                if (RushIntentTick >= 450 * Math.Clamp(healthRatioLower, 0.5, 1) && RushIntentTick <= 500 * Math.Clamp(healthRatioLower, 0.5, 1) && !Hovering)
                {
                    // decelerate
                    NPC.velocity = Vector2.Lerp(NPC.velocity, Vector2.Zero, 0.08f);
                }
                if (RushIntentTick >= 500 * Math.Clamp(healthRatioLower, 0.5, 1) && !Hovering)
                {
                    ChargingRush = true;
                    NPC.velocity = Vector2.Zero;
                    NPC.rotation += MathHelper.WrapAngle(direction.ToRotation() + MathHelper.PiOver2 - NPC.rotation) * 0.15f; // Interpolate
                    RushIntentTick = 0;
                }

                if (HoverIntentTick >= 300 * Math.Clamp(healthRatioLower, 0.5, 1) && !ChargingRush)
                {
                    Hovering = true;
                    NPC.rotation += MathHelper.WrapAngle(direction.ToRotation() + MathHelper.PiOver2 - NPC.rotation) * 0.15f; // Interpolate
                    HoverIntentTick = 0;
                    HoverPos = new Vector2(0, -300);
                    HoverX = new Random().Next(-5, 6);
                    HoverY = new Random().Next(-2, 3);
                }
                RushIntentTick++;
                HoverIntentTick++;
                NPC.netUpdate = true;
                return;
            }

            // Attacks

            if (Hovering)
            {
                if (HoverTicks >= 200)
                {
                    HoverTicks = 0;
                    Hovering = false;
                }
                if (HoverTicks % 30 == 0)
                {
                    int R = new Random().Next(0, 2);
                    SoundEngine.PlaySound(SoundID.Item33, NPC.position);
                    Vector2 dir = target.Center - NPC.Center;
                    dir.Normalize();
                    int Proj = Projectile.NewProjectile(
                    NPC.GetSource_FromAI(),              // The spawn source context
                    NPC.Center,                          // Where it spawns (NPC center)
                    dir * 10,                            // The direction and speed
                    ModContent.ProjectileType<ChangesiteBeam>(),     // The projectile type
                    25,                                  // Damage dealt to the player
                    1f,                                  // Knockback force
                    Main.myPlayer                        // The owner index
                    );
                    if (Main.projectile[Proj].ModProjectile is ChangesiteBeam proj)
                    {
                        if (R == 1)
                        {
                            proj.Homes = true;
                        }
                        Main.projectile[Proj].scale += Main.rand.NextFloat(0f, 0.5f);
                    }
                }
                Vector2 hoverTarget = target.Center + HoverPos;
                Vector2 direction = hoverTarget - NPC.Center;
                direction.Normalize();
                NPC.velocity = Vector2.Lerp(NPC.velocity, direction * 10, 0.1f);
                NPC.rotation += MathHelper.WrapAngle(NPC.velocity.ToRotation() + MathHelper.PiOver2 - NPC.rotation) * 0.15f; // Interpolate
                HoverTicks++;
                HoverPos -= new Vector2(HoverX, HoverY);
            }

            if (ChargingRush && !Rushing)
            {
                if (ChargingRushTick >= 15)
                {
                    Rushing = true;
                    ChargingRushTick = 0;
                    ChargingRush = false;
                    RushDirection = target.Center;
                    SoundEngine.PlaySound(SoundID.ForceRoar, NPC.position);
                }
                ChargingRushTick++;
            }
            if (Rushing)
            {
                Vector2 toTarget = RushDirection - NPC.Center;
                float distance = toTarget.Length();
                if (RushingTick > 80 * healthRatioHigher)
                {
                    Rushing = false;
                    RushingTick = 0;
                }
                Vector2 direction = RushDirection - NPC.Center;
                direction.Normalize();
                if (distance >= 10)
                {
                    NPC.velocity = Vector2.Lerp(NPC.velocity, direction * 10f * Math.Clamp(healthRatioHigher, 1f, 2f), 0.08f);
                    NPC.rotation += MathHelper.WrapAngle(direction.ToRotation() + MathHelper.PiOver2 - NPC.rotation) * 0.15f; // Interpolate
                }
                else
                {
                    Rushing = false;
                    RushingTick = 0;
                }
                prevPos = NPC.position;
                RushingTick++;
            }

            NPC.netUpdate = true;
        }
    }
}