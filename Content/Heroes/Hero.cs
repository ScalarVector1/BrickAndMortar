using BrickAndMortar.Content.Buildings;
using System;
using System.IO;
using Terraria.ModLoader.IO;

namespace BrickAndMortar.Content.Heroes
{
	internal abstract class Hero : ModNPC
	{
		public Building parent;

		public int whoAmIFollowing;

		public HeroAttack primaryAttack;
		public HeroAttack secondaryAttack;
		public HeroAttack activeAttack;

		/// <summary>
		/// Global timer for this hero. Will be ticked automaticall.
		/// </summary>
		public ref float Timer => ref NPC.ai[0];
		/// <summary>
		/// Timer passed to attacks for use. Will be reset when an attack returns true from Activate.
		/// </summary>
		public ref float AttackTimer => ref NPC.ai[1];
		/// <summary>
		/// Global state for the hero. Can be used to track things like Follow VS Wander VS Attack.
		/// </summary>
		public ref float State => ref NPC.ai[2];
		/// <summary>
		/// Which attack the hero should be using. 0 = primary, 1 = secondary, 2 = active, -1 = nothing
		/// </summary>
		public ref float AttackState => ref NPC.ai[3];

		public Player Following => Main.player[whoAmIFollowing];

		public int Level => parent.level;

		public int EffectiveLevel => parent.EffectiveLevel;

		public sealed override void SetDefaults()
		{
			NPC.friendly = true;
			NPC.dontCountMe = true;
			NPC.lifeMax = 20;
			NPC.damage = 1;
			SafeSetDefaults();
			RefreshStats();
		}

		/// <summary>
		/// Set things like width/height here only. Use RefreshStats to set things such as like and damage based on leveling.
		/// </summary>
		public virtual void SafeSetDefaults() { }

		/// <summary>
		/// Where you should set stats like life, damage, etc. based on EffectiveLevel. To be called on leveling the altar and creation.
		/// </summary>
		public virtual void RefreshStats() { }

		public sealed override void AI()
		{
			Timer++;
			AttackTimer++;

			bool shouldReset = AttackState switch
			{
				0 => primaryAttack.Activate((int)AttackTimer),
				1 => secondaryAttack.Activate((int)AttackTimer),
				2 => activeAttack.Activate((int)AttackTimer),
				_ => true
			};

			if (shouldReset)
			{
				AttackTimer = 0;
				AttackState = -1;
			}
		}

		/// <summary>
		/// This is the primary method you should be overriding for general behavior like movement. Setting AttackState here will determine what happens when AI runs. You can
		/// check for the hero being ready for a new attack by checking if AttackState == -1. Note that this will run before Timer and AttackTimer updates for that frame.
		/// </summary>
		/// <returns>If default AI should run (using attack based on AttackState, resetting AttackTimer and AttackState if attack is over, incrementing Timer and AttackTimer)</returns>
		public override bool PreAI()
		{
			return base.PreAI(); //Yes, I overrode this just for documentation.
		}

		//sync player target
		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(whoAmIFollowing);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			whoAmIFollowing = reader.Read();
		}

		//Save and load utility to remember which abilities this hero has. To be saved/loaded by the altar building's save/load methods.
		public void SaveDataToBuilding(TagCompound tag)
		{
			tag["primary"] = primaryAttack.GetType().ToString();
			tag["secondary"] = primaryAttack.GetType().ToString();
			tag["active"] = primaryAttack.GetType().ToString();
		}

		public void LoadDataFromBuilding(TagCompound tag)
		{
			primaryAttack = (HeroAttack)Activator.CreateInstance(BrickAndMortar.instance.Code.GetType(tag.GetString("primary")));
			secondaryAttack = (HeroAttack)Activator.CreateInstance(BrickAndMortar.instance.Code.GetType(tag.GetString("secondary")));
			activeAttack = (HeroAttack)Activator.CreateInstance(BrickAndMortar.instance.Code.GetType(tag.GetString("active")));
		}
	}
}
