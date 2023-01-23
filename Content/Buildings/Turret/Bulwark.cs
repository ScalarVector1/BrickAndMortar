using BrickAndMortar.Core.Systems.BuildingSystem;
using System;
using System.Linq;

namespace BrickAndMortar.Content.Buildings.Turret
{
	internal class Bulwark : Building
	{
		int timer;

		public int PushPower => EffectiveLevel + 1;

		public float EnduranceBoost => (EffectiveLevel + 1) * 0.1f;

		public override string Name => "Bulwark";

		public override string Info => "Bulwarks are small but dependable fortifications, for your base and body. They will repel nearby enemies from themselves, as well as toughening your body a tiny bit. While each individual bulwark has minimal impact, they have strength in numbers!";

		public override int Width => 1;

		public override int Height => 1;

		public override void SetStatLines()
		{
			statlines.Add(new Statline(PushPower, 0, "Push power", "BrickAndMortar/Assets/GUI/TowerDamage", new Color(255, 255, 255)));
			statlines.Add(new Statline(EnduranceBoost, 0, "Damage reduction boost", "BrickAndMortar/Assets/GUI/Defense", new Color(200, 200, 200)));
		}

		public override void SetNextStatLines()
		{
			statlines.Add(new Statline(PushPower, EffectiveLevel + 2, "Push power", "BrickAndMortar/Assets/GUI/TowerDamage", new Color(255, 255, 255)));
			statlines.Add(new Statline(EnduranceBoost, (EffectiveLevel + 2) * 0.1f, "Damage reduction boost", "BrickAndMortar/Assets/GUI/Defense", new Color(200, 200, 200)));
		}

		public override void PassiveBoost(Player player)
		{
			player.endurance += EnduranceBoost * 0.01f;
		}

		public override void Update()
		{
			System.Collections.Generic.IEnumerable<NPC> possibleTargets = Main.npc.Where(n => n.active && n.Hitbox.Intersects(new Rectangle(position.X * 16, position.Y * 16, 16, 16)));

			if (possibleTargets.Count() > 0 && timer <= 0)
			{
				foreach (NPC npc in possibleTargets)
				{
					npc.velocity += Vector2.Normalize(npc.Center - Center) * PushPower;
				}

				timer = 180;
			}

			if (timer > 0)
				timer--;
		}

		public override int GetAurumCost()
		{
			return 100 * (int)Math.Pow(2, level + 1);
		}

		public override int GetLifeforceCost()
		{
			return 100 * (int)Math.Pow(2, level + 1);
		}

		public override long GetBuildTime()
		{
			return 0;
		}

		public override int GetBuildCount()
		{
			return (BuildingSystem.GetWorldTier() + 1) * 25;
		}
	}
}
