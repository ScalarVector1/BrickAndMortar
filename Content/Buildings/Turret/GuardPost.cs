using BrickAndMortar.Core.Systems.BuildingSystem;
using System.Collections.Generic;
using System.Linq;

namespace BrickAndMortar.Content.Buildings.Turret
{
	internal class GuardPost : Building
	{
		private readonly List<Projectile> minions = new();

		public int MinionCount => (EffectiveLevel + 1) / 3 + 1;

		public int MinionDamage => (EffectiveLevel + 1) * 20;

		public int DefenseBoost => EffectiveLevel + 1;

		public override string Name => "GuardPost";

		public override string FriendlyName => "Guard post";

		public override string Info => "The guard post is the ultimate fortification, acting as a base of operations for steadfast guardians who will tirelessly fight to slay monsters who venture too close. Their incredible defensive spirit is even strong enough to empower your own defense!";

		public override int Width => 4;

		public override int Height => 8;

		public override void SetStatLines()
		{
			statlines.Add(new Statline(MinionCount, 0, "Guard quantity", "BrickAndMortar/Assets/GUI/TowerDamage", new Color(255, 100, 200)));
			statlines.Add(new Statline(MinionDamage, 0, "Guard power", "BrickAndMortar/Assets/GUI/TowerDamage", new Color(230, 230, 230)));
			statlines.Add(new Statline(DefenseBoost, 0, "Defense boost", "BrickAndMortar/Assets/GUI/Defense", new Color(200, 200, 200)));
		}

		public override void SetNextStatLines()
		{
			statlines.Add(new Statline(MinionCount, (EffectiveLevel + 2) / 3 + 1, "Guard quantity", "BrickAndMortar/Assets/GUI/TowerDamage", new Color(255, 100, 200)));
			statlines.Add(new Statline(MinionDamage, (EffectiveLevel + 2) * 20, "Guard power", "BrickAndMortar/Assets/GUI/TowerDamage", new Color(230, 230, 230)));
			statlines.Add(new Statline(DefenseBoost, EffectiveLevel + 2, "Defense boost", "BrickAndMortar/Assets/GUI/Defense", new Color(200, 200, 200)));
		}

		public override void PassiveBoost(Player player)
		{
			player.statDefense += DefenseBoost;
		}

		public override void Update()
		{
			minions.RemoveAll(n => !n.active || n is null);

			IEnumerable<NPC> possibleTargets = Main.npc.Where(n => n.active && Vector2.Distance(n.Center, Center) < 1000);

			if (possibleTargets.Count() > 0)
			{
				while (minions.Count < MinionCount)
				{
					int i = Projectile.NewProjectile(null, Center, Vector2.Zero, ModContent.ProjectileType<Guard>(), MinionDamage, 0, Main.myPlayer);

					var mp = Main.projectile[i].ModProjectile as Guard;
					mp.parent = this;

					minions.Add(Main.projectile[i]);
				}
			}
		}

		public override int GetAurumCost()
		{
			return (level + 1) * 1000;
		}

		public override long GetBuildTime()
		{
			return (level + 1) * 900;
		}

		public override int GetBuildCount()
		{
			return
				BuildingSystem.GetWorldTier() >= 2 ? 4 :
				BuildingSystem.GetWorldTier() >= 0 ? 2 :
				0;
		}
	}

	internal class Guard : ModProjectile
	{
		private NPC target;

		public Building parent;

		public Vector2 moveTarget;

		public override string Texture => "BrickAndMortar/Assets/Buildings/Guard";

		public override void SetDefaults()
		{
			Projectile.friendly = true;
			Projectile.timeLeft = 2;
			Projectile.width = 32;
			Projectile.height = 48;
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			return false;
		}

		public override void AI()
		{
			Projectile.timeLeft = 2;

			if (Projectile.velocity.Y < 5)
				Projectile.velocity.Y += 0.4f;

			if (target is null || !target.active || Vector2.Distance(target.Center, parent.Center) > 1000)
			{
				IEnumerable<NPC> possibleTargets = Main.npc.Where(n => n.active && Vector2.Distance(n.Center, parent.Center) < 1000);

				if (possibleTargets.Count() <= 0)
					target = null;

				target = possibleTargets.ElementAt(Main.rand.Next(possibleTargets.Count()));
			}

			if (target is null)
				moveTarget = parent.Center + Vector2.UnitY * parent.Height * 8;
			else
				moveTarget = target.Center;

			Projectile.velocity.X += Vector2.Normalize(Projectile.Center - moveTarget).X * 0.1f;
			Projectile.velocity.X *= 0.95f;

			if (moveTarget.Y < Projectile.Center.Y && Projectile.velocity.Y == 0)
				Projectile.velocity.Y -= 5;
		}
	}
}
