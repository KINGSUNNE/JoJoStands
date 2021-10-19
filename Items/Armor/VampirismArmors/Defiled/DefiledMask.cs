﻿using JoJoStands.Items.Vampire;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace JoJoStands.Items.Armor.VampirismArmors.Defiled
{
    [AutoloadEquip(EquipType.Head)]
    public class DefiledMask : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("A mask created with the rotten chunks of your victims.\n+8% Vampiric Damage");
        }

        public override void SetDefaults()
        {
            item.width = 22;
            item.height = 18;
            item.value = Item.buyPrice(silver: 80);
            item.rare = ItemRarityID.Orange;
            item.defense = 6;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == mod.ItemType("DefiledChestplate") && legs.type == mod.ItemType("DefiledGreaves");
        }

        public override void UpdateArmorSet(Player player)
        {
            VampirePlayer vPlayer = player.GetModPlayer<VampirePlayer>();
            player.setBonus = "45% Sunburn Damage Reduction\n+25% Movement Speed";
            vPlayer.sunburnDamageMultiplier *= 0.55f;
            player.moveSpeed *= 1.25f;

            if (Main.rand.Next(0, 3 + 1) == 0)
            {
                int newDust = Dust.NewDust(player.position, player.width, player.height, 17);
                Main.dust[newDust].noGravity = true;
            }
        }

        public override void UpdateEquip(Player player)
        {
            VampirePlayer vPlayer = player.GetModPlayer<VampirePlayer>();
            vPlayer.vampiricDamageMultiplier += 0.08f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.DemoniteBar, 11);
            recipe.AddIngredient(ItemID.RottenChunk, 15);
            recipe.AddIngredient(ItemID.ShadowScale, 6);
            recipe.AddIngredient(ItemID.WormTooth, 2);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}