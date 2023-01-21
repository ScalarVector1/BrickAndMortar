using BrickAndMortar.Content.Buildings;
using BrickAndMortar.Core.Loaders.UILoading;
using System.Collections.Generic;
using Terraria.UI;

namespace BrickAndMortar.Content.GUI
{
	internal class BuidlingGUI : SmartUIState
	{
		public static Building building;

		public override bool Visible => building != null;

		public override int InsertionIndex(List<GameInterfaceLayer> layers)
		{
			return layers.Count;
		}

		public override void Update(GameTime gameTime)
		{
			if (Vector2.Distance(Main.LocalPlayer.Center, building.Center) > 500)
				building = null;
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			if (building is null)
				return;

			Vector2 pos = building.Center + new Vector2(-150, -300) - Main.screenPosition;

			var bgTarget = new Rectangle((int)pos.X, (int)pos.Y, 300, 600);

			Texture2D bg = Terraria.GameContent.TextureAssets.MagicPixel.Value;

			spriteBatch.Draw(bg, bgTarget, Color.Black * 0.5f);

			Utils.DrawBorderString(spriteBatch, building.FriendlyName, pos + Vector2.One * 8, Color.White, 1.2f);

			ReLogic.Graphics.DynamicSpriteFont font = Terraria.GameContent.FontAssets.MouseText.Value;
			string info = Helpers.Helper.WrapString(building.Info, 220, font, 0.8f);
			Utils.DrawBorderString(spriteBatch, info, pos + new Vector2(8, 48), Color.White, 0.8f);

			building.statlines = new();
			building.SetStatLines();
			float y = building.DrawStatLines(spriteBatch, pos + new Vector2(8, 48 + font.MeasureString(info).Y));

			building.DrawCost(spriteBatch, new Vector2(pos.X + 8, y + 24));
		}
	}
}
