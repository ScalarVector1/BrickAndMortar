global using BrickAndMortar.Core;
global using Microsoft.Xna.Framework;
global using Microsoft.Xna.Framework.Graphics;
global using Terraria;
global using Terraria.ModLoader;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BrickAndMortar
{
	public class BrickAndMortar : Mod
	{
		public static Mod instance;
		private List<IOrderedLoadable> loadCache;

		public BrickAndMortar()
		{
			instance = this;
		}

		public override void Load()
		{
			loadCache = new List<IOrderedLoadable>();

			foreach (Type type in Code.GetTypes())
			{
				if (!type.IsAbstract && type.GetInterfaces().Contains(typeof(IOrderedLoadable)))
				{
					object instance = Activator.CreateInstance(type);
					loadCache.Add(instance as IOrderedLoadable);
				}

				loadCache.Sort((n, t) => n.Priority.CompareTo(t.Priority));
			}

			for (int k = 0; k < loadCache.Count; k++)
			{
				loadCache[k].Load();
			}
		}

		public override void Unload()
		{
			foreach (IOrderedLoadable loadable in loadCache)
			{
				loadable.Unload();
			}

			loadCache = null;
		}
	}
}