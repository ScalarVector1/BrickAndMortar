using BrickAndMortar.Core.Loaders.UILoading;
using BrickAndMortar.Core.Systems.ResourceSystem;
using System.Collections.Generic;
using Terraria.UI;

namespace BrickAndMortar.Content.GUI
{
	internal class ResourceGUI : SmartUIState
	{
		public ResourceBar aurumBar;
		public ResourceBar lifeforceBar;

		public override bool Visible => Main.playerInventory;

		public override int InsertionIndex(List<GameInterfaceLayer> layers)
		{
			return 1;
		}

		public override void OnInitialize()
		{
			AddResourceBar(ref aurumBar, new Vector2(210, 270), "BrickAndMortar/Assets/GUI/AurumIcon", new Color(255, 200, 20));
			AddResourceBar(ref lifeforceBar, new Vector2(340, 270), "BrickAndMortar/Assets/GUI/LifeforceIcon", new Color(20, 220, 255));
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
		private void AddResourceBar(ref ResourceBar element, Vector2 pos, string texture, Color color)
		{
			element = new ResourceBar(texture, color);
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
		Color color;

		public int fillTarget;
		public int visibleFill;
		public int maxFill;

		public float VisiblePercent => visibleFill / (float)maxFill;

		public ResourceBar(string iconTexture, Color color)
		{
			this.iconTexture = iconTexture;
			this.color = color;
		}

		public override void Update(GameTime gameTime)
		{
			if (visibleFill < fillTarget)
				visibleFill++;

			if (visibleFill > fillTarget)
				visibleFill--;
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
		}
	}
}
