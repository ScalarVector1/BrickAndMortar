using BrickAndMortar.Content.Buildings;

namespace BrickAndMortar.Content.Heroes
{
	internal abstract class HeroAttack
	{
		/// <summary>
		/// The hero who owns this attack instance
		/// </summary>
		public Hero owner;

		/// <summary>
		/// The stat line that this attack should show on the altar building UI when it's selected
		/// </summary>
		public abstract Statline statline { get; }

		/// <summary>
		/// The stat line this attack should show on the altar building UI when it's selected and about to be upgraded
		/// </summary>
		public abstract Statline statlineNext { get; }

		/// <summary>
		/// The behavior of the actual attack when activated. Spawning projectiles, damaging enemies, etc.
		/// </summary>
		/// <param name="time">The attack timer of the hero</param>
		/// <returns>if the attack is 'over' and the hero should reset it's attack state.</returns>
		public abstract bool Activate(int time);
	}
}
