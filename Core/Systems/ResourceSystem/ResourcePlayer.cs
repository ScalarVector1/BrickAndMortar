using Terraria.ModLoader.IO;

namespace BrickAndMortar.Core.Systems.ResourceSystem
{
	internal class ResourcePlayer : ModPlayer
	{
		public int aurumAmount;
		public int lifeforceAmount;

		public int storageLevel;

		public float AurumPercent => aurumAmount / GetMaxFromStorageLevel();
		public float LifeforcePercent => lifeforceAmount / GetMaxFromStorageLevel();

		/// <summary>
		/// Gets a players maximum storage amount for both resources based on their storage level
		/// </summary>
		/// <returns></returns>
		public int GetMaxFromStorageLevel()
		{
			return 1000 + storageLevel * 500;
		}

		public int GetStorageUpgradeCost()
		{
			return (storageLevel + 1) * 400;
		}

		public override void SaveData(TagCompound tag)
		{
			tag["aurum"] = aurumAmount;
			tag["lifeforce"] = lifeforceAmount;
			tag["storageLevel"] = storageLevel;
		}

		public override void LoadData(TagCompound tag)
		{
			aurumAmount = tag.GetInt("aurum");
			lifeforceAmount = tag.GetInt("lifeforce");
			storageLevel = tag.GetInt("storageLevel");
		}
	}
}
