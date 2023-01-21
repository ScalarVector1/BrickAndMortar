using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.DataStructures;
using Terraria.Utilities;

namespace BrickAndMortar.Helpers
{
	public static partial class Helper
	{
		public static Rectangle ToRectangle(this Vector2 vector)
		{
			return new Rectangle(0, 0, (int)vector.X, (int)vector.Y);
		}

		public static bool OnScreen(Vector2 pos)
		{
			return pos.X > -16 && pos.X < Main.screenWidth + 16 && pos.Y > -16 && pos.Y < Main.screenHeight + 16;
		}

		public static bool OnScreen(Rectangle rect)
		{
			return rect.Intersects(new Rectangle(0, 0, Main.screenWidth, Main.screenHeight));
		}

		public static bool OnScreen(Vector2 pos, Vector2 size)
		{
			return OnScreen(new Rectangle((int)pos.X, (int)pos.Y, (int)size.X, (int)size.Y));
		}

		public static Vector3 Vec3(this Vector2 vector)
		{
			return new Vector3(vector.X, vector.Y, 0);
		}

		public static Vector3 ScreenCoord(this Vector3 vector)
		{
			return new Vector3(-1 + vector.X / Main.screenWidth * 2, (-1 + vector.Y / Main.screenHeight * 2f) * -1, 0);
		}

		public static Vector2 Centeroid(List<NPC> input) //Helper overload for NPCs for support NPCs
		{
			var centers = new List<Vector2>();

			for (int k = 0; k < input.Count; k++)
				centers.Add(input[k].Center);

			return Centeroid(centers);
		}

		public static Vector2 Centeroid(List<Vector2> input) //this gets the centeroid of the points. see: https://math.stackexchange.com/questions/1801867/finding-the-centre-of-an-abritary-set-of-points-in-two-dimensions
		{
			float sumX = 0;
			float sumY = 0;

			for (int k = 0; k < input.Count; k++)
			{
				sumX += input[k].X;
				sumY += input[k].Y;
			}

			return new Vector2(sumX / input.Count, sumY / input.Count);
		}

		public static bool CheckLinearCollision(Vector2 point1, Vector2 point2, Rectangle hitbox, out Vector2 intersectPoint)
		{
			intersectPoint = Vector2.Zero;

			return
				LinesIntersect(point1, point2, hitbox.TopLeft(), hitbox.TopRight(), out intersectPoint) ||
				LinesIntersect(point1, point2, hitbox.TopLeft(), hitbox.BottomLeft(), out intersectPoint) ||
				LinesIntersect(point1, point2, hitbox.BottomLeft(), hitbox.BottomRight(), out intersectPoint) ||
				LinesIntersect(point1, point2, hitbox.TopRight(), hitbox.BottomRight(), out intersectPoint);
		}

		//algorithm taken from http://web.archive.org/web/20060911055655/http://local.wasp.uwa.edu.au/~pbourke/geometry/lineline2d/
		public static bool LinesIntersect(Vector2 point1, Vector2 point2, Vector2 point3, Vector2 point4, out Vector2 intersectPoint)
		{
			intersectPoint = Vector2.Zero;

			float denominator = (point4.Y - point3.Y) * (point2.X - point1.X) - (point4.X - point3.X) * (point2.Y - point1.Y);

			float a = (point4.X - point3.X) * (point1.Y - point3.Y) - (point4.Y - point3.Y) * (point1.X - point3.X);
			float b = (point2.X - point1.X) * (point1.Y - point3.Y) - (point2.Y - point1.Y) * (point1.X - point3.X);

			if (denominator == 0)
			{
				if (a == 0 || b == 0) //lines are coincident
				{
					intersectPoint = point3; //possibly not the best fallback?
					return true;
				}
				else
				{
					return false; //lines are parallel
				}
			}

			float ua = a / denominator;
			float ub = b / denominator;

			if (ua > 0 && ua < 1 && ub > 0 && ub < 1)
			{
				intersectPoint = new Vector2(point1.X + ua * (point2.X - point1.X), point1.Y + ua * (point2.Y - point1.Y));
				return true;
			}

			return false;
		}

