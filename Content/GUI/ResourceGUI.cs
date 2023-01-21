using BrickAndMortar.Core.Loaders.UILoading;
using BrickAndMortar.Core.Systems.ResourceSystem;
using System;
using System.Collections.Generic;
using Terraria.ID;
using Terraria.UI;

namespace BrickAndMortar.Content.GUI
{
	internal class ResourceGUI : SmartUIState
	{
		public ResourceBar aurumBar;
		public ResourceBar lifeforceBar;

		public StorageUpgradeButton upgradeButton;

		public override bool Visible => Main.playerInventory;

		public override int InsertionIndex(List<GameInterfaceLayer> layers)
		{
			return 1;
		}

		public override void OnInitialize()
		{
			AddResourceBar(ref aurumBar, new Vector2(210, 270), "BrickAndMortar/Assets/GUI/AurumIcon", "Aurum", new Color(255, 200, 20));
			AddResourceBar(ref lifeforceBar, new Vector2(340, 270), "BrickAndMortar/Assets/GUI/LifeforceIcon", "Lifeforce", new Color(20, 220, 255));

			upgradeButton = new StorageUpgradeButton();
			upgradeButton.Left.Set(200, 0);
			upgradeButton.Top.Set(310, 0);
			upgradeButton.Width.Set(32, 0);
			upgradeButton.Height.Set(32, 0);
			Append(upgradeButton);
		}

		public override void Update(GameTime gameTime)
		{
			ResourcePlayer mp = Main.LocalPlayer.GetModPlayer<ResourcePlayer>();

			aurumBar.fillTarget = mp.aurumAmount;
			aurumBar.maxFill = mp.GetMaxFromStorageLevel();

			lifeforceBar.fillTarget = mp.lifeforceAmount;
			lifeforceBar.maxFill = mp.GetMaxFromStorageLevel();

			base.Update(gameTime);
		}

		/// <summary>
		/// Sets up a resource bar UI element
		/// </summary>
		/// <param name="element">The element to set up</param>
		/// <param name="pos">Where the element should be positioned on the screen</param>
		/// <param name="texture">The path to the icon texture of the bar</param>
		/// <param name="color">The color of the bar's fill</param>
		private void AddResourceBar(ref ResourceBar element, Vector2 pos, string texture, string hoverText, Color color)
		{
			element = new ResourceBar(texture, hoverText, color);
			element.Left.Set(pos.X, 0);
			element.Top.Set(pos.Y, 0);
			element.Width.Set(100, 0);
			element.Height.Set(32, 0);

			Append(element);
		}
	}

	internal class ResourceBar : UIElement
	{
		readonly string iconTexture;
		readonly string hoverText;
		Color color;

		public int fillTarget;
		public int visibleFill;
		public int maxFill;

		public float VisiblePercent => visibleFill / (float)maxFill;

		public ResourceBar(string iconTexture, string hoverText, Color color)
		{
			this.iconTexture = iconTexture;
			this.hoverText = hoverText;
			this.color = color;
		}

		public override void Update(GameTime gameTime)
		{
			if (visibleFill < fillTarget)
			{
				int diff = fillTarget - visibleFill;
				visibleFill += Math.Max(1, diff / 10);
			}

			if (visibleFill > fillTarget)
			{
				int diff = visibleFill - fillTarget;
				visibleFill -= Math.Max(1, diff / 10);
			}
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			Texture2D iconTex = ModContent.Request<Texture2D>(iconTexture).Value;
			Texture2D barTex = ModContent.Request<Texture2D>("BrickAndMortar/Assets/GUI/BarEmpty").Value;
			Texture2D fillTex = ModContent.Request<Texture2D>("BrickAndMortar/Assets/GUI/BarFill").Value;

			Vector2 pos = GetDimensions().ToRectangle().TopLeft();
			var fillTarget = GetDimensions().ToRectangle();
			fillTarget.Width = (int)(fillTarget.Width * VisiblePercent);

			var fillSource = new Rectangle(0, 0, fillTarget.Width, fillTarget.Height);

			spriteBatch.Draw(barTex, pos, Color.White);
			spriteBatch.Draw(fillTex, fillTarget, fillSource, color);

			spriteBatch.Draw(iconTex, pos + new Vector2(-24, -2), Color.White);

			Vector2 textPos = GetDimensions().ToRectangle().Center();

			Utils.DrawBorderString(spriteBatch, $"{visibleFill}/{maxFill}", textPos, Color.White, 0.7f, 0.5f, 0.5f);

			if (IsMouseHovering)
				Utils.DrawBorderString(spriteBatch, hoverText + $": {visibleFill}/{maxFill}", Main.MouseScreen + Vector2.One * 16, Color.White, 1f);
		}
	}

	internal class StorageUpgradeButton : UIElement
	{
		public override void Draw(SpriteBatch spriteBatch)
		{
			Texture2D tex = ModContent.Request<Texture2D>("BrickAndMortar/Assets/GUI/StorageUpgradeButton").Value;
			Vector2 pos = GetDimensions().ToRectangle().TopLeft();

			spriteBatch.Draw(tex, pos, Color.White);

			if (IsMouseHovering)
			{
				ResourcePlayer mp = Main.LocalPlayer.GetModPlayer<ResourcePlayer>();

				ResourceSpending.DrawCost(spriteBatch, Main.MouseScreen + Vector2.One * 16, "upgrade storage capacity", mp.GetStorageUpgradeCost(), 0);

				Main.LocalPlayer.mouseInterface = true;
			}
		}

		public override void Click(UIMouseEvent evt)
		{
			ResourcePlayer mp = Main.LocalPlayer.GetModPlayer<ResourcePlayer>();

			if (ResourceSpending.TrySpendAurum(Main.LocalPlayer, mp.GetStorageUpgradeCost()))
			{
				mp.storageLevel++;
				Main.NewText($"Your resource storage has been upgraded to level {mp.storageLevel + 1}!", Color.Yellow);
				Terraria.Audio.SoundEngine.PlaySound(SoundID.ResearchComplete);
			}
		}
	}
}
