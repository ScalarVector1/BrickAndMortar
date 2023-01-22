using BrickAndMortar.Core.Systems.BuildingSystem;
using System;

namespace BrickAndMortar.Content.Buildings.Support
{
	internal class Energizer : Building
	{
		public override string Name => "Energizer";

		public override string Info => "This mysterious building powers up all buildings near it, making them more powerful. While powerful, it's range is short, so be sure to place it in a tactical position. This effect cannot power up other energizers.";

		public override int Width => 3;

		public override int Height => 6;

		public override int MaxLevel => Math.Max(0, BuildingSystem.GetWorldTier() - 2);

		public override void SetStatLines()
		{
			statlines.Add(new Statline(level + 1, 0, "Building level boost", "BrickAndMortar/Assets/GUI/TowerDamage", new Color(200, 150, 255)));
		}

		public override void SetNextStatLines()
		{
			statlines.Add(new Statline(level + 1, level + 2, "Building level boost", "BrickAndMortar/Assets/GUI/TowerDamage", new Color(200, 150, 255)));
		}

		public override void PrePassiveUpdate()
		{
			foreach (Building building in BuildingSystem.buildings)
			{
				if (Vector2.Distance(building.Center, Center) < 150 && building.Name != "Energizer")
					building.levelBoost += Math.Max(building.levelBoost, level + 1);
			}
		}

		public override void DrawBuilding(SpriteBatch spriteBatch, Vector2 position, Color lightColor)
		{
			base.DrawBuilding(spriteBatch, position, lightColor);

			Texture2D tex = ModContent.Request<Texture2D>("BrickAndMortar/Assets/Misc/GlowRing").Value;
			var target = new Rectangle((int)(position.X + Width * 8 - 150), (int)(position.Y + Height * 8 - 150), 300, 300);
			var color = new Color(200, 150, 255)
			{
				A = 0
			};

			spriteBatch.Draw(tex, target, color);
		}

		public override int GetLifeforceCost()
		{
			return (level + 1) * 3000;
		}

		public override long GetBuildTime()
		{
			return (level + 1) * 1800;
		}

		public override int GetBuildCount()
		{
			return
				BuildingSystem.GetWorldTier() >= 4 ? 2 :
				BuildingSystem.GetWorldTier() >= 2 ? 1 :
				0;
		}
	}
}
