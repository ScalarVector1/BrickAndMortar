using BrickAndMortar.Core.Systems.BuildingSystem;
using BrickAndMortar.Core.Systems.ResourceSystem;
using Terraria.ID;
using Terraria.ModLoader.IO;

namespace BrickAndMortar.Content.Buildings.Resources
{
	internal class AurumMine : Building
	{
		int aurumStored;

		int AurumMax => (EffectiveLevel + 1) * 1000;

		int ProductionRate => (EffectiveLevel + 1) * 600;

		public override string Name => "AurumMine";

		public override string FriendlyName => "Aurum mine";

		public override string Info => "The aurum mine collects aurum automatically over time, but can only hold a limited amount. Be sure to regularly check in and collect it to prevent it's production from going to waste!";

		public override int Width => 3;

		public override int Height => 3;

		public override void PrePassiveUpdate()
		{
			int interval = 216000 / ProductionRate;

			if (aurumStored < AurumMax && Main.GameUpdateCount % interval == 0)
				aurumStored++;
		}

		public override void PassiveBoost(Player player)
		{
			if (player.Hitbox.Intersects(new Rectangle(position.X * 16, position.Y * 16, Width * 16, Height * 16)))
			{
				int granted = ResourceDropHelper.GrantAurum(Center, player, aurumStored);

				if (granted > 0)
				{
					aurumStored -= granted;
					CombatText.NewText(new Rectangle((int)Center.X, (int)Center.Y, 1, 1), new Color(255, 255, 200), granted);
					Terraria.Audio.SoundEngine.PlaySound(SoundID.CoinPickup);
				}
			}
		}

		public override void SetStatLines()
		{
			statlines.Add(new Statline(AurumMax, 0, "Aurum capacity", "BrickAndMortar/Assets/GUI/AurumIcon", new Color(255, 255, 230)));
			statlines.Add(new Statline(ProductionRate, 0, "Aurum per hour", "BrickAndMortar/Assets/GUI/AurumIcon", new Color(255, 255, 200)));
		}

		public override void SetNextStatLines()
		{
			statlines.Add(new Statline(AurumMax, (EffectiveLevel + 2) * 1000, "Aurum capacity", "BrickAndMortar/Assets/GUI/AurumIcon", new Color(255, 255, 230)));
			statlines.Add(new Statline(ProductionRate, (EffectiveLevel + 2) * 600, "Aurum per hour", "BrickAndMortar/Assets/GUI/AurumIcon", new Color(255, 255, 200)));
		}

		public override int GetLifeforceCost()
		{
			return (level + 1) * 300;
		}

		public override long GetBuildTime()
		{
			return 60 * 5 * (level + 1);
		}

		public override int GetBuildCount()
		{
			return (BuildingSystem.GetWorldTier() + 1) * 2;
		}

		public override void SaveData(TagCompound tag)
		{
			tag["aurum"] = aurumStored;
			base.SaveData(tag);
		}

		public override void LoadData(TagCompound tag)
		{
			aurumStored = tag.GetInt("aurum");
			base.LoadData(tag);
		}
	}
}
