using BrickAndMortar.Content.Buildings;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;

namespace BrickAndMortar.Core.Systems.BuildingSystem
{
	internal class BuildingSystem : ModSystem
	{
		public static Dictionary<string, Building> typeDummies = new();

		public static List<Building> buildings = new();

		public static Dictionary<Point16, Building> byPosition = new();

		public static void AddBuilding(Building toAdd)
		{
			buildings.Add(toAdd);
			byPosition.Add(toAdd.position, toAdd);
		}

		public static void RemoveBuilding(Building toRemove)
		{
			buildings.Remove(toRemove);
			byPosition.Remove(toRemove.position);
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
				1 => $"Defeat the Eater of Worlds or Brain of Cthulu to unlock this!",
				2 => "Defeat the Wall of Flesh to unlock this!",
				3 => "Defeat any mechanical boss to unlock this!",
				4 => "Defeat the golem to unlock this!",
				5 => "Maximum reached!",
				_ => "An unknown error has occured.",
			};
		}

		public override void PostUpdatePlayers()
		{
			buildings.ForEach(n => n.PrePassiveUpdate());

			foreach (Player player in Main.player.Where(n => n.active))
			{
				buildings.ForEach(n => n.PassiveBoost(player));
			}

			buildings.ForEach(n =>
			{
				if (!n.IsTileValid(Framing.GetTileSafely(n.position)))
				{
					RemoveBuilding(n);
					return;
				}

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
			byPosition = new();

			IList<TagCompound> buildingData = tag.GetList<TagCompound>("buildings");

			foreach (TagCompound tag2 in buildingData)
			{
				var building = (Building)Activator.CreateInstance(BrickAndMortar.instance.Code.GetType(tag2.GetString("type")));
				building.LoadData(tag2);

				AddBuilding(building);
			}
		}
	}
}
