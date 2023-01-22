using BrickAndMortar.Core.Systems.BuildingSystem;
using Terraria.ModLoader.IO;

namespace BrickAndMortar.Content.Buildings.Support
{
	internal class Pylon : Building
	{
		private enum buffMode
		{
			Melee = 0,
			Ranged = 1,
			Magic = 2,
			Summon = 3,
			Crit = 4,
			Defense = 5,
			Life = 6
		}

		buffMode mode;

		public override string Name => "Pylon";

		public override string Info => "The pylon provides a powerful stat buff to you of your choice! The pylon always effects you, nomatter where you are. You can change the buff it grants at any time.";

		public override int Width => 6;

		public override int Height => 6;

		public override bool HasTertiaryButton => true;

		public override void OnTertiaryButtonClick()
		{
			mode = (buffMode)((int)(mode + 1) % 7);
		}

		public override void PassiveBoost(Player player)
		{
			switch (mode) //These are a bit ugly but this is never really re-used
			{
				case buffMode.Melee:
					player.GetDamage(DamageClass.Melee) += 0.05f * (EffectiveLevel + 1);
					break;

				case buffMode.Ranged:
					player.GetDamage(DamageClass.Ranged) += 0.05f * (EffectiveLevel + 1);
					break;

				case buffMode.Magic:
					player.GetDamage(DamageClass.Magic) += 0.05f * (EffectiveLevel + 1);
					break;

				case buffMode.Summon:
					player.GetDamage(DamageClass.Summon) += 0.05f * (EffectiveLevel + 1);
					break;

				case buffMode.Crit:
					player.GetCritChance(DamageClass.Generic) += 3 * (EffectiveLevel + 1);
					break;

				case buffMode.Defense:
					player.statDefense += 5 * (EffectiveLevel + 1);
					break;

				case buffMode.Life:
					player.statLifeMax2 += 25 * (EffectiveLevel + 1);
					break;
			}
		}

		public override void SetStatLines()
		{
			switch (mode)
			{
				case buffMode.Melee:
					statlines.Add(new Statline(5 * (EffectiveLevel + 1), 0, "Melee damage boost %", "BrickAndMortar/Assets/GUI/Damage", new Color(255, 200, 200)));
					break;

				case buffMode.Ranged:
					statlines.Add(new Statline(5 * (EffectiveLevel + 1), 0, "Ranged damage boost %", "BrickAndMortar/Assets/GUI/Damage", new Color(200, 255, 200)));
					break;

				case buffMode.Magic:
					statlines.Add(new Statline(5 * (EffectiveLevel + 1), 0, "Magic damage boost %", "BrickAndMortar/Assets/GUI/Damage", new Color(255, 200, 255)));
					break;

				case buffMode.Summon:
					statlines.Add(new Statline(5 * (EffectiveLevel + 1), 0, "Summon damage boost %", "BrickAndMortar/Assets/GUI/Damage", new Color(200, 255, 255)));
					break;

				case buffMode.Crit:
					statlines.Add(new Statline(3 * (EffectiveLevel + 1), 0, "Critical strike chance boost", "BrickAndMortar/Assets/GUI/Damage", new Color(255, 200, 150)));
					break;

				case buffMode.Defense:
					statlines.Add(new Statline(5 * (EffectiveLevel + 1), 0, "Defense boost", "BrickAndMortar/Assets/GUI/Defense", new Color(200, 200, 200)));
					break;

				case buffMode.Life:
					statlines.Add(new Statline(25 * (EffectiveLevel + 1), 0, "Life boost", "BrickAndMortar/Assets/GUI/Life", new Color(255, 150, 200)));
					break;
			}
		}

		public override void SetNextStatLines()
		{
			switch (mode)
			{
				case buffMode.Melee:
					statlines.Add(new Statline(5 * (EffectiveLevel + 1), 5 * (EffectiveLevel + 2), "Melee damage boost %", "BrickAndMortar/Assets/GUI/Damage", new Color(255, 200, 200)));
					break;

				case buffMode.Ranged:
					statlines.Add(new Statline(5 * (EffectiveLevel + 1), 5 * (EffectiveLevel + 2), "Ranged damage boost %", "BrickAndMortar/Assets/GUI/Damage", new Color(200, 255, 200)));
					break;

				case buffMode.Magic:
					statlines.Add(new Statline(5 * (EffectiveLevel + 1), 5 * (EffectiveLevel + 2), "Magic damage boost %", "BrickAndMortar/Assets/GUI/Damage", new Color(255, 200, 255)));
					break;

				case buffMode.Summon:
					statlines.Add(new Statline(5 * (EffectiveLevel + 1), 5 * (EffectiveLevel + 2), "Summon damage boost %", "BrickAndMortar/Assets/GUI/Damage", new Color(200, 255, 255)));
					break;

				case buffMode.Crit:
					statlines.Add(new Statline(3 * (EffectiveLevel + 1), 3 * (EffectiveLevel + 2), "Critical strike chance boost", "BrickAndMortar/Assets/GUI/Damage", new Color(255, 200, 150)));
					break;

				case buffMode.Defense:
					statlines.Add(new Statline(5 * (EffectiveLevel + 1), 5 * (EffectiveLevel + 2), "Defense boost", "BrickAndMortar/Assets/GUI/Defense", new Color(200, 200, 200)));
					break;

				case buffMode.Life:
					statlines.Add(new Statline(25 * (EffectiveLevel + 1), 25 * (EffectiveLevel + 2), "Life boost", "BrickAndMortar/Assets/GUI/Life", new Color(255, 150, 200)));
					break;
			}
		}

		public override int GetLifeforceCost()
		{
			return (level + 1) * 2000;
		}

		public override long GetBuildTime()
		{
			return 600 * 2 * (level + 1);
		}

		public override int GetBuildCount()
		{
			return BuildingSystem.GetWorldTier();
		}

		public override void SaveData(TagCompound tag)
		{
			tag["mode"] = (int)mode;
			base.SaveData(tag);
		}

		public override void LoadData(TagCompound tag)
		{
			mode = (buffMode)tag.GetInt("mode");
			base.LoadData(tag);
		}
	}
}
