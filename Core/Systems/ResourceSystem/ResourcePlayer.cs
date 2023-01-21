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
	}
}
