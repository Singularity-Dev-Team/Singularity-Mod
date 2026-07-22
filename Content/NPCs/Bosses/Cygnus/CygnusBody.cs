using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SingularityMod.Content.Projectiles;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace SingularityMod.Content.NPCs.Bosses.Cygnus
{
    // Body

    public class CygnusBody : CygnusSegment
    {
        public override string Texture => "SingularityMod/Content/Assets/NPCs/Cygnus/CygnusBody";

        public override void SetDefaults()
        {
            NPC.width = 40;
            NPC.height = 40;

            NPC.damage = 40;
            NPC.defense = 30;
            NPC.lifeMax = 5000;

            NPC.aiStyle = 6;

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

        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            if (projectile.type == ModContent.ProjectileType<ChangesiteBeam>())
            {
                return false;
            }
            return true;
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
            NPC head = Main.npc[(int)NPC.ai[2]];

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
            head.life -= (int)(hit.Damage / CygnusHead.CygnusLength);
            if (head.life <= 0)
            {
                NPC.timeLeft = 0;
                NPC.active = false;
            }

            NPC.life = head.life;
        }

        public override void AI()
        {
            if (NPC.life <= 0)
            {
                NPC.timeLeft = 0;
                NPC.StrikeInstantKill();
                return;
            }
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

                Vector2 direction = previousNPC.Center - NPC.Center;

                float distance = direction.Length();

                if (distance > 40)
                {
                    direction.Normalize();
                    NPC.Center = previousNPC.Center - direction * 40f;
                }

                NPC.rotation = direction.ToRotation() + MathHelper.PiOver2;
                NPC.life = headNPC.life;
            }
        }

    }
}