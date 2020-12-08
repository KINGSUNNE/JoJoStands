﻿using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoStands.Items
{
	public class SilverChariotT2 : StandItemClass
	{
		public override int standSpeed => 7;
		public override int standType => 1;

		public override string Texture
		{
			get { return mod.Name + "/Items/SilverChariotT1"; }
		}

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Silver Chariot (Tier 2)");
			Tooltip.SetDefault("stab n shoot.\nUsed in Stand Slot");
		}

		public override void SetDefaults()
		{
			item.damage = 46;
			item.width = 32;
			item.height = 32;
			item.noUseGraphic = true;
			item.maxStack = 1;
			item.value = 0;
			item.rare = ItemRarityID.LightPurple;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(mod.ItemType("SilverChariotT1"));
			recipe.AddIngredient(ItemID.PlatinumBar, 12);
			recipe.AddIngredient(ItemID.Hellstone, 20);
			recipe.AddIngredient(ItemID.Amethyst, 2);
			recipe.AddIngredient(ItemID.FallenStar, 5);
			recipe.AddIngredient(mod.ItemType("WillToFight"), 2);
			recipe.AddIngredient(mod.ItemType("WillToProtect"));
			recipe.AddTile(mod.TileType("RemixTableTile"));
			recipe.SetResult(this);
			recipe.AddRecipe();
			recipe = new ModRecipe(mod);
			recipe.AddIngredient(mod.ItemType("SilverChariotT1"));
			recipe.AddIngredient(ItemID.GoldBar, 12);
			recipe.AddIngredient(ItemID.Hellstone, 20);
			recipe.AddIngredient(ItemID.Amethyst, 2);
			recipe.AddIngredient(ItemID.FallenStar, 5);
			recipe.AddIngredient(mod.ItemType("WillToFight"), 2);
			recipe.AddIngredient(mod.ItemType("WillToProtect"));
			recipe.AddTile(mod.TileType("RemixTableTile"));
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}
