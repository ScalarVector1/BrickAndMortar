namespace BrickAndMortar.Core
{
	interface IOrderedLoadable
	{
		void Load();
		void Unload();
		float Priority { get; }
	}
}
