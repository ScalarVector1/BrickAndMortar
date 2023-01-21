using BrickAndMortar.Content.Buildings;
using BrickAndMortar.Core.Loaders.UILoading;
using System.Collections.Generic;
using Terraria.UI;

namespace BrickAndMortar.Content.GUI
{
	internal class BuildingGUI : SmartUIState
	{
		public static Building building;

		public UpgradeButton upgradeButton;
		public CloseButton closeButton;

		private int bgHeight;

		public override bool Visible => building != null;

		public override int InsertionIndex(List<GameInterfaceLayer> layers)
		{
			return layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
		}

		private void SetButton(UIElement element, Vector2 pos)
		{
			element.Left.Set(pos.X, 0);
			element.Top.Set(pos.Y, 0);
			element.Width.Set(32, 0);
			element.Height.Set(32, 0);
		}

		public override void OnInitialize()
		{
			upgradeButton = new();
			Append(upgradeButton);

			closeButton = new();
			Append(closeButton);
		}

		public override void Update(GameTime gameTime)
		{
			if (building is null)
				return;

			if (Vector2.Distance(Main.LocalPlayer.Center, building.Center) > 500)
				building = null;

			SetButton(closeButton, building.Center + new Vector2(-150, -300 + bgHeight + 16) - Main.screenPosition);
			SetButton(upgradeButton, building.Center + new Vector2(150 - 32, -300 + bgHeight + 16) - Main.screenPosition);

			Recalculate();
			base.Update(gameTime);
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			if (building is null)
				return;

			Vector2 pos = building.Center + new Vector2(-150, -300) - Main.screenPosition;

			var bgTarget = new Rectangle((int)pos.X, (int)pos.Y, 300, bgHeight);

			Texture2D bg = Terraria.GameContent.TextureAssets.MagicPixel.Value;

			spriteBatch.Draw(bg, bgTarget, new Color(50, 50, 100) * 0.75f);

			Utils.DrawBorderString(spriteBatch, $"{building.FriendlyName} (Level {building.level + 1})", pos + Vector2.One * 8, Color.White, 1.2f);

			ReLogic.Graphics.DynamicSpriteFont font = Terraria.GameContent.FontAssets.MouseText.Value;
			string info = Helpers.Helper.WrapString(building.Info, 220, font, 0.8f);
			Utils.DrawBorderString(spriteBatch, info, pos + new Vector2(8, 48), Color.White, 0.8f);

			building.statlines = new();

			if (building.level < building.MaxLevel && (upgradeButton.IsMouseHovering || building.underConstruction))
				building.SetNextStatLines();
			else
				building.SetStatLines();

			float y = building.DrawStatLines(spriteBatch, pos + new Vector2(8, 48 + font.MeasureString(info).Y));

			bgHeight = (int)(y + 24 - pos.Y);

			base.Draw(spriteBatch);
		}
	}

	internal class UpgradeButton : UIElement
	{
		public override void Draw(SpriteBatch spriteBatch)
		{
			if (BuildingGUI.building != null)
			{
				Texture2D tex = ModContent.Request<Texture2D>("BrickAndMortar/Assets/GUI/Upgrade").Value;
				Vector2 pos = GetDimensions().Position();

				spriteBatch.Draw(tex, pos, Color.White);

				if (IsMouseHovering)
				{
					if (BuildingGUI.building.level >= BuildingGUI.building.MaxLevel)
						Utils.DrawBorderString(spriteBatch, "Max level!", Main.MouseScreen + Vector2.One * 16, Color.White);
					else if (BuildingGUI.building.underConstruction)
						Utils.DrawBorderString(spriteBatch, "Already upgrading...", Main.MouseScreen + Vector2.One * 16, Color.White);
					else
						BuildingGUI.building.DrawCost(spriteBatch, Main.MouseScreen + Vector2.One * 16);
				}
			}
		}

		public override void Click(UIMouseEvent evt)
		{
			if (BuildingGUI.building.level >= BuildingGUI.building.MaxLevel)
				return;

			if (BuildingGUI.building != null)
				BuildingGUI.building.TryUpgrade(Main.LocalPlayer);
		}
	}

	internal class CloseButton : UIElement
	{
		public override void Draw(SpriteBatch spriteBatch)
		{
			if (BuildingGUI.building != null)
			{
				Texture2D tex = ModContent.Request<Texture2D>("BrickAndMortar/Assets/GUI/Close").Value;
				Vector2 pos = GetDimensions().Position();

				spriteBatch.Draw(tex, pos, Color.White);
			}
		}

		public override void Click(UIMouseEvent evt)
		{
			if (BuildingGUI.building != null)
				BuildingGUI.building = null;
		}
	}
}
