using BrickAndMortar.Core.Systems.BuildingSystem;
using System;
using System.Linq;
using Terraria.ID;

namespace BrickAndMortar.Content.Buildings.Turret
{
	internal class Melter : Building
	{
		public int visualTimer;

		public float opacity;

		public int Burn => (EffectiveLevel + 1) * 5;

		public int DamageBoost => (EffectiveLevel + 1) * 4;

		public override string Name => "Melter";

		public override string Info => "Turn up the heat with the melter! Any and all enemies near it's superheated core will start to burn, slowly eating away at their life. Even across the entire world, the heat of the melter will empower your weapons with extra damage.";

		public override int Width => 5;

		public override int Height => 5;

		public override void SetStatLines()
		{
			statlines.Add(new Statline(Burn, 0, "Burning damage per second", "BrickAndMortar/Assets/GUI/TowerDamage", new Color(255, 210, 140)));
			statlines.Add(new Statline(DamageBoost, 0, "Flat damage boost", "BrickAndMortar/Assets/GUI/Damage", new Color(255, 200, 180)));
		}

		public override void SetNextStatLines()
		{
			statlines.Add(new Statline(Burn, (EffectiveLevel + 2) * 5, "Burning damage per second", "BrickAndMortar/Assets/GUI/TowerDamage", new Color(255, 210, 140)));
			statlines.Add(new Statline(DamageBoost, (EffectiveLevel + 2) * 4, "Flat damage boost", "BrickAndMortar/Assets/GUI/Damage", new Color(255, 200, 180)));
		}

		public override void PassiveBoost(Player player)
		{
			player.GetDamage(DamageClass.Generic).Flat += DamageBoost;
		}

		public override void Update()
		{
			System.Collections.Generic.IEnumerable<NPC> possibleTargets = Main.npc.Where(n => n.active && Vector2.Distance(n.Center, Center) < 400);

			foreach (NPC npc in possibleTargets)
			{
				npc.AddBuff(BuffID.OnFire, 2, false);
				npc.GetGlobalNPC<BurnNPC>().burn += Burn;
			}

			if (possibleTargets.Count() > 0)
			{
				visualTimer++;

				if (opacity < 1)
					opacity += 0.02f;
			}
			else
			{
				visualTimer = 0;

				if (opacity > 0)
					opacity -= 0.02f;
			}
		}

		public override void DrawBuilding(SpriteBatch spriteBatch, Vector2 position, Color lightColor)
		{
			base.DrawBuilding(spriteBatch, position, lightColor);

			for (int k = 0; k < 5; k++)
			{
				float progress = (visualTimer + k * 400f / 5f) % 400 / 400f;

				Texture2D tex = ModContent.Request<Texture2D>("BrickAndMortar/Assets/Misc/GlowRing").Value;
				var target = new Rectangle((int)(position.X + Width * 8 - 200 * progress), (int)(position.Y + Height * 8 - 200 * progress), (int)(400 * progress), (int)(400 * progress));
				var color = new Color(255, 200, 50)
				{
					A = 0
				};

				spriteBatch.Draw(tex, target, color * (1 - progress) * opacity);
			}
		}

		public override int GetAurumCost()
		{
			return (level + 1) * 1500;
		}

		public override long GetBuildTime()
		{
			return (level + 1) * 900;
		}

		public override int GetBuildCount()
		{
			return Math.Min(3, BuildingSystem.GetWorldTier());
		}
	}

	public class BurnNPC : GlobalNPC //Dont have access to all the funky SLR hook framework here and it would be kinda goofy to copy it over just for this
	{
		public int burn;

		public override bool InstancePerEntity => true;

		public override void UpdateLifeRegen(NPC npc, ref int damage)
		{
			npc.lifeRegen -= burn * 2;
		}

		public override void ResetEffects(NPC npc)
		{
			burn = 0;
		}
	}
}
