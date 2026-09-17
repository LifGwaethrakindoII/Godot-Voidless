using Godot;
using System;

namespace Voidless
{
	public class VRandom
	{
		/// <returns>Random sign (either -1f or 1f).</returns>
		public static float Sign()
		{
			return GD.Randf() > 0.5f ? 1f : -1f;
		}
		
		/// <returns>Random float number between min and max.</returns>
		public static float Range(float min, float max)
		{
			return (float)GD.RandRange(min, max);
		}
		
		/// <returns>Random int number between min and max.</returns>
		public static int Range(int min, int max)
		{
			return (int)GD.RandRange(min, max);
		}
	}
}
