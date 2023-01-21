namespace BrickAndMortar.Core.Systems.ResourceSystem
{
	internal class ResourceSpending
	{
		/// <summary>
		/// Attempts to withdraw a given amount of aurum from a player.
		/// </summary>
		/// <param name="player">The player to withdraw from</param>
		/// <param name="amount">The amount to try and withdraw</param>
		/// <returns>If the player had enough and that amount was subtracted from their total.</returns>
		public static bool TrySpendAurum(Player player, int amount)
		{
			ResourcePlayer mp = player.GetModPlayer<ResourcePlayer>();

			if (amount > mp.aurumAmount)
			{
				ErrorMessage("aurum", amount);
				return false;
			}

			mp.aurumAmount -= amount;
			return true;
		}

		/// <summary>
		/// Attempts to withdraw a given amount of lifeforce from a player.
		/// </summary>
		/// <param name="player">The player to withdraw from</param>
		/// <param name="amount">The amount to try and withdraw</param>
		/// <returns>If the player had enough and that amount was subtracted from their total.</returns>
		public static bool TrySpendLifeforce(Player player, int amount)
		{
			ResourcePlayer mp = player.GetModPlayer<ResourcePlayer>();

			if (amount > mp.lifeforceAmount)
			{
				ErrorMessage("lifeforce", amount);
				return false;
			}

			mp.lifeforceAmount -= amount;
			return true;
		}

		/// <summary>
		/// Attempts to withdraw a given amount of both aurum and lifeforce from a player
		/// </summary>
		/// <param name="player">The player to withdraw from</param>
		/// <param name="aurumAmount">The amount of aurum to try and withdraw</param>
		/// <param name="lifeforceAmount">The amount of lifeforce to try and withdraw</param>
		/// <returns>If the player had enough aurum AND lifeforce, and if it was subtracted from their total.</returns>
		public static bool TrySpendBoth(Player player, int aurumAmount, int lifeforceAmount)
		{
			ResourcePlayer mp = player.GetModPlayer<ResourcePlayer>();

			if (aurumAmount > mp.aurumAmount || lifeforceAmount > mp.lifeforceAmount)
			{
				if (aurumAmount > mp.aurumAmount)
					ErrorMessage("aurum", aurumAmount);

				if (lifeforceAmount > mp.lifeforceAmount)
					ErrorMessage("lifeforce", lifeforceAmount);

				return false;
			}

			mp.aurumAmount -= aurumAmount;
			mp.lifeforceAmount -= lifeforceAmount;
			return true;
		}

		/// <summary>
		/// Draws the cost of something, using a description and provided costs.
		/// </summary>
		/// <param name="spriteBatch">The spritebatch to use to draw the cost</param>
		/// <param name="pos">Where on the screen to draw the cost (top-left)</param>
		/// <param name="description">What you're drawing the cost of</param>
		/// <param name="aurumAmount">The amount of aurum that this costs</param>
		/// <param name="lifeforceAmount">The amount of lifeforce that this costs</param>
		public static void DrawCost(SpriteBatch spriteBatch, Vector2 pos, string description, int aurumAmount, int lifeforceAmount)
		{
			ResourcePlayer mp = Main.LocalPlayer.GetModPlayer<ResourcePlayer>();

			Utils.DrawBorderString(spriteBatch, $"Cost to {description}:", pos, Color.White);
			pos.Y += 28;

			if (aurumAmount > 0)
			{
				Texture2D aurumTex = ModContent.Request<Texture2D>("BrickAndMortar/Assets/GUI/AurumIcon").Value;
				spriteBatch.Draw(aurumTex, pos, null, Color.White, 0, Vector2.Zero, 0.5f, 0, 0);
				Utils.DrawBorderString(spriteBatch, $"Aurum: {aurumAmount}", pos + Vector2.UnitX * 20, mp.aurumAmount >= aurumAmount ? Color.White : Color.Red);
				pos.Y += 28;
			}

			if (lifeforceAmount > 0)
			{
				Texture2D lifeforceTex = ModContent.Request<Texture2D>("BrickAndMortar/Assets/GUI/LifeforceIcon").Value;
				spriteBatch.Draw(lifeforceTex, pos, null, Color.White, 0, Vector2.Zero, 0.5f, 0, 0);
				Utils.DrawBorderString(spriteBatch, $"Lifeforce: {aurumAmount}", pos + Vector2.UnitX * 20, mp.aurumAmount >= aurumAmount ? Color.White : Color.Red);
			}
		}

		/// <summary>
		/// Prints an error message to the chat when the player has insufficient resources
		/// </summary>
		/// <param name="resource">Which resource the player is missing</param>
		/// <param name="amount">How much they actually need</param>
		public static void ErrorMessage(string resource, int amount)
		{
			Main.NewText($"Not enough {resource}! You need {amount} {resource} to do this.", Color.Red);
		}
	}
}
