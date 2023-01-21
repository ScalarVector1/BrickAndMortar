using Terraria.ID;

namespace BrickAndMortar.Core
{
	public abstract class QuickTileItem : ModItem
	{
		public string internalName = "";
		public string itemName;
		public string itemToolTip;
		//private readonly int Tiletype;
		private readonly string tileName;
		private readonly int rarity;
		private readonly string texturePath;
		private readonly bool pathHasName;
		private readonly int itemValue;

		public override string Name => internalName != "" ? internalName : base.Name;

		public override string Texture => string.IsNullOrEmpty(texturePath) ? "" : texturePath + (pathHasName ? string.Empty : Name);

		public QuickTileItem() { }

		public QuickTileItem(string name, string tooltip, string placetype, int rare = ItemRarityID.White, string texturePath = null, bool pathHasName = false, int ItemValue = 0)
		{
			itemName = name;
			itemToolTip = tooltip;
			tileName = placetype;
			rarity = rare;
			this.texturePath = texturePath;
			this.pathHasName = pathHasName;
			itemValue = ItemValue;
		}

		public QuickTileItem(string internalName, string name, string tooltip, string placetype, int rare = ItemRarityID.White, string texturePath = null, bool pathHasName = false, int ItemValue = 0)
		{
			this.internalName = internalName;
			itemName = name;
			itemToolTip = tooltip;
			tileName = placetype;
			rarity = rare;
			this.texturePath = texturePath;
			this.pathHasName = pathHasName;
			itemValue = ItemValue;
		}

		public virtual void SafeSetDefaults() { }

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault(itemName ?? "ERROR");
			Tooltip.SetDefault(itemToolTip ?? "Report me please!");
		}

		public override void SetDefaults()
		{
			if (tileName is null)
				return;

			Item.width = 16;
			Item.height = 16;
			Item.maxStack = 999;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.useAnimation = 15;
			Item.useTime = 10;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.consumable = true;
			Item.createTile = Mod.Find<ModTile>(tileName).Type;
			Item.rare = rarity;
			Item.value = itemValue;
			SafeSetDefaults();
		}
	}

	public abstract class QuickWallItem : ModItem
	{
		public string itemName;
		public string itemToolTip;
		private readonly int wallType;
		private readonly int rarity;
		private readonly string texturePath;
		private readonly bool pathHasName;

		public override string Texture => string.IsNullOrEmpty(texturePath) ? base.Texture : texturePath + (pathHasName ? string.Empty : Name);

		protected QuickWallItem(string name, string tooltip, int placetype, int rare, string texturePath = null, bool pathHasName = false)
		{
			itemName = name;
			itemToolTip = tooltip;
			wallType = placetype;
			rarity = rare;
			this.texturePath = texturePath;
			this.pathHasName = pathHasName;
		}

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault(itemName);
			Tooltip.SetDefault(itemToolTip);
		}

		public override void SetDefaults()
		{
			Item.width = 16;
			Item.height = 16;
			Item.maxStack = 999;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.useAnimation = 15;
			Item.useTime = 10;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.consumable = true;
			Item.createWall = wallType;
			Item.rare = rarity;
		}
	}
}