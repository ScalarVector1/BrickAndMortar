using BrickAndMortar.Core.Systems.BuildingSystem;
using System.Linq;
using Terraria.ID;

namespace BrickAndMortar.Content.Buildings.Turret
{
	internal class Cannon : Building
	{
		private int timer;

		public int ProjectileDamage => (EffectiveLevel + 1) * 35;

		public float DamageBoost => (EffectiveLevel + 1) * 0.02f;

		public override string Name => "Cannon";

		public override string Info => "This magically powered cannon serves a dual purpose, pelting any nearby foes with flaming cannonballs, and empowering all of your damage.";

		public override int Width => 5;

		public override int Height => 5;

		public override void SetStatLines()
		{
			statlines.Add(new Statline(ProjectileDamage, 0, "Cannon damage", "BrickAndMortar/Assets/GUI/TowerDamage", new Color(255, 210, 210)));
			statlines.Add(new Statline(DamageBoost * 100, 0, "Damage boost %", "BrickAndMortar/Assets/GUI/Damage", new Color(255, 180, 180)));
		}

		public override void SetNextStatLines()
		{
			statlines.Add(new Statline(ProjectileDamage, (EffectiveLevel + 2) * 35, "Cannon damage", "BrickAndMortar/Assets/GUI/TowerDamage", new Color(255, 210, 210)));
			statlines.Add(new Statline(DamageBoost * 100, (EffectiveLevel + 2) * 2, "Damage boost %", "BrickAndMortar/Assets/GUI/Damage", new Color(255, 180, 180)));
		}

		public override void PassiveBoost(Player player)
		{
			player.GetDamage(DamageClass.Generic) += DamageBoost;
		}

		public override void Update()
		{
			System.Collections.Generic.IEnumerable<NPC> possibleTargets = Main.npc.Where(n => n.active && Vector2.Distance(n.Center, Center) < 1000);

			if (possibleTargets.Count() > 0)
			{
				timer++;

				if (timer % 120 == 0)
					Projectile.NewProjectile(null, Center, Vector2.Normalize(possibleTargets.First().Center - Center) * 7, ModContent.ProjectileType<Cannonball>(), ProjectileDamage, 1, Main.myPlayer);
			}
		}

		public override int GetAurumCost()
		{
			return (level + 1) * 950;
		}

		public override long GetBuildTime()
		{
			return (level + 1) * 600;
		}

		public override int GetBuildCount()
		{
			return BuildingSystem.GetWorldTier() + 1;
		}
	}

	internal class Cannonball : ModProjectile
	{
		public override string Texture => "BrickAndMortar/Assets/Misc/DotTell";

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Cannonball");
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 45;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
		}

		public override void SetDefaults()
		{
			Projectile.width = 32;
			Projectile.height = 32;
			Projectile.friendly = true;
			Projectile.extraUpdates = 2;
		}

		public override void Kill(int timeLeft)
		{
			Terraria.Audio.SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);

			for (int k = 0; k < 15; k++)
			{
				Dust.NewDustPerfect(Projectile.Center, DustID.GoldFlame, (-Projectile.velocity * 2 * Main.rand.NextFloat(0f, 0.4f)).RotatedByRandom(1.2f), 0, default, 1.5f);
				Dust.NewDustPerfect(Projectile.Center, DustID.Iron, (-Projectile.velocity * 2 * Main.rand.NextFloat(0f, 0.1f)).RotatedByRandom(0.45f), 0, default, 1.5f);
			}
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D tex = ModContent.Request<Texture2D>("BrickAndMortar/Assets/Misc/DotTell").Value;

			for (int k = 0; k < Projectile.oldPos.Length; k++)
			{
				Color color = Color.Lerp(Color.Orange, new Color(255, 255, 100), (Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);

				if (k <= 4)
					color *= 1.2f;

				color.A = 0;

				color *= Projectile.damage / 300f;

				float scale = Projectile.scale * (Projectile.oldPos.Length - k) / Projectile.oldPos.Length * 0.6f;

				Main.spriteBatch.Draw(tex, (Projectile.oldPos[k] + Projectile.Size / 2 + Projectile.Center) * 0.5f - Main.screenPosition, null, color, 0, tex.Size() / 2, scale, default, default);
			}

			Color glowColor = new Color(255, 255, 100) * (Projectile.damage / 300f) * 0.5f;
			glowColor.A = 0;

			Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, glowColor, 0, tex.Size() / 2, 1, 0, 0);

			Texture2D tex2 = Terraria.GameContent.TextureAssets.Item[929].Value;
			Main.spriteBatch.Draw(tex2, Projectile.Center - Main.screenPosition, null, Color.White, 0, tex2.Size() / 2, 1, 0, 0);

			Lighting.AddLight(Projectile.Center, new Vector3(0.1f, 0.1f, 0.05f));

			return false;
		}
	}
}
