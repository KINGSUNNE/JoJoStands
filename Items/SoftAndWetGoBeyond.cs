using JoJoStands.Items.CraftingMaterials;
using JoJoStands.Projectiles.PlayerStands.SoftAndWetGoBeyond;
using JoJoStands.Tiles;
using Terraria.Audio;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoStands.Items
{
    public class SoftAndWetGoBeyond : StandItemClass
    {
        public override int standSpeed => 9;
        public override int standType => 1;
        public override string standProjectileName => "SoftAndWet";
        public override int standTier => 5;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Soft and Wet (Go Beyond!)");
            Tooltip.SetDefault("Punch enemies at a fast rate and right-click to create a non-existent bubble!\nSpecial: Explosive Spin!\nRight-Click while holding the Item to revert back to Soft & Wet Final (You can revert back to Go Beyond!)");
        }
        public override string Texture
        {
            get { return Mod.Name + "/Items/SoftAndWetT1"; }
        }
        public override void SetDefaults()
        {
            Item.damage = 118;
            Item.width = 32;
            Item.height = 32;
            Item.noUseGraphic = true;
            Item.maxStack = 1;
            Item.value = 0;
            Item.rare = ItemRarityID.LightPurple;
        }

        public override void HoldItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer && Main.mouseRight)
            {
                MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();
                mPlayer.canRevertFromGoBeyond = true;
                if (mPlayer.revertTimer <= 0)
                {
                    Item.type = ModContent.ItemType<SoftAndWetFinal>();
                    Item.SetDefaults(ModContent.ItemType<SoftAndWetFinal>());
                    SoundEngine.PlaySound(SoundID.Grab);
                    mPlayer.revertTimer += 30;
                }
            }
        }
        public override bool ManualStandSpawning(Player player)
        {
            Projectile.NewProjectile(player.GetSource_FromThis(), player.position, player.velocity, ModContent.ProjectileType<SoftAndWetGoBeyondStand>(), 0, 0f, Main.myPlayer);
            return true;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<RequiemArrow>())
                .AddIngredient(ModContent.ItemType<SoftAndWetFinal>())
                .AddIngredient(ModContent.ItemType<KillerQueenFinal>())
                .AddIngredient(ModContent.ItemType<RighteousLifeforce>())
                .AddTile(ModContent.TileType<RemixTableTile>())
                .Register();
        }
    }
}