		public static bool CheckCircularCollision(Vector2 center, int radius, Rectangle hitbox)
		{
			if (Vector2.Distance(center, hitbox.TopLeft()) <= radius)
				return true;

			if (Vector2.Distance(center, hitbox.TopRight()) <= radius)
				return true;

			if (Vector2.Distance(center, hitbox.BottomLeft()) <= radius)
				return true;

			return Vector2.Distance(center, hitbox.BottomRight()) <= radius;
		}

		public static bool CheckConicalCollision(Vector2 center, int radius, float angle, float width, Rectangle hitbox)
		{
			if (CheckPoint(center, radius, hitbox.TopLeft(), angle, width))
				return true;

			if (CheckPoint(center, radius, hitbox.TopRight(), angle, width))
				return true;

			if (CheckPoint(center, radius, hitbox.BottomLeft(), angle, width))
				return true;

			return CheckPoint(center, radius, hitbox.BottomRight(), angle, width);
		}

		private static bool CheckPoint(Vector2 center, int radius, Vector2 check, float angle, float width)
		{
			float thisAngle = (center - check).ToRotation() % 6.28f;
			return Vector2.Distance(center, check) <= radius && thisAngle > angle - width && thisAngle < angle + width;
		}

		public static bool PointInTile(Vector2 point)
		{
			var startCoords = new Point16((int)point.X / 16, (int)point.Y / 16);
			for (int x = -1; x <= 1; x++)
			{
				for (int y = -1; y <= 1; y++)
				{
					Point16 thisPoint = startCoords + new Point16(x, y);

					if (!WorldGen.InWorld(thisPoint.X, thisPoint.Y))
						return false;

					Tile tile = Framing.GetTileSafely(thisPoint);

					if (Main.tileSolid[tile.TileType] && tile.HasTile)
					{
						var rect = new Rectangle(thisPoint.X * 16, thisPoint.Y * 16, 16, 16);

						if (rect.Contains(point.ToPoint()))
							return true;
					}
				}
			}

			return false;
		}

		public static string WrapString(string input, int length, DynamicSpriteFont font, float scale)
		{
			string output = "";
			string[] words = input.Split();

			string line = "";
			foreach (string str in words)
			{
				if (str == "NEWBLOCK")
				{
					output += "\n\n";
					line = "";
					continue;
				}

				if (font.MeasureString(line).X * scale < length)
				{
					output += " " + str;
					line += " " + str;
				}
				else
				{
					output += "\n" + str;
					line = str;
				}
			}

			return output[1..];
		}

		public static List<T> RandomizeList<T>(List<T> input)
		{
			int n = input.Count();

			while (n > 1)
			{
				n--;
				int k = Main.rand.Next(n + 1);
				(input[n], input[k]) = (input[k], input[n]);
			}

			return input;
		}

		public static List<T> RandomizeList<T>(List<T> input, UnifiedRandom rand)
		{
			int n = input.Count();

			while (n > 1)
			{
				n--;
				int k = rand.Next(n + 1);
				(input[n], input[k]) = (input[k], input[n]);
			}

			return input;
		}

		public static Player FindNearestPlayer(Vector2 position)
		{
			Player Player = null;

			for (int k = 0; k < Main.maxPlayers; k++)
			{
				if (Main.player[k] != null && Main.player[k].active && (Player == null || Vector2.DistanceSquared(position, Main.player[k].Center) < Vector2.DistanceSquared(position, Player.Center)))
					Player = Main.player[k];
			}

			return Player;
		}

		public static float BezierEase(float time)
		{
			return time * time / (2f * (time * time - time) + 1f);
		}

		public static float SwoopEase(float time)
		{
			return 3.75f * (float)Math.Pow(time, 3) - 8.5f * (float)Math.Pow(time, 2) + 5.75f * time;
		}

		public static float Lerp(float a, float b, float f)
		{
			return a * (1.0f - f) + b * f;
		}
	}
}

