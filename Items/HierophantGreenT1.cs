using JoJoStands.Items.CraftingMaterials;
using JoJoStands.Tiles;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoStands.Items
{
    public class HierophantGreenT1 : StandItemClass
    {
        public override int standSpeed => 40;
        public override int standType => 2;
        public override string standProjectileName => "HierophantGreen";
        public override int standTier => 1;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hierophant Green (Tier 1)");
            Tooltip.SetDefault("Left-click to release a flurry of emeralds!\nUsed in Stand Slot");
        }

        public override void SetDefaults()
        {
            Item.damage = 12;
            Item.width = 32;
            Item.height = 32;
            Item.maxStack = 1;
            Item.value = 0;
            Item.noUseGraphic = true;
            Item.rare = ItemRarityID.LightPurple;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<StandArrow>())
                .AddIngredient(ModContent.ItemType<WillToProtect>())
                .AddTile(ModContent.TileType<RemixTableTile>())
                .Register();
        }
    }
}
