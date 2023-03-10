using BrickAndMortar.Content.Buildings;
using System;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;

namespace BrickAndMortar.Core.Systems.BuildingSystem
{
	internal class BuildingSystem : ModSystem
	{
		public static List<Building> typeDummies = new();
		public static Dictionary<string, Building> typeDummyByName = new();

		public static List<Building> buildings = new();
		public static Dictionary<Point16, Building> buildingByPosition = new();

		public static void AddBuilding(Building toAdd)
		{
			buildings.Add(toAdd);
			buildingByPosition.Add(toAdd.position, toAdd);
		}

		public static void RemoveBuilding(Building toRemove)
		{
			buildings.Remove(toRemove);
			buildingByPosition.Remove(toRemove.position);
		}

		public static int GetWorldTier()
		{
			if (NPC.downedGolemBoss)
				return 4;

			if (NPC.downedMechBossAny)
				return 3;

			if (Main.hardMode)
				return 2;

			if (NPC.downedBoss2)
				return 1;

			return 0;
		}

		public static string GetErrorMessage(int neededTier)
		{
			return neededTier switch
			{
				1 => $"Defeat the Eater of Worlds or Brain of Cthulu to upgrade!",
				2 => "Defeat the Wall of Flesh to upgrade!",
				3 => "Defeat any mechanical boss to uupgrade!",
				4 => "Defeat the golem to upgrade!",
				5 => "Maximum reached!",
				_ => "An unknown error has occured.",
			};
		}

		public override void PostUpdateNPCs()
		{
			buildings.ForEach(n => n.levelBoost = 0); //reset

			buildings.ForEach(n => n.PrePassiveUpdate());

			buildings.ForEach(n =>
			{
				if (!n.IsTileValid(Framing.GetTileSafely(n.position)))
				{
					RemoveBuilding(n);
					return;
				}

				n.Update();
				n.UpdateBuildTimes();
			});
		}

		public override void SaveWorldData(TagCompound tag)
		{
			var buildingData = new List<TagCompound>();

			foreach (Building building in buildings)
			{
				var tag2 = new TagCompound();
				building.SaveData(tag2);
				buildingData.Add(tag2);
			}

			tag["buildings"] = buildingData;
		}

		public override void LoadWorldData(TagCompound tag)
		{
			buildings = new();
			buildingByPosition = new();

			IList<TagCompound> buildingData = tag.GetList<TagCompound>("buildings");

			foreach (TagCompound tag2 in buildingData)
			{
				var building = (Building)Activator.CreateInstance(BrickAndMortar.instance.Code.GetType(tag2.GetString("type")));
				building.LoadData(tag2);

				AddBuilding(building);
			}
		}
	}

	public class BuildingPlayer : ModPlayer
	{
		public override void UpdateEquips()
		{
			BuildingSystem.buildings.ForEach(n => n.PassiveBoost(Player));

			if (Player.trashItem.ModItem is BuildingItem) //Have to manually handle the trash slot here because its funny
				Player.trashItem.TurnToAir();
		}
	}
}
