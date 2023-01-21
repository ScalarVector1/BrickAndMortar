using BrickAndMortar.Core.Systems.BuildingSystem;
using Terraria.ModLoader.IO;

namespace BrickAndMortar.Content.Buildings.Resources
{
	internal class LifeforceVat : Building
	{
		int lifeforceStored;

		int LifeforceMax => (EffectiveLevel + 1) * 250;

		int ProductionRate => (EffectiveLevel + 1) * 150;

		public override string Name => "LifeforceVat";

		public override string FriendlyName => "Lifeforce vat";

		public override string Info => "The lifeforce vat slowly accumulates lifeforce over time. While the vat is much slower than the aurum mine, slaying enemies grants almost twice as much lifeforce as it does aurum.";

		public override int Width => 3;

		public override int Height => 3;

		public override void PrePassiveUpdate()
		{
			int interval = 216000 / ProductionRate;

			if (lifeforceStored < LifeforceMax && Main.GameUpdateCount % interval == 0)
				lifeforceStored++;
		}

		public override void SetStatLines()
		{
			statlines.Add(new Statline(LifeforceMax, 0, "Lifeforce capacity", "BrickAndMortar/Assets/GUI/LifeforceIcon", new Color(230, 255, 255)));
			statlines.Add(new Statline(ProductionRate, 0, "Lifeforce per hour", "BrickAndMortar/Assets/GUI/LifeforceIcon", new Color(200, 255, 255)));
		}

		public override void SetNextStatLines()
		{
			statlines.Add(new Statline(LifeforceMax, (EffectiveLevel + 2) * 250, "Lifeforce capacity", "BrickAndMortar/Assets/GUI/LifeforceIcon", new Color(230, 255, 255)));
			statlines.Add(new Statline(ProductionRate, (EffectiveLevel + 2) * 150, "Lifeforce per hour", "BrickAndMortar/Assets/GUI/LifeforceIcon", new Color(210, 255, 255)));
		}

		public override int GetAurumCost()
		{
			return (level + 1) * 300;
		}

		public override long GetBuildTime()
		{
			return 60 * 5 * (level + 1);
		}

		public override int GetBuildCount()
		{
			return BuildingSystem.GetWorldTier() * 2;
		}

		public override void SaveData(TagCompound tag)
		{
			tag["lifeforce"] = lifeforceStored;
			base.SaveData(tag);
		}

		public override void LoadData(TagCompound tag)
		{
			lifeforceStored = tag.GetInt("lifeforce");
			base.LoadData(tag);
		}
	}
}
