using System;
using System.IO;
using Humanizer;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SingularityMod.Content.Items.BossBags;
using SingularityMod.Content.Items.Relics;
using SingularityMod.Content.Projectiles;
using SingularityMod.Singularity;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
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
        private static int MaxPlayerDistance = 1500;
        public int CygnusID = 0; // This prevents messing up cygnus with two of them
        private static int NextCygnusID = 0; // So they all share this but not CygnusID
        enum AttackState
        {
            Hover,
            Spiral,
            None
        }
        AttackState CurrentState = AttackState.None;
        int Counter = 0;
        int HoverX = 0;
        int HoverY = 0;
        float spiralAngle = 0f;

        public bool shootingLasers = false;

        Vector2 RushPosition;
        Vector2 HoverPosition;
        Vector2 idleTarget;
        public int targetIndex;

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                new FlavorTextBestiaryInfoElement("A mysterious celestial serpent from a distant constellation.")
            });
        }

        public override void SetDefaults()
        {
            NPC.width = 36;
            NPC.height = 36;

            NPC.damage = 80;
            NPC.defense = 15;
            NPC.lifeMax = 5000;

            NPC.aiStyle = -1;

            NPC.boss = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.knockBackResist = 0f;
            Music = MusicLoader.GetMusicSlot(Mod, "Content/Assets/Music/CygnusOST");


            NPC.friendly = false;

            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.Item119; // Temporal

            NPC.value = 10000;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(targetIndex);
            writer.Write(spiralAngle);

            writer.Write((byte)CurrentState);
            writer.Write(Counter);

            writer.Write(HoverX);
            writer.Write(HoverY);

            writer.Write(RushPosition.X);
            writer.Write(RushPosition.Y);

            writer.Write(HoverPosition.X);
            writer.Write(HoverPosition.Y);

            writer.Write(idleTarget.X);
            writer.Write(idleTarget.Y);

            writer.Write(shootingLasers);

        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            targetIndex = reader.ReadInt32();
            spiralAngle = reader.ReadInt32();

            CurrentState = (AttackState)reader.ReadByte();
            Counter = reader.ReadInt32();

            HoverX = reader.ReadInt32();
            HoverY = reader.ReadInt32();

            RushPosition = new Vector2(
                reader.ReadSingle(),
                reader.ReadSingle()
            );

            HoverPosition = new Vector2(
                reader.ReadSingle(),
                reader.ReadSingle()
            );

            idleTarget = new Vector2(
                reader.ReadSingle(),
                reader.ReadSingle()
            );

            shootingLasers = reader.ReadBoolean();
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            // drops
            if (Main.expertMode || Main.masterMode)
            {
                if (!Main.expertMode)
                {
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CygnusRelic>(), 1));
                }
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CygnusBag>(), 1));
            }
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
            CygnusID = NextCygnusID++;
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

                CygnusSegment segmentMod = Main.npc[segment].ModNPC as CygnusSegment;

                if (segmentMod != null)
                {
                    segmentMod.CygnusID = CygnusID;
                }

                Main.npc[segment].ai[2] = NPC.whoAmI;
                Main.npc[segment].ai[1] = previous;
                Main.npc[previous].ai[0] = segment;

                previous = segment;
            }
        }

        void PickNextAttack()
        {
            idleTarget = Vector2.Zero;
            int r = Main.rand.Next(100);

            if (r < 50)
                CurrentState = AttackState.Hover;
            else
                CurrentState = AttackState.Spiral;
        }

        public override void AI()
        {
            // 60 ticks = 1 sec

            Player target = Main.player[NPC.target];
            targetIndex = NPC.target;
            float targetDistance = (target.Center - NPC.Center).Length();

            float healthRatio = (float)NPC.lifeMax / NPC.life;

            bool allPlayersDead = true;


            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];

                if (player.active && !player.dead)
                {
                    allPlayersDead = false;
                    break;
                }
            }

            if (!target.active || target.dead || targetDistance >= MaxPlayerDistance)
            {
                NPC.TargetClosest();
                target = Main.player[NPC.target];
            }

            if (allPlayersDead)
            {
                NPC.timeLeft = 0;
                NPC.active = false;
            }

            if (CurrentState == AttackState.None)
            {
                Vector2 targetPos = target.Center;

                if (Counter % 60 == 0)
                {
                    idleTarget = new Vector2(Main.rand.Next(-250, 250), Main.rand.Next(-250, 250));
                }
                targetPos += idleTarget;

                Vector2 toTarget = targetPos - NPC.Center;
                float distance = toTarget.Length();

                if (distance > 40f)
                {
                    Vector2 direction = toTarget.SafeNormalize(Vector2.UnitY);

                    float maxSpeed = 5f * Math.Clamp(healthRatio, 1.0f, 1.5f);
                    float acceleration = 0.08f;

                    Vector2 desiredVelocity = direction * maxSpeed;

                    NPC.velocity = Vector2.Lerp(NPC.velocity, desiredVelocity, acceleration);

                    if (NPC.velocity.Length() > maxSpeed * 1.3f)
                        NPC.velocity = NPC.velocity.SafeNormalize(Vector2.UnitY) * maxSpeed * 1.3f;
                    else if (NPC.velocity.Length() < maxSpeed * 0.7f && NPC.velocity.Length() > 0.1f)
                        NPC.velocity = NPC.velocity.SafeNormalize(Vector2.UnitY) * maxSpeed * 0.7f;
                }
                else
                {
                    // Slow down when close
                    NPC.velocity = Vector2.Lerp(NPC.velocity, Vector2.Zero, 0.08f);
                }

                if (NPC.velocity.Length() > 0.1f)
                {
                    NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;
                }

                if (Counter >= 400 / Math.Clamp(healthRatio, 1, 1.5) + Main.rand.Next(-100, 100))
                {
                    Counter = 0;
                    PickNextAttack();
                }
            }
            else
            {
                switch (CurrentState)
                {
                    case AttackState.Spiral:
                        {
                            if (Counter == 1)
                            {
                                spiralAngle = 0f;
                            }

                            float radius = 300f;
                            radius *= 0.5f;

                            float angularSpeed = 0.04f * (1f + 0.5f * (1f - Math.Clamp(healthRatio, 1, 5)));
                            spiralAngle += angularSpeed;

                            Vector2 offset = new Vector2((float)Math.Cos(spiralAngle), (float)Math.Sin(spiralAngle)) * radius;
                            Vector2 targetPos = target.Center + offset;

                            Vector2 toTarget = targetPos - NPC.Center;
                            float dist = toTarget.Length();
                            if (dist > 0f)
                            {
                                toTarget.Normalize();
                                float speed = Math.Min(dist * 0.1f, 20f * Math.Clamp(healthRatio, 1f, 2f));
                                NPC.velocity = Vector2.Lerp(NPC.velocity, toTarget * speed, 0.15f);
                            }

                            if (NPC.velocity.Length() > 0.1f)
                                NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;

                            if (Counter >= 600)
                            {
                                CurrentState = AttackState.None;
                                Counter = 0;
                            }
                            break;
                        }
                    case AttackState.Hover:
                        {
                            if (HoverPosition == Vector2.Zero)
                            {
                                HoverPosition = new Vector2(0, -100);
                                HoverPosition.Normalize();
                                HoverX = Main.rand.Next(-6, 7);
                            }

                            if (Counter >= 300 + Main.rand.Next(-100, 100))
                            {
                                CurrentState = AttackState.None;
                                HoverPosition = Vector2.Zero;
                                Counter = 0;
                                shootingLasers = false;

                                break;
                            }

                            if (Counter % 30 / (int)Math.Floor(Math.Clamp(healthRatio, 1f, 2f)) == 0)
                            {
                                shootingLasers = true;
                                bool Homing = Main.rand.NextBool();

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
                                    if (Homing)
                                    {
                                        proj.Homes = true;
                                    }
                                    Main.projectile[Proj].scale += Main.rand.NextFloat(0f, 0.5f);
                                }
                            }

                            Vector2 hoverTarget = target.Center + HoverPosition;
                            Vector2 toTarget = hoverTarget - NPC.Center;
                            float distance = toTarget.Length();
                            if (distance > 20f)
                            {
                                Vector2 direction = toTarget / distance;
                                float maxSpeed = MathHelper.Clamp(distance / 20f, 5f, 15f) * Math.Clamp(healthRatio, 1f, 2f);
                                Vector2 desiredVelocity = direction * maxSpeed;
                                NPC.velocity = Vector2.Lerp(NPC.velocity, desiredVelocity, 0.08f + 0.02f * (1f - Math.Clamp(healthRatio, 1, 5)));
                                float currentSpeed = NPC.velocity.Length();
                                if (currentSpeed > 0f)
                                {
                                    float higherSpeed = maxSpeed * 1.3f;
                                    float lowerSpeed = maxSpeed * 0.7f;
                                    if (currentSpeed > higherSpeed)
                                        NPC.velocity = NPC.velocity / currentSpeed * higherSpeed;
                                    else if (currentSpeed < lowerSpeed && currentSpeed > 0.1f)
                                        NPC.velocity = NPC.velocity / currentSpeed * lowerSpeed;
                                }
                            }
                            else
                            {
                                NPC.velocity = Vector2.Lerp(NPC.velocity, Vector2.Zero, 0.08f);
                            }
                            if (NPC.velocity.Length() > 0.1f)
                                NPC.rotation = MathHelper.Lerp(NPC.rotation, NPC.velocity.ToRotation() + MathHelper.PiOver2, 0.15f);
                            HoverPosition -= new Vector2(HoverX, HoverY);
                            break;
                        }
                }
            }

            Counter++;
        }
    }
}