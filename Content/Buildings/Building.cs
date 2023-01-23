using BrickAndMortar.Content.GUI;
using BrickAndMortar.Core.Loaders.TileLoading;
using BrickAndMortar.Core.Systems.BuildingSystem;
using BrickAndMortar.Core.Systems.ResourceSystem;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;
using Terraria.UI.Chat;

namespace BrickAndMortar.Content.Buildings
{
	internal struct Statline
	{
		readonly float currentValue;
		readonly float nextValue;
		readonly string description;
		readonly string iconTexture;
		Color color;

		public Statline(float currentValue, float nextValue, string description, string iconTexture, Color color)
		{
			this.currentValue = currentValue;
			this.nextValue = nextValue;
			this.description = description;
			this.iconTexture = iconTexture;
			this.color = color;
		}

		public void Draw(SpriteBatch spriteBatch, Vector2 position)
		{
			var bgTarget = new Rectangle((int)position.X - 4, (int)position.Y - 4, 266, 42);
			Texture2D bg = Terraria.GameContent.TextureAssets.MagicPixel.Value;
			Texture2D glow = Terraria.GameContent.TextureAssets.Extra[209].Value;

			Color glowColor = color;
			glowColor.A = 0;

			spriteBatch.Draw(bg, bgTarget, Color.Black * 0.25f);
			spriteBatch.Draw(glow, bgTarget, glowColor * 0.75f);

			Texture2D icon = ModContent.Request<Texture2D>(iconTexture).Value;

			if (nextValue != 0)
			{
				Utils.DrawBorderString(spriteBatch, $"{description}:", position, color, 0.9f);

				spriteBatch.Draw(icon, position + new Vector2(0, 20), null, Color.White, 0, Vector2.Zero, 0.5f, 0, 0);
				Utils.DrawBorderString(spriteBatch, $"{currentValue} => {nextValue}", position + new Vector2(24, 18), color, 0.8f);
			}
			else
			{
				Utils.DrawBorderString(spriteBatch, $"{description}:", position, color, 0.9f);

				spriteBatch.Draw(icon, position + new Vector2(0, 20), null, Color.White, 0, Vector2.Zero, 0.5f, 0, 0);
				Utils.DrawBorderString(spriteBatch, $"{currentValue}", position + new Vector2(24, 18), color, 0.8f);
			}
		}
	}

	internal abstract class Building : ILoadable
	{
		public int level;
		public int levelBoost;

		public Point16 position;

		public bool underConstruction;
		public long buildStartTime;

		public List<Statline> statlines;

		public int EffectiveLevel => level + levelBoost;

		private TileObjectData Data => TileObjectData.GetTileData(TileType, 0);

		public Vector2 Center => position.ToVector2() * 16 + new Vector2(Data.Width, Data.Height) * 16 / 2;

		public string TexturePath => $"BrickAndMortar/Assets/Buildings/{Name}";

		/// <summary>
		/// The type of the tile that this building actually gets placed as in the world
		/// </summary>
		public int TileType => BrickAndMortar.instance.Find<ModTile>(Name + "_tile").Type;

		/// <summary>
		/// The intenral name of this building
		/// </summary>
		public abstract string Name { get; }

		/// <summary>
		/// The name visible to the player
		/// </summary>
		public virtual string FriendlyName => Name;

		/// <summary>
		/// The description given to this building when hovered over
		/// </summary>
		public abstract string Info { get; }

		/// <summary>
		/// Width of the building in tiles
		/// </summary>
		public abstract int Width { get; }

		/// <summary>
		/// Height of the building in tiles
		/// </summary>
		public abstract int Height { get; }

		/// <summary>
		/// Gets the max level of this building. By default the world tier.
		/// </summary>
		public virtual int MaxLevel => BuildingSystem.GetWorldTier();

		/// <summary>
		/// If this building should have a third button option when clicked on
		/// </summary>
		/// <returns></returns>
		public virtual bool HasTertiaryButton => false;

		public void Load(Mod mod)
		{
			BuildingSystem.typeDummyByName[Name] = this;
			BuildingSystem.typeDummies.Add(this);

			mod.AddContent(new BuildingTile(Name + "_tile", new FurnitureLoadData(Width, Height, 0, SoundID.Tink, false, Color.White), 0, $"BrickAndMortar/Assets/Buildings/{Name}", Name));
			mod.AddContent(new BuildingItem(Name + "_tile", Name));
		}

		public void Unload()
		{

		}

		/// <summary>
		/// If the tile at the building's position can actually host the building. If not, the building is destroyed.
		/// </summary>
		/// <param name="tile">The tile which the building is supposedly located at</param>
		/// <returns>If the building can continue to exist at that tile or not</returns>
		public virtual bool IsTileValid(Tile tile) { return tile.TileType == BrickAndMortar.instance.Find<ModTile>(Name + "_tile").Type; }

		/// <summary>
		/// Occurs before passive boosts are applied. Used for things like buffing other buildings.
		/// </summary>
		public virtual void PrePassiveUpdate() { }

		/// <summary>
		/// The buffs which this building should grant a player passively.
		/// </summary>
		/// <param name="player">The player to buff</param>
		public virtual void PassiveBoost(Player player) { }

		/// <summary>
		/// What the building should do while active, used for things like shooting at NPCs
		/// </summary>
		public virtual void Update() { }

		/// <summary>
		/// Change the amount of aurum it costs to upgrade this building based on it's level
		/// </summary>
		/// <returns>The cost of an upgrade in aurum</returns>
		public virtual int GetAurumCost() { return 0; }

		/// <summary>
		/// Change the amount of lifeforce it costs to upgrade this building based on it's level
		/// </summary>
		/// <returns>The cost of an upgrade in lifeforce</returns>
		public virtual int GetLifeforceCost() { return 0; }

		/// <summary>
		/// Gets the build time of a building in seconds based on it's level
		/// </summary>
		/// <returns>The time in seconds to upgrade the building to the next level</returns>
		public virtual long GetBuildTime() { return 60; }

		/// <summary>
		/// Gets the amount of copies that can be placed based on the world tier
		/// </summary>
		/// <returns></returns>
		public virtual int GetBuildCount() { return 1; }

		/// <summary>
		/// What happens when the tertiary button is pressed. See HasTertiaryButton
		/// </summary>
		public virtual void OnTertiaryButtonClick() { }

		/// <summary>
		/// Sets the stat lines of this building
		/// </summary>
		public virtual void SetStatLines() { }

		/// <summary>
		/// Sets the stat lines for displaying this building's next upgrade
		/// </summary>
		public virtual void SetNextStatLines() { }

		/// <summary>
		/// Allows you to modify or override how the building is drawn. Base call draws a frame based on level and width/height.
		/// </summary>
		/// <param name="spriteBatch"></param>
		/// <param name="position"></param>
		/// <param name="lightColor"></param>
		public virtual void DrawBuilding(SpriteBatch spriteBatch, Vector2 position, Color lightColor)
		{
			Texture2D tex = ModContent.Request<Texture2D>(TexturePath).Value;
			var source = new Rectangle(level * Width * 16, 0, Width * 16, Height * 16);

			spriteBatch.Draw(tex, position, source, lightColor);

			DrawBuildTime(spriteBatch, position + new Vector2(Width * 16 / 2 - 42, -36));
		}

		/// <summary>
		/// Draws this building's stats at a given position.
		/// </summary>
		/// <param name="spriteBatch"></param>
		/// <param name="position"></param>
		/// <returns>the Y position of the bottom of the stat lines</returns>
		public float DrawStatLines(SpriteBatch spriteBatch, Vector2 position)
		{
			foreach (Statline line in statlines)
			{
				line.Draw(spriteBatch, position);
				position.Y += 48;
			}

			return position.Y;
		}

		/// <summary>
		/// Modify what data gets saved with this building. Base call saves type, pos, level and build data. Dont forget to override LoadData also!
		/// </summary>
		/// <param name="tag"></param>
		public virtual void SaveData(TagCompound tag)
		{
			tag["type"] = GetType().ToString(); //used in BuildingSystem loader to create instance
			tag["position"] = position.ToVector2();
			tag["level"] = level;
			tag["building"] = underConstruction;
			tag["buildStartTime"] = buildStartTime;
		}

		/// <summary>
		/// Modifies how data is loaded. Base call loads type, pos, level and build data.
		/// </summary>
		/// <param name="tag"></param>
		public virtual void LoadData(TagCompound tag)
		{
			position = tag.Get<Vector2>("position").ToPoint16();
			level = tag.GetInt("level");
			underConstruction = tag.GetBool("building");
			buildStartTime = tag.GetLong("buildStartTime");
		}

		public void UpdateBuildTimes()
		{
			long time = DateTimeOffset.Now.ToUnixTimeSeconds();

			if (underConstruction && time >= buildStartTime + GetBuildTime())
			{
				level++;
				underConstruction = false;
				buildStartTime = 0;
			}
		}

		public void DrawCost(SpriteBatch spriteBatch, Vector2 pos)
		{
			ResourceSpending.DrawCost(spriteBatch, pos, $"upgrade {FriendlyName}", GetAurumCost(), GetLifeforceCost());
		}

		public void DrawBuildTime(SpriteBatch spriteBatch, Vector2 pos)
		{
			if (underConstruction)
			{
				Texture2D iconTex = ModContent.Request<Texture2D>("BrickAndMortar/Assets/GUI/Clock").Value;
				Texture2D barTex = ModContent.Request<Texture2D>("BrickAndMortar/Assets/GUI/BarEmpty").Value;
				Texture2D fillTex = ModContent.Request<Texture2D>("BrickAndMortar/Assets/GUI/BarFill").Value;

				long time = DateTimeOffset.Now.ToUnixTimeSeconds();

				float visiblePercent = (time - buildStartTime) / (float)GetBuildTime();

				var fillTarget = new Rectangle((int)pos.X, (int)pos.Y, 100, 32);
				fillTarget.Width = (int)(fillTarget.Width * visiblePercent);

				var fillSource = new Rectangle(0, 0, fillTarget.Width, fillTarget.Height);

				spriteBatch.Draw(barTex, pos, Color.White);
				spriteBatch.Draw(fillTex, fillTarget, fillSource, new Color(50, 255, 150));

				spriteBatch.Draw(iconTex, pos + new Vector2(-24, -2), Color.White);

				Vector2 textPos = pos + new Vector2(50, 16);

				long diff = buildStartTime + GetBuildTime() - time;

				int minutes = (int)(diff / 60);
				int seconds = (int)(diff % 60);

				string secAdj = seconds < 10 ? "0" : "";
				string minAdj = minutes < 10 ? "0" : "";

				string timer = $"{minAdj}{minutes}:{secAdj}{seconds}";
				Utils.DrawBorderString(spriteBatch, timer, textPos, Color.White, 0.7f, 0.5f, 0.5f);
			}
		}

		/// <summary>
		/// Attempts to upgrade a building based on it's costs
		/// </summary>
		/// <param name="player">The player to draw resources from to upgrade</param>
		/// <returns>If the upgrade was successful or not</returns>
		public bool TryUpgrade(Player player)
		{
			if (underConstruction)
				return false;

			ResourcePlayer mp = player.GetModPlayer<ResourcePlayer>();

			if (GetAurumCost() > 0 && GetLifeforceCost() > 0)
			{
				if (ResourceSpending.TrySpendBoth(player, GetAurumCost(), GetLifeforceCost()))
				{
					underConstruction = true;
					buildStartTime = DateTimeOffset.Now.ToUnixTimeSeconds();
					return true;
				}
			}
			else if (GetAurumCost() > 0)
			{
				if (ResourceSpending.TrySpendAurum(player, GetAurumCost()))
				{
					underConstruction = true;
					buildStartTime = DateTimeOffset.Now.ToUnixTimeSeconds();
					return true;
				}
			}
			else if (GetLifeforceCost() > 0)
			{
				if (ResourceSpending.TrySpendLifeforce(player, GetLifeforceCost()))
				{
					underConstruction = true;
					buildStartTime = DateTimeOffset.Now.ToUnixTimeSeconds();
					return true;
				}
			}
			else //upgrade is marked as free
			{
				underConstruction = true;
				buildStartTime = DateTimeOffset.Now.ToUnixTimeSeconds();
				return true;
			}

			return false;
		}

		/// <summary>
		/// Attempts to place a building based on it's costs
		/// </summary>
		/// <param name="player">The player trying to build</param>
		/// <returns>If the building can be placed or not</returns>
		public bool TryPlace(Player player)
		{
			ResourcePlayer mp = player.GetModPlayer<ResourcePlayer>();

			if (GetAurumCost() > 0 && GetLifeforceCost() > 0)
			{
				if (ResourceSpending.TrySpendBoth(player, GetAurumCost(), GetLifeforceCost()))
					return true;
			}
			else if (GetAurumCost() > 0)
			{
				if (ResourceSpending.TrySpendAurum(player, GetAurumCost()))
					return true;
			}
			else if (GetLifeforceCost() > 0)
			{
				if (ResourceSpending.TrySpendLifeforce(player, GetLifeforceCost()))
					return true;
			}
			else //building is marked as free
			{
				return true;
			}

			return false;
		}
	}

	[Autoload(false)]
	internal class BuildingTile : LoaderFurniture
	{
		private readonly string buildingName;

		public BuildingTile(string name, FurnitureLoadData data, int drop, string texture, string building) : base(name, data, drop, texture)
		{
			buildingName = building;
		}

		public Building BuildingInstance(Point16 pos)
		{
			if (BuildingSystem.buildingByPosition.ContainsKey(pos))
				return BuildingSystem.buildingByPosition[pos];

			return null;
		}

		public override bool CanPlace(int i, int j)
		{
			if (BuildingSystem.typeDummyByName[buildingName].TryPlace(Main.LocalPlayer))
			{
				return true;
			}
			else
			{
				Main.LocalPlayer.itemAnimation = 0;
				Main.mouseItem.TurnToAir();
				return false;
			}
		}

		public override void PlaceInWorld(int i, int j, Item item)
		{
			Tile tile = Framing.GetTileSafely(i, j);

			i -= tile.TileFrameX / 18;
			j -= tile.TileFrameY / 18;

			var building = (Building)Activator.CreateInstance(BuildingSystem.typeDummyByName[buildingName].GetType());
			building.position = new Point16(i, j);

			BuildingSystem.AddBuilding(building);
		}

		public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			i -= frameX / 18;
			j -= frameY / 18;

			BuildingSystem.RemoveBuilding(BuildingInstance(new Point16(i, j)));
		}

		public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
		{
			Tile tile = Framing.GetTileSafely(i, j);

			if (tile.TileFrameX == 0 && tile.TileFrameY == 0)
			{
				Building instance = BuildingInstance(new Point16(i, j));

				if (instance != null)
					instance.DrawBuilding(spriteBatch, new Vector2(i, j) * 16 - Main.screenPosition + Helpers.Helper.TileAdj, Lighting.GetColor(new Point(i, j)));
			}

			return false;
		}

		public override bool RightClick(int i, int j)
		{
			Tile tile = Framing.GetTileSafely(i, j);

			i -= tile.TileFrameX / 18;
			j -= tile.TileFrameY / 18;

			BuildingGUI.building = BuildingSystem.buildingByPosition[new Point16(i, j)];

			return true;
		}
	}

	[Autoload(false)]
	internal class BuildingItem : QuickTileItem
	{
		private string buildingName;

		private Building Building => BuildingSystem.typeDummyByName[buildingName];

		protected override bool CloneNewInstances => true;

		public BuildingItem() : base() { }

		public BuildingItem(string placetype, string building) : base(
			$"{BuildingSystem.typeDummyByName[building].Name}_item",
			$"{BuildingSystem.typeDummyByName[building].FriendlyName}",
			"", placetype, 0, BuildingSystem.typeDummyByName[building].TexturePath + "_item", true)
		{
			buildingName = building;
		}

		public override void SafeSetDefaults()
		{
			Item.rare = ItemRarityID.Blue;
			Item.consumable = true;
		}

		public override ModItem Clone(Item newEntity)
		{
			ModItem clone = base.Clone(newEntity);
			(clone as BuildingItem).buildingName = buildingName;

			return clone;
		}

		public override void Update(ref float gravity, ref float maxFallSpeed)
		{
			Item.TurnToAir();
		}

		public override void UpdateInventory(Player player)
		{
			if (Item != Main.mouseItem || !CanUseItem(player))
				Item.TurnToAir();
		}

		public override bool ConsumeItem(Player player)
		{
			return BuildingSystem.buildings.Count(n => n.GetType() == Building.GetType()) >= Building.GetBuildCount();
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			if (Building is null)
				return;

			tooltips.Add(new TooltipLine(Mod, "Info", $"{Helpers.Helper.WrapString(Building.Info, 400, Terraria.GameContent.FontAssets.MouseText.Value, 1f)}"));

			var countLine = new TooltipLine(Mod, "MaxCount", $"Placed: {BuildingSystem.buildings.Count(n => n.GetType() == Building.GetType())}/{Building.GetBuildCount()}")
			{
				OverrideColor = CanUseItem(Main.LocalPlayer) ? Color.Yellow : Color.LightGray
			};
			tooltips.Add(countLine);
		}

		public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
		{
			ResourceSpending.DrawCost(spriteBatch, Main.MouseScreen + Vector2.One * 16, $"place {Building.FriendlyName}", Building.GetAurumCost(), Building.GetLifeforceCost());
		}

		public override bool PreDrawTooltip(ReadOnlyCollection<TooltipLine> lines, ref int x, ref int y)
		{

			float width;
			float height = -16;
			Vector2 pos;

			ReLogic.Graphics.DynamicSpriteFont font = Terraria.GameContent.FontAssets.MouseText.Value;

			string widest = lines.OrderBy(n => ChatManager.GetStringSize(font, n.Text, Vector2.One).X).Last().Text;
			width = ChatManager.GetStringSize(font, widest, Vector2.One).X;

			pos = new Vector2(x, y) + new Vector2(width + 30, 0);

			width = ChatManager.GetStringSize(font, $"Cost to place {Building.FriendlyName}", Vector2.One).X;
			height = 52;

			if (Building.GetAurumCost() > 0 && Building.GetLifeforceCost() > 0)
				height = 80;

			Utils.DrawInvBG(Main.spriteBatch, new Rectangle((int)pos.X - 10, (int)pos.Y - 10, (int)width + 24, (int)height + 20), new Color(25, 20, 55) * 0.925f);
			ResourceSpending.DrawCost(Main.spriteBatch, pos, $"place {Building.FriendlyName}", Building.GetAurumCost(), Building.GetLifeforceCost());

			return true;
		}

		public override bool CanUseItem(Player player)
		{
			return BuildingSystem.buildings.Count(n => n.GetType() == Building.GetType()) < Building.GetBuildCount();
		}
	}
}
