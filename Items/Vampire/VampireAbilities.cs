using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoStands.Items.Vampire
{
    public class VampireAbilities : VampireDamageClass
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Vampire Abilities");
            //Tooltip.SetDefault("Left-click to lunge and hold right-click to grab an enemy and suck their blood!\nSpecial: Switch the abilities used for right-click!");
        }

        public override void SafeSetDefaults()
        {
            item.damage = 51;
            item.width = 28;
            item.height = 30;
            item.useTime = 20;
            item.useAnimation = 20;
            item.consumable = false;
            item.noUseGraphic = true;
            item.maxStack = 1;
            item.knockBack = 12f;
            item.value = 0;
            item.rare = ItemRarityID.Blue;
        }

        public const int GrabAndSuck = 0;
        public const int SpacerRipperStingyEyes = 1;
        public const int ZombieMinionSummoning = 2;

        private int useCool = 0;
        private int lungeChargeTimer = 0;
        private bool enemyBeingGrabbed = false;
        private int heldEnemyIndex = -1;
        private int heldEnemyTimer = 0;
        private int eyeLaserChargeUpTimer = 0;
        private int zombieSummonTimer = 0;

        private int abilityNumber = 0;

        public override void HoldItem(Player player)
        {
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();
            VampirePlayer vPlayer = player.GetModPlayer<VampirePlayer>();
            if (player.whoAmI != item.owner || !vPlayer.vampire || (mPlayer.StandOut && !mPlayer.StandAutoMode))
                return;

            if (useCool > 0)
                useCool--;

            bool specialPressed = false;
            if (!Main.dedServ)
                specialPressed = JoJoStands.SpecialHotKey.JustPressed;

            if (specialPressed && player.whoAmI == item.owner)
            {
                abilityNumber++;
                if (abilityNumber >= 3)
                {
                    abilityNumber = 0;
                }

                if (abilityNumber == GrabAndSuck)
                {
                    Main.NewText("Ability: Blood Absorbtion");
                }
                if (abilityNumber == SpacerRipperStingyEyes)
                {
                    Main.NewText("Ability: Space Ripper Stingy Eyes");
                }
                if (abilityNumber == ZombieMinionSummoning)
                {
                    Main.NewText("Ability: Zombie Minion Summoning");
                }
            }

            vPlayer.enemyIgnoreItemInUse = true;
            if (Main.mouseLeft && useCool <= 0)
            {
                lungeChargeTimer++;
                if (lungeChargeTimer > 180)
                {
                    lungeChargeTimer = 180;
                }
            }
            if (!Main.mouseLeft && lungeChargeTimer > 0 && useCool <= 0)
            {
                useCool += item.useTime + (20 * (lungeChargeTimer / 30));
                int multiplier = lungeChargeTimer / 60;
                if (multiplier == 0)
                {
                    multiplier = 1;
                }
                if (Main.MouseWorld.X - player.position.X >= 0)
                {
                    player.direction = 1;
                }
                else
                {
                    player.direction = -1;
                }
                player.immune = true;
                player.immuneTime = 20;
                Vector2 launchVector = Main.MouseWorld - player.position;
                launchVector.Normalize();
                launchVector *= multiplier * 6;
                player.velocity += launchVector;
                Projectile.NewProjectile(player.Center, Vector2.Zero, mod.ProjectileType("VampiricSlash"), item.damage * multiplier, item.knockBack * multiplier, item.owner);
                Main.PlaySound(2, (int)player.position.X, (int)player.position.Y, 1, 1f, 0.2f);
                lungeChargeTimer = 0;
            }
            if (Main.mouseRight && useCool <= 0)
            {
                if (abilityNumber == GrabAndSuck)
                {
                    if (!enemyBeingGrabbed)
                    {
                        for (int n = 0; n < Main.maxNPCs; n++)
                        {
                            NPC npc = Main.npc[n];
                            if (npc.active)
                            {
                                if (player.Distance(npc.Center) <= 30f && !npc.boss && !npc.immortal && !npc.hide)
                                {
                                    enemyBeingGrabbed = true;
                                    heldEnemyIndex = npc.whoAmI;
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        NPC heldNPC = Main.npc[heldEnemyIndex];
                        if (vPlayer.enemyToIgnoreDamageFromIndex == -1 || !heldNPC.active)
                        {
                            vPlayer.enemyToIgnoreDamageFromIndex = -1;
                            enemyBeingGrabbed = false;
                            heldEnemyIndex = -1;
                            heldEnemyTimer = 0;
                            useCool += 120;
                            return;
                        }

                        player.controlUp = false;
                        player.controlDown = false;
                        player.controlLeft = false;
                        player.controlRight = false;
                        player.controlJump = false;
                        player.velocity = Vector2.Zero;
                        player.itemRotation = MathHelper.ToRadians(30f);

                        heldNPC.direction = -player.direction;
                        heldNPC.position = player.position + new Vector2(5f * player.direction, -2f - heldNPC.height / 3f);
                        heldNPC.velocity = Vector2.Zero;
                        vPlayer.enemyToIgnoreDamageFromIndex = heldNPC.whoAmI;

                        heldEnemyTimer++;
                        if (heldEnemyTimer >= 60)
                        {
                            int suckAmount = (int)(heldNPC.lifeMax * 0.08f);
                            player.HealEffect(suckAmount);
                            player.statLife += suckAmount;
                            heldNPC.StrikeNPC(suckAmount, 0f, player.direction);
                            Main.PlaySound(SoundID.Item, (int)player.position.X, (int)player.position.Y, 3, 1f, -0.8f);
                            heldEnemyTimer = 0;
                        }
                    }
                }
                else if (abilityNumber == SpacerRipperStingyEyes)
                {
                    eyeLaserChargeUpTimer++;
                    if (eyeLaserChargeUpTimer % 5 == 0)
                    {
                        int dustIndex = Dust.NewDust(player.Center + new Vector2(0f, -6f), 2, 2, 226, newColor: Color.MediumPurple);
                        Main.dust[dustIndex].noGravity = true;
                    }
                    if (eyeLaserChargeUpTimer >= 90)
                    {
                        if (eyeLaserChargeUpTimer % 15 == 0)
                        {
                            Vector2 shootVel = Main.MouseWorld - player.Center;
                            if (shootVel == Vector2.Zero)
                            {
                                shootVel = new Vector2(0f, 1f);
                            }
                            shootVel.Normalize();
                            shootVel *= 12f;
                            int proj = Projectile.NewProjectile(player.Center.X, player.Center.Y - 20f, shootVel.X, shootVel.Y, mod.ProjectileType("SpaceRipperStingyEyes"), 82, 4f, Main.myPlayer, 1f);
                            Main.projectile[proj].netUpdate = true;
                            Main.PlaySound(2, (int)player.position.X, (int)player.position.Y, 12, 1f, -2.4f);
                        }
                        if (eyeLaserChargeUpTimer >= 165)
                        {
                            eyeLaserChargeUpTimer = 0;
                        }
                    }
                }
                else if (abilityNumber == ZombieMinionSummoning)
                {
                    zombieSummonTimer++;
                    if (zombieSummonTimer >= 6 * 60)
                    {
                        Vector2 randomPosition = new Vector2(player.position.X + Main.rand.Next(-4 * 16, (4 * 16) + 1), player.position.Y - (Main.screenHeight / 2f));
                        Projectile.NewProjectile(randomPosition, Vector2.Zero, mod.ProjectileType("WarriorZombie"), 37, 3f, item.owner);
                        zombieSummonTimer = 0;
                    }
                    for (int d = 0; d < 11; d++)
                    {
                        int dustIndex = Dust.NewDust(player.Center + new Vector2(-2f, 8f), player.width, 2, DustID.Dirt);
                        Main.dust[dustIndex].noGravity = true;
                    }
                }
            }
            if (!Main.mouseRight)
            {
                enemyBeingGrabbed = false;
                heldEnemyIndex = -1;
                heldEnemyTimer = 0;
                vPlayer.enemyToIgnoreDamageFromIndex = -1;
                eyeLaserChargeUpTimer = 0;
                zombieSummonTimer = 0;
            }
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine tooltip = tooltips.FirstOrDefault(x => x.Name == "Damage" && x.mod == "Terraria");
            if (tooltip != null)
            {
                string[] splitText = tooltip.text.Split(' ');
                tooltip.text = splitText.First() + " Vampiric " + splitText.Last();
            }

            string rightClickMessage = "";
            if (abilityNumber == GrabAndSuck)
            {
                rightClickMessage = "hold right-click to grab an enemy and suck their blood!";
            }
            else if (abilityNumber == SpacerRipperStingyEyes)
            {
                rightClickMessage = "hold right-click to use Space Ripper Stingy Eyes!";
            }
            else if (abilityNumber == ZombieMinionSummoning)
            {
                rightClickMessage = "hold right-click to spawn zombie minions!";
            }
            TooltipLine tooltipAddition = new TooltipLine(mod, "Speed", "Left-click to lunge and " + rightClickMessage + "\nSpecial: Switch the abilities used for right-click!");
            tooltips.Add(tooltipAddition);
        }

        public override void AddRecipes()
        {
            VampirePlayer vPlayer = Main.player[item.owner].GetModPlayer<VampirePlayer>();
            VampiricItemRecipe recipe = new VampiricItemRecipe(mod, vPlayer.vampire);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}