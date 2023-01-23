using BrickAndMortar.Content.Buildings;
using BrickAndMortar.Core.Loaders.UILoading;
using BrickAndMortar.Core.Systems.BuildingSystem;
using System;
using System.Collections.Generic;
using Terraria.UI;

namespace BrickAndMortar.Content.GUI
{
	internal class BuildingGUI : SmartUIState
	{
		public static Building building;

		public UpgradeButton upgradeButton;
		public CloseButton closeButton;
		public SpecialButton specialButton;

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

			specialButton = new();
			Append(specialButton);
		}

		public override void Update(GameTime gameTime)
		{
			if (building is null)
				return;

			if (Vector2.Distance(Main.LocalPlayer.Center, building.Center) > 500)
				building = null;

			if (building is null)
				return;

			SetButton(closeButton, building.Center + new Vector2(160, -building.Height * 8 - bgHeight - 28) - Main.screenPosition);
			SetButton(upgradeButton, building.Center + new Vector2(160, -building.Height * 8 - 68) - Main.screenPosition);
			SetButton(specialButton, building.Center + new Vector2(160, -building.Height * 8 - 68 - 42) - Main.screenPosition);

			Recalculate();
			base.Update(gameTime);
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			if (building is null)
				return;

			Vector2 pos = building.Center + new Vector2(-150, -(bgHeight + building.Height * 8 + 32)) - Main.screenPosition;

			var bgTarget = new Rectangle((int)pos.X, (int)pos.Y, 300, bgHeight);

			Texture2D bg = Terraria.GameContent.TextureAssets.MagicPixel.Value;

			DrawBox(spriteBatch, bgTarget);

			string levelString = $"(Level {building.level + 1}" + (building.levelBoost > 0 ? $"+{building.levelBoost})" : ")");
			Utils.DrawBorderString(spriteBatch, $"{building.FriendlyName} {levelString}", pos + Vector2.One * 10, Color.White, 1.2f);

			var dividerTarget = new Rectangle((int)pos.X + 10, (int)pos.Y + 40, 280, 2);
			spriteBatch.Draw(bg, dividerTarget, new Color(20, 40, 70) * 0.8f);

			dividerTarget.Offset(0, 2);
			spriteBatch.Draw(bg, dividerTarget, new Color(49, 84, 141) * 0.8f);

			ReLogic.Graphics.DynamicSpriteFont font = Terraria.GameContent.FontAssets.MouseText.Value;
			string info = Helpers.Helper.WrapString(building.Info, 220, font, 0.8f);
			Utils.DrawBorderString(spriteBatch, info, pos + new Vector2(20, 48), Color.White, 0.8f);

			dividerTarget = new Rectangle((int)pos.X + 10, (int)(pos.Y + font.MeasureString(info).Y) + 32, 280, 2);
			spriteBatch.Draw(bg, dividerTarget, new Color(20, 40, 70) * 0.8f);

			dividerTarget.Offset(0, 2);
			spriteBatch.Draw(bg, dividerTarget, new Color(49, 84, 141) * 0.8f);

			building.statlines = new();

			if (building.level < building.MaxLevel && (upgradeButton.IsMouseHovering || building.underConstruction))
				building.SetNextStatLines();
			else
				building.SetStatLines();

			float y = building.DrawStatLines(spriteBatch, pos + new Vector2(20, 48 + font.MeasureString(info).Y));

			bgHeight = (int)(y + 24 - pos.Y);

			base.Draw(spriteBatch);
		}

		public static void DrawBox(SpriteBatch sb, Rectangle target, Color color = default)
		{
			Texture2D tex = ModContent.Request<Texture2D>("BrickAndMortar/Assets/GUI/Box").Value;

			if (color == default)
				color = new Color(49, 84, 141) * 0.9f;

			var sourceCorner = new Rectangle(0, 0, 6, 6);
			var sourceEdge = new Rectangle(6, 0, 4, 6);
			var sourceCenter = new Rectangle(6, 6, 4, 4);

			Rectangle inner = target;
			inner.Inflate(-4, -4);

			sb.Draw(tex, inner, sourceCenter, color);

			sb.Draw(tex, new Rectangle(target.X + 2, target.Y, target.Width - 4, 6), sourceEdge, color, 0, Vector2.Zero, 0, 0);
			sb.Draw(tex, new Rectangle(target.X, target.Y - 2 + target.Height, target.Height - 4, 6), sourceEdge, color, -(float)Math.PI * 0.5f, Vector2.Zero, 0, 0);
			sb.Draw(tex, new Rectangle(target.X - 2 + target.Width, target.Y + target.Height, target.Width - 4, 6), sourceEdge, color, (float)Math.PI, Vector2.Zero, 0, 0);
			sb.Draw(tex, new Rectangle(target.X + target.Width, target.Y + 2, target.Height - 4, 6), sourceEdge, color, (float)Math.PI * 0.5f, Vector2.Zero, 0, 0);

			sb.Draw(tex, new Rectangle(target.X, target.Y, 6, 6), sourceCorner, color, 0, Vector2.Zero, 0, 0);
			sb.Draw(tex, new Rectangle(target.X + target.Width, target.Y, 6, 6), sourceCorner, color, (float)Math.PI * 0.5f, Vector2.Zero, 0, 0);
			sb.Draw(tex, new Rectangle(target.X + target.Width, target.Y + target.Height, 6, 6), sourceCorner, color, (float)Math.PI, Vector2.Zero, 0, 0);
			sb.Draw(tex, new Rectangle(target.X, target.Y + target.Height, 6, 6), sourceCorner, color, (float)Math.PI * 1.5f, Vector2.Zero, 0, 0);
		}
	}

	internal class UpgradeButton : UIElement
	{
		public override void Draw(SpriteBatch spriteBatch)
		{
			if (BuildingGUI.building != null)
			{
				var inflated = GetDimensions().ToRectangle();
				inflated.Inflate(4, 4);
				BuildingGUI.DrawBox(spriteBatch, inflated);

				Texture2D tex = ModContent.Request<Texture2D>("BrickAndMortar/Assets/GUI/Upgrade").Value;
				Vector2 pos = GetDimensions().Position();

				spriteBatch.Draw(tex, pos, Color.White);

				if (IsMouseHovering)
				{
					if (BuildingGUI.building.level >= BuildingGUI.building.MaxLevel)
						Utils.DrawBorderString(spriteBatch, BuildingSystem.GetWorldTier() == 4 ? "Max level!" : "Advance your world tier to unlock", Main.MouseScreen + Vector2.One * 16, Color.White);
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
				var inflated = GetDimensions().ToRectangle();
				inflated.Inflate(4, 4);
				BuildingGUI.DrawBox(spriteBatch, inflated);

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

	internal class SpecialButton : UIElement
	{
		public override void Draw(SpriteBatch spriteBatch)
		{
			if (BuildingGUI.building != null && BuildingGUI.building.HasTertiaryButton)
			{
				var inflated = GetDimensions().ToRectangle();
				inflated.Inflate(4, 4);
				BuildingGUI.DrawBox(spriteBatch, inflated);

				Texture2D tex = ModContent.Request<Texture2D>("BrickAndMortar/Assets/GUI/Special").Value;
				Vector2 pos = GetDimensions().Position();

				spriteBatch.Draw(tex, pos, Color.White);

				if (IsMouseHovering)
					Utils.DrawBorderString(spriteBatch, BuildingGUI.building.TertiaryButtonText, Main.MouseScreen + Vector2.One * 16, Color.White);
			}
		}

		public override void Click(UIMouseEvent evt)
		{
			if (BuildingGUI.building != null && BuildingGUI.building.HasTertiaryButton)
				BuildingGUI.building.OnTertiaryButtonClick();
		}
	}
}
