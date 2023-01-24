using BrickAndMortar.Core.Systems.BuildingSystem;
using System.Linq;
using Terraria.ID;

namespace BrickAndMortar.Content.Buildings.Support
{
	internal class Bonfire : Building
	{
		public int RegenAura => (EffectiveLevel + 1) * 5;

		public int RegenBoost => (EffectiveLevel + 1) * 2;

		public override string Name => "Bonfire";

		public override string Info => "The soft glow of the bonfire will heal you slowly -- not only as you stand near it, but anywhere in the world at a reduced rate. Nearby townsfolk will also be restored by it's healing warmpth.";

		public override int Width => 5;

		public override int Height => 3;

		public override void SetStatLines()
		{
			statlines.Add(new Statline(RegenAura, 0, "Life regeneration aura", "BrickAndMortar/Assets/GUI/TowerDamage", new Color(255, 100, 200)));
			statlines.Add(new Statline(RegenBoost, 0, "Life regeneration boost", "BrickAndMortar/Assets/GUI/Damage", new Color(255, 150, 200)));
		}

		public override void SetNextStatLines()
		{
			statlines.Add(new Statline(RegenAura, (EffectiveLevel + 2) * 5, "Life regeneration aura", "BrickAndMortar/Assets/GUI/TowerDamage", new Color(255, 100, 200)));
			statlines.Add(new Statline(RegenBoost, (EffectiveLevel + 2) * 2, "Life regeneration boost", "BrickAndMortar/Assets/GUI/Damage", new Color(255, 150, 200)));
		}

		public override void PassiveBoost(Player player)
		{
			player.lifeRegen += RegenBoost;

			if (Vector2.Distance(player.Center, Center) < 500)
			{
				player.lifeRegen += RegenAura;

				if (Main.rand.NextBool(5))
					Dust.NewDustPerfect(player.Center + Main.rand.NextVector2Circular(16, 32), DustID.HealingPlus, Vector2.UnitY * -1);
			}
		}

		public override void Update()
		{
			Lighting.AddLight(Center, new Vector3(1f, 0.8f, 0.4f));

			foreach (NPC npc in Main.npc.Where(n => n.active && n.friendly))
				npc.lifeRegen += RegenAura;
		}

		public override int GetAurumCost()
		{
			return (level + 1) * 800;
		}

		public override long GetBuildTime()
		{
			return (level + 1) * 600;
		}

		public override int GetBuildCount()
		{
			return
				BuildingSystem.GetWorldTier() >= 2 ? 2 :
				BuildingSystem.GetWorldTier() >= 0 ? 1 :
				0;
		}
	}
}
