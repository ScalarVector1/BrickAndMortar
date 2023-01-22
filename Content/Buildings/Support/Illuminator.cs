using BrickAndMortar.Core.Systems.BuildingSystem;

namespace BrickAndMortar.Content.Buildings.Support
{
	internal class Illuminator : Building
	{
		public float LightPower => 1 + EffectiveLevel * 0.25f;

		public float CritBoost => (EffectiveLevel + 1) * 0.01f;

		public override string Name => "Illuminator";

		public override string Info => "This extra-bright monolith will light your way -- not only letting you see in the dark, but also letting you better spot the weak points of your enemies anywhere you are.";

		public override int Width => 3;

		public override int Height => 10;

		public override void SetStatLines()
		{
			statlines.Add(new Statline(LightPower, 0, "Light strength", "BrickAndMortar/Assets/GUI/TowerDamage", new Color(255, 255, 200)));
			statlines.Add(new Statline(CritBoost * 100, 0, "Critical strike chance boost", "BrickAndMortar/Assets/GUI/Damage", new Color(255, 200, 150)));
		}

		public override void SetNextStatLines()
		{
			statlines.Add(new Statline(LightPower, 1 + (EffectiveLevel + 1) * 0.25f, "Light strength", "BrickAndMortar/Assets/GUI/TowerDamage", new Color(255, 255, 200)));
			statlines.Add(new Statline(CritBoost * 100, EffectiveLevel + 2, "Critical strike chance boost", "BrickAndMortar/Assets/GUI/Damage", new Color(255, 200, 150)));
		}

		public override void PassiveBoost(Player player)
		{
			player.GetCritChance(DamageClass.Generic) += CritBoost;
		}

		public override void Update()
		{
			Lighting.AddLight(Center + new Vector2(0, -80), new Vector3(1, 1, 0.9f) * LightPower);
		}

		public override int GetAurumCost()
		{
			return (level + 1) * 1200;
		}

		public override long GetBuildTime()
		{
			return (level + 1) * 600;
		}

		public override int GetBuildCount()
		{
			return
				BuildingSystem.GetWorldTier() >= 3 ? 2 :
				BuildingSystem.GetWorldTier() >= 1 ? 1 :
				0;
		}
	}
}
