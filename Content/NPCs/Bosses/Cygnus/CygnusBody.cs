using System;
using System.IO;
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
    // Body

    public class CygnusBody : CygnusSegment
    {
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
        }

        public override string Texture => "SingularityMod/Content/Assets/NPCs/Cygnus/CygnusBody";
        private int Counter = 0;

        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Hide = true
            });
            NPCID.Sets.CantTakeLunchMoney[Type] = true;
        }
        public override void SetDefaults()
        {
            NPC.width = 40;
            NPC.height = 40;

            NPC.damage = 40;
            NPC.defense = 30;
            NPC.Mod().DamageResist = 0.75f;
            NPC.lifeMax = 5000;

            NPC.aiStyle = -1;

            Main.npcFrameCount[Type] = 3;

            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.friendly = false;
            NPC.knockBackResist = 0f;

            NPC.HitSound = SoundID.NPCHit4;
        }

        public override bool CheckActive()
        {
            return false; // prevents the NPC from despawning
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frame.Y = (int)NPC.ai[3] * frameHeight; // choose body variant
        }
        public override void OnSpawn(IEntitySource source)
        {
            NPC.ai[3] = Main.rand.Next(0, 3);
            NPC.netUpdate = true;
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

        private void InstakillNPC(NPC npc)
        {
            NPC.HitInfo hit = new NPC.HitInfo();
            hit.Damage = npc.lifeMax;
            hit.InstantKill = true;
            hit.HitDirection = 1;
            npc.StrikeNPC(hit);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            /*NPC head = Main.npc[(int)NPC.ai[2]];

            if (!head.active)
            {
                NPC.timeLeft = 0;
                NPC.active = false;
                return;
            }

            CygnusHead headMod = head.ModNPC as CygnusHead;

            if (headMod == null || headMod.CygnusID != CygnusID)
            {
                NPC.timeLeft = 0;
                NPC.active = false;
                return;
            }
            NPC.HitInfo replicate = hit;
            replicate.Damage /= 3;
            head.StrikeNPC(replicate);
            if (head.life <= 0)
            {

                NPC.timeLeft = 0;
                NPC.active = false;
            }

            NPC.life = head.life;
            NPC.lifeMax = head.lifeMax;
            if (NPC.life != head.life || NPC.lifeMax != head.lifeMax)
            {
                NPC.life = head.life;
                NPC.lifeMax = head.lifeMax;
                NPC.netUpdate = true;
            }*/
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Counter);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Counter = reader.ReadInt32();
        }

        public override void AI()
        {
            NPC headNPC = Main.npc[(int)NPC.ai[2]];
            NPC.life = headNPC.life;
            /*if (NPC.life <= 0)
            {
                NPC.timeLeft = 0;
                NPC.active = false;
                return;
            }*/
            Wormaround();
            Lasers();
        }
        public void Wormaround()
        {

            int previous = (int)NPC.ai[1];
            NPC previousNPC = Main.npc[previous];
            NPC headNPC = Main.npc[(int)NPC.ai[2]];

            if (!headNPC.active)
            {
                NPC.timeLeft = 0;
                NPC.active = false;
                return;
            }

            CygnusHead headMod = headNPC.ModNPC as CygnusHead;

            if (headMod == null || headMod.CygnusID != CygnusID)
            {
                NPC.timeLeft = 0;
                NPC.active = false;
                return;
            }

            if (previous >= 0 && previousNPC.active && headNPC.active)
            {
                if (headNPC.life <= 0)
                {
                    NPC.timeLeft = 0;
                    NPC.active = false;
                    return;
                }
            }

            // smooth movement (extra sick and cool)

            Vector2 directionToNextSegment = previousNPC.Center - NPC.Center;
            if (previousNPC.rotation != NPC.rotation)
            {
                directionToNextSegment = directionToNextSegment.RotatedBy(MathHelper.WrapAngle(previousNPC.rotation - NPC.rotation) * 0.04f);
                directionToNextSegment = directionToNextSegment.MoveTowards((previousNPC.rotation - NPC.rotation).ToRotationVector2(), 1f);
            }

            NPC.rotation = directionToNextSegment.ToRotation() + MathHelper.PiOver2;
            NPC.Center = previousNPC.Center - directionToNextSegment.SafeNormalize(Vector2.Zero) * NPC.scale * NPC.width;
            NPC.spriteDirection = (directionToNextSegment.X > 0).ToDirectionInt();

            /*Vector2 direction = previousNPC.Center - NPC.Center;

            float distance = direction.Length();

            if (distance > 40)
            {
                direction.Normalize();
                NPC.Center = previousNPC.Center - direction * 40f;
            }

            NPC.rotation = direction.ToRotation() + MathHelper.PiOver2;
            NPC.life = headNPC.life;
            NPC.lifeMax = headNPC.lifeMax;*/
        }

        // out of the way
        public void Lasers()
        {
            NPC headNPC = Main.npc[(int)NPC.ai[2]];
            CygnusHead headMod = headNPC.ModNPC as CygnusHead;

            if (headMod.shootingLasers)
            {
                if (Counter % 60 + Main.rand.Next(-5, 6) == 0 && Main.rand.Next(0, 5) == 1)
                {
                    SoundEngine.PlaySound(SoundID.Item33, NPC.position);
                    Vector2 dir = Main.player[headMod.targetIndex].Center - NPC.Center;
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

                    Main.projectile[Proj].scale += Main.rand.NextFloat(0f, 0.25f);
                    Counter = 0;
                }
                Counter++;
            }
            else
            {
                Counter = 0;
            }
        }
    }
}