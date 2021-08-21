using JoJoStands.NPCs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoStands.Projectiles.PlayerStands.KillerQueen
{
    public class KillerQueenStandT1 : StandClass
    {
        public override int punchDamage => 14;
        public override int altDamage => 61;
        public override int punchTime => 12;
        public override int halfStandHeight => 37;
        public override float fistWhoAmI => 5f;
        public override float maxAltDistance => 165f;     //about 10 tiles
        public override string poseSoundName => "IWouldntLose";
        public override string spawnSoundName => "Killer Queen";
        public override int standType => 1;


        private int explosionTimer = 0;
        private Vector2 savedPosition = Vector2.Zero;
        private bool touchedNPC = false;
        private bool touchedTile = false;

        public static NPC savedTarget = null;
        private int updateTimer = 0;

        public override void AI()
        {
            SelectAnimation();
            UpdateStandInfo();
            updateTimer++;
            if (shootCount > 0)
                shootCount--;

            if (updateTimer >= 90)
            {
                projectile.netUpdate = true;
                updateTimer = 0;
            }
            Player player = Main.player[projectile.owner];
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();
            if (mPlayer.standOut)
                projectile.timeLeft = 2;

            if (!mPlayer.standAutoMode)
            {
                if (Main.mouseLeft && projectile.owner == Main.myPlayer)
                {
                    Punch();
                }
                else
                {
                    if (player.whoAmI == Main.myPlayer)
                        attackFrames = false;
                }
                if (!attackFrames)
                {
                    StayBehind();
                }
                if (Main.mouseRight && shootCount <= 0 && projectile.owner == Main.myPlayer)
                {
                    attackFrames = false;
                    normalFrames = false;
                    float mouseToPlayerDistance = Vector2.Distance(Main.MouseWorld, player.Center);

                    if (!touchedNPC && !touchedTile)
                    {
                        if (mouseToPlayerDistance < maxAltDistance)
                        {
                            bool foundNPCTarget = false;        //This first so you can get targets in tiles, like worms
                            for (int n = 0; n < Main.maxNPCs; n++)
                            {
                                NPC npc = Main.npc[n];
                                if (npc.active)
                                {
                                    if (npc.Distance(Main.MouseWorld) <= (npc.width / 2f) + 20f)
                                    {
                                        touchedNPC = true;
                                        shootCount += 60;
                                        foundNPCTarget = true;
                                        npc.GetGlobalNPC<JoJoGlobalNPC>().taggedByKillerQueen = true;
                                        Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/sound/KQButtonClick"));
                                        break;
                                    }
                                }
                            }
                            if (!foundNPCTarget)
                            {
                                if (Collision.SolidCollision(Main.MouseWorld, 1, 1) && !touchedTile)
                                {
                                    touchedTile = true;
                                    shootCount += 60;
                                    savedPosition = Main.MouseWorld;
                                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/sound/KQButtonClick"));
                                }
                            }
                        }
                    }
                    else
                    {
                        if (touchedNPC)
                        {
                            for (int n = 0; n < Main.maxNPCs; n++)
                            {
                                NPC npc = Main.npc[n];
                                if (npc.active)
                                {
                                    JoJoGlobalNPC jojoNPC = npc.GetGlobalNPC<JoJoGlobalNPC>();
                                    if (jojoNPC.taggedByKillerQueen)
                                    {
                                        touchedNPC = false;
                                        int projectile = Projectile.NewProjectile(npc.position, Vector2.Zero, mod.ProjectileType("KillerQueenBomb"), 0, 9f, player.whoAmI, (int)(altDamage * mPlayer.standDamageBoosts));
                                        Main.projectile[projectile].timeLeft = 2;
                                        Main.projectile[projectile].netUpdate = true;
                                        jojoNPC.taggedByKillerQueen = false;
                                        break;
                                    }
                                }
                            }
                        }
                        if (touchedTile)
                        {
                            touchedTile = false;
                            secondaryAbilityFrames = true;
                            int projectile = Projectile.NewProjectile(savedPosition, Vector2.Zero, mod.ProjectileType("KillerQueenBomb"), 0, 9f, player.whoAmI, (int)(altDamage * mPlayer.standDamageBoosts));
                            Main.projectile[projectile].timeLeft = 2;
                            Main.projectile[projectile].netUpdate = true;
                            savedPosition = Vector2.Zero;
                        }
                    }
                }
                else
                {
                    secondaryAbilityFrames = false;
                }
            }
            if (mPlayer.standAutoMode)
            {
                NPC target = FindNearestTarget(newMaxDistance * 1.5f);
                if (!attackFrames)
                {
                    StayBehind();
                }
                float touchedTargetDistance = 0f;
                if (savedTarget != null)
                {
                    touchedTargetDistance = Vector2.Distance(player.Center, savedTarget.Center);
                    if (!savedTarget.active)
                    {
                        savedTarget = null;
                        explosionTimer = 0;
                    }
                }
                if (savedTarget != null && touchedTargetDistance > newMaxDistance + 8f)       //if the target leaves and the bomb won't damage you, detonate the enemy
                {
                    attackFrames = false;
                    normalFrames = false;
                    secondaryAbilityFrames = true;
                    explosionTimer++;
                    if (explosionTimer == 5)
                    {
                        Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/sound/KQButtonClick"));
                    }
                    if (explosionTimer >= 90)
                    {
                        int projectile = Projectile.NewProjectile(savedTarget.position, Vector2.Zero, mod.ProjectileType("KillerQueenBomb"), 0, 9f, player.whoAmI, (int)(altDamage * mPlayer.standDamageBoosts));
                        Main.projectile[projectile].timeLeft = 2;
                        Main.projectile[projectile].netUpdate = true;
                        explosionTimer = 0;
                        savedTarget = null;
                    }
                }
                if (target != null)
                {
                    attackFrames = true;
                    normalFrames = false;

                    projectile.direction = 1;
                    if (target.position.X - projectile.Center.X < 0)
                    {
                        projectile.direction = -1;
                    }
                    projectile.spriteDirection = projectile.direction;

                    Vector2 velocity = target.position - projectile.position;
                    velocity.Normalize();
                    projectile.velocity = velocity * 4f;

                    if (shootCount <= 0)
                    {
                        if (Main.myPlayer == projectile.owner)
                        {
                            shootCount += newPunchTime;
                            Vector2 shootVel = target.position - projectile.Center;
                            if (shootVel == Vector2.Zero)
                            {
                                shootVel = new Vector2(0f, 1f);
                            }
                            shootVel.Normalize();
                            if (projectile.direction == 1)
                            {
                                shootVel *= shootSpeed;
                            }
                            int proj = Projectile.NewProjectile(projectile.Center, shootVel, mod.ProjectileType("Fists"), (int)(newPunchDamage * 0.9f), 3f, projectile.owner, fistWhoAmI, tierNumber);
                            Main.projectile[proj].netUpdate = true;
                            projectile.netUpdate = true;
                        }
                    }
                }
                else
                {
                    normalFrames = true;
                    attackFrames = false;
                }
            }

            if (touchedTile && MyPlayer.AutomaticActivations)
            {
                for (int n = 0; n < Main.maxNPCs; n++)
                {
                    NPC npc = Main.npc[n];
                    float npcDistance = Vector2.Distance(npc.Center, savedPosition);
                    if (npcDistance < 50f && touchedTile)       //or youd need to go from its center, add half its width to the direction its facing, and then add 16 (also with direction) -- Direwolf
                    {
                        touchedTile = false;
                        int projectile = Projectile.NewProjectile(savedPosition, Vector2.Zero, mod.ProjectileType("KillerQueenBomb"), 0, 9f, player.whoAmI, (int)(altDamage * mPlayer.standDamageBoosts));
                        Main.projectile[projectile].timeLeft = 2;
                        Main.projectile[projectile].netUpdate = true;
                        savedPosition = Vector2.Zero;
                    }
                }
            }
            LimitDistance();
        }

        public override bool PreDrawExtras(SpriteBatch spriteBatch)
        {
            if (touchedTile)
            {
                Texture2D texture = mod.GetTexture("Extras/Bomb");
                spriteBatch.Draw(texture, savedPosition - Main.screenPosition, new Rectangle(0, 0, 16, 16), Color.White, 0f, new Vector2(16f / 2f), 1f, SpriteEffects.None, 0);
            }
            return true;
        }

        public override void SelectAnimation()
        {
            if (attackFrames)
            {
                normalFrames = false;
                PlayAnimation("Attack");
            }
            if (normalFrames)
            {
                attackFrames = false;
                PlayAnimation("Idle");
            }
            if (secondaryAbilityFrames)
            {
                normalFrames = false;
                attackFrames = false;
                PlayAnimation("Secondary");
                if (projectile.frame >= 4)      //cause it should only click once
                {
                    secondaryAbilityFrames = false;
                }
            }
            if (Main.player[projectile.owner].GetModPlayer<MyPlayer>().poseMode)
            {
                normalFrames = false;
                attackFrames = false;
                PlayAnimation("Pose");
            }
        }

        public override void PlayAnimation(string animationName)
        {
            if (Main.netMode != NetmodeID.Server)
                standTexture = mod.GetTexture("Projectiles/PlayerStands/KillerQueen/KillerQueen_" + animationName);

            if (animationName == "Idle")
            {
                AnimateStand(animationName, 4, 20, true);
            }
            if (animationName == "Attack")
            {
                AnimateStand(animationName, 4, newPunchTime, true);
            }
            if (animationName == "Secondary")
            {
                AnimateStand(animationName, 6, 18, true);
            }
            if (animationName == "Pose")
            {
                AnimateStand(animationName, 1, 2, true);
            }
        }
    }
}