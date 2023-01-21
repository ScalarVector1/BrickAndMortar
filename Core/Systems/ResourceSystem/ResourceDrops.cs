using BrickAndMortar.Content.Dusts;
using System.Linq;

namespace BrickAndMortar.Core.Systems.ResourceSystem
{
	internal class ResourceDropHelper
	{
		/// <summary>
		/// Spawns resource particles based on an amount of resources
		/// </summary>
		/// <param name="origin">Where the particles should originate from</param>
		/// <param name="recipient">Who the particles should go towards</param>
		/// <param name="amount">The amount of resource to spawn particles for. Larger particles are used for divisions of 100 and 10</param>
		/// <param name="color">The color of particle to spawn</param>
		public static void SpawnResourceDust(Vector2 origin, Player recipient, int amount, Color color)
		{
			int amountBig = amount / 100;
			int amountMedium = amount % 100 / 10;
			int amountSmall = amount % 10;

			for (int k = 0; k < amountBig; k++)
			{
				var d = Dust.NewDustPerfect(origin, ModContent.DustType<RewardDust>(), Vector2.One.RotatedByRandom(6.28f) * Main.rand.NextFloat(5f), 0, color, 1.3f);
				d.customData = recipient;
			}

			for (int k = 0; k < amountMedium; k++)
			{
				var d = Dust.NewDustPerfect(origin, ModContent.DustType<RewardDust>(), Vector2.One.RotatedByRandom(6.28f) * Main.rand.NextFloat(5f), 0, color * 0.75f, 0.65f);
				d.customData = recipient;
			}

			for (int k = 0; k < amountSmall; k++)
			{
				var d = Dust.NewDustPerfect(origin, ModContent.DustType<RewardDust>(), Vector2.One.RotatedByRandom(6.28f) * Main.rand.NextFloat(5f), 0, color * 0.5f, 0.25f);
				d.customData = recipient;
			}
		}

		/// <summary>
		/// Attempts to give a player a given amount of aurum
		/// </summary>
		/// <param name="source">The place for particles to originate from</param>
		/// <param name="player">The player to grant the resources to</param>
		/// <param name="amount">The amount of resources to try to give them</param>
		/// <returns>The amount of resources actually given (can be less than the requested amount due to storage caps)</returns>
		public static int GrantAurum(Vector2 source, Player player, int amount)
		{
			ResourcePlayer mp = player.GetModPlayer<ResourcePlayer>();
			int realAmount = mp.aurumAmount + amount < mp.GetMaxFromStorageLevel() ? amount : mp.GetMaxFromStorageLevel() - mp.aurumAmount;
			mp.aurumAmount += realAmount;

			SpawnResourceDust(source, player, realAmount, new Color(255, 200, 20));

			return realAmount;
		}

		/// <summary>
		/// Attempts to give a player a given amount of lifeforce
		/// </summary>
		/// <param name="source">The place for particles to originate from</param>
		/// <param name="player">The player to grant the resources to</param>
		/// <param name="amount">The amount of resources to try to give them</param>
		/// <returns>The amount of resources actually given (can be less than the requested amount due to storage caps)</returns>
		public static int GrantLifeforce(Vector2 source, Player player, int amount)
		{
			ResourcePlayer mp = player.GetModPlayer<ResourcePlayer>();
			int realAmount = mp.lifeforceAmount + amount < mp.GetMaxFromStorageLevel() ? amount : mp.GetMaxFromStorageLevel() - mp.lifeforceAmount;
			mp.lifeforceAmount += realAmount;

			SpawnResourceDust(source, player, realAmount, new Color(20, 155, 255));

			return realAmount;
		}
	}

	internal class ResourceDrops : GlobalNPC
	{
		/// <summary>
		/// Adds experience and resource rewards to killing enemies
		/// </summary>
		/// <param name="npc"></param>
		public override void OnKill(NPC npc)
		{
			int aurumReward = (int)(npc.lifeMax * 0.05f * Main.rand.NextFloat(0.5f, 1.5f));
			int lifeforceReward = (int)(npc.lifeMax * 0.1f * Main.rand.NextFloat(0.5f, 1.5f));
			int expReward = (int)(npc.lifeMax * 0.01f * Main.rand.NextFloat(0.5f, 1.5f));

			foreach (Player player in Main.player.Where(n => Vector2.Distance(n.Center, npc.Center) < 2560))
			{
				ResourceDropHelper.GrantAurum(npc.Center, player, aurumReward);
				ResourceDropHelper.GrantLifeforce(npc.Center, player, lifeforceReward);
			}
		}
	}
}
