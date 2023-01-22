using BrickAndMortar.Content.Buildings;
using BrickAndMortar.Core.Loaders.UILoading;
using BrickAndMortar.Core.Systems.BuildingSystem;
using System.Collections.Generic;
using System.Linq;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace BrickAndMortar.Content.GUI
{
	internal class BuildMenu : SmartUIState
	{
		public UIImageButton expandButton;

		public override bool Visible => Main.playerInventory;

		public override int InsertionIndex(List<GameInterfaceLayer> layers)
		{
			return layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
		}

		public override void OnInitialize()
		{
			expandButton = new UIImageButton(ModContent.Request<Texture2D>("BrickAndMortar/Assets/GUI/TowerDamage"));
			expandButton.Left.Set(236, 0);
			expandButton.Top.Set(310, 0);
			expandButton.Width.Set(32, 0);
			expandButton.Height.Set(32, 0);
			expandButton.OnClick += Expand;
			expandButton.SetVisibility(1, 1);
			Append(expandButton);
		}

		private void Expand(UIMouseEvent evt, UIElement listeningElement)
		{
			expandButton.OnClick += Collapse;

			int x = 200;

			foreach (Building building in BuildingSystem.typeDummies)
			{
				if (building.GetBuildCount() <= 0) //Show only buildings that are level appropriate
					continue;

				ModItem instance = BrickAndMortar.instance.Find<ModItem>(building.Name + "_item");
				var button = new BuildingButton(instance.Item, building);
				button.Left.Set(x, 0);
				button.Top.Set(346, 0);
				button.Width.Set(32, 0);
				button.Height.Set(32, 0);
				Append(button);
				x += 36;
			}
		}

		private void Collapse(UIMouseEvent evt, UIElement listeningElement)
		{
			RemoveAllChildren();
			OnInitialize();
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			base.Draw(spriteBatch);

			Texture2D backTex = ModContent.Request<Texture2D>("BrickAndMortar/Assets/GUI/Defense").Value;

			spriteBatch.Draw(backTex, new Vector2(400, 310), Color.White);
			Utils.DrawBorderString(spriteBatch, $"{BuildingSystem.GetWorldTier() + 1}", new Vector2(416, 332), Color.Lerp(new Color(100, 255, 100), new Color(255, 100, 100), BuildingSystem.GetWorldTier() / 4f), 1.1f, 0.5f, 0.5f);

			if (new Rectangle(400, 310, 32, 32).Contains(Main.MouseScreen.ToPoint()))
				Utils.DrawBorderString(spriteBatch, $"World tier: {BuildingSystem.GetWorldTier() + 1}\n{BuildingSystem.GetErrorMessage(BuildingSystem.GetWorldTier() + 1)}", Main.MouseScreen + Vector2.One * 16, Color.White);

			if (expandButton.IsMouseHovering)
				Utils.DrawBorderString(spriteBatch, "Build menu", Main.MouseScreen + Vector2.One * 16, Color.White);
		}
	}

	internal class BuildingButton : UIElement
	{
		public Item item;
		public Building building;

		public BuildingButton(Item item, Building building)
		{
			this.item = item;
			this.building = building;
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			var rect = GetDimensions().ToRectangle();
			BuildingGUI.DrawBox(spriteBatch, rect);

			Texture2D itemTex = ModContent.Request<Texture2D>(item.ModItem.Texture).Value;
			rect.Inflate(-4, -4);

			spriteBatch.Draw(itemTex, rect, Color.White);

			if (IsMouseHovering)
			{
				Main.LocalPlayer.mouseInterface = true;
				Main.HoverItem = item.Clone();
				Main.hoverItemName = "a";
			}
		}

		public override void Click(UIMouseEvent evt)
		{
			if (Main.mouseItem.IsAir)
			{
				if (BuildingSystem.buildings.Count(n => n.Name == building.Name) < building.GetBuildCount())
					Main.mouseItem = item.Clone();
				else
					Main.NewText("Upgrade your world tier to place more of this building!", Color.Red);
			}
		}
	}
}
