using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoStands.Projectiles
{
    public class BadCompanyTankRocket : ModProjectile
    {
        public override string Texture
        {
            get { return mod.Name + "/Projectiles/AerosmithBomb"; }
        }

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 14;
            projectile.aiStyle = 0;
            projectile.timeLeft = 300;
            projectile.friendly = true;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
        }

        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.ToRadians(45f);
            projectile.velocity.Y += 0.1f;
        }

        public override void Kill(int timeLeft)
        {
            Player player = Main.player[projectile.owner];
            MyPlayer Mplayer = player.GetModPlayer<MyPlayer>();
            var explosion = Projectile.NewProjectile(projectile.position, projectile.velocity, ProjectileID.GrenadeIII, (int)(projectile.ai[0] * Mplayer.standDamageBoosts), 8f, projectile.owner);
            Main.projectile[explosion].timeLeft = 2;
            Main.projectile[explosion].netUpdate = true;
            Main.PlaySound(SoundID.Item62);
        }
    }
}