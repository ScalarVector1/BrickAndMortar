using BrickAndMortar.Content.Heroes;

namespace BrickAndMortar.Content.Buildings.HeroAltars
{
	internal abstract class HeroAltar : Building
	{
		/// <summary>
		/// The hero instance currently alive associated with this altar. If this instance ever dies or despawns a new one will spawn back at the altar.
		/// </summary>
		public Hero hero;

		/// <summary>
		/// Gets the type of NPC to spawn as a hero. Does bad things if you set it to not a type associated with a hero, so dont do that.
		/// </summary>
		public abstract int HeroType { get; }

		public sealed override void Update()
		{
			if (hero is null || !hero.NPC.active)
			{
				int i = NPC.NewNPC(null, (int)Center.X, (int)Center.Y, HeroType);
				NPC npc = Main.npc[i];

				if (npc.ModNPC is Hero newHero)
					hero = newHero;

				hero.parent = this;
				hero.RefreshStats();
			}

			SafeUpdate();
		}

		public override void OnBuildComplete()
		{
			Main.NewText($"{hero.DisplayName} has reached level {level}!", Color.SkyBlue);
			hero.RefreshStats();
		}

		public virtual void SafeUpdate() { }
	}
}
