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

				if (timer % 3600 == 0)
					Projectile.NewProjectile(null, Center, Vector2.Normalize(possibleTargets.First().Center - Center), ProjectileID.WoodenArrowFriendly, ProjectileDamage, 1, Main.myPlayer);
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
}
