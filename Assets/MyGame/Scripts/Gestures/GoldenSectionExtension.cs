using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using MyMath;

namespace TemplateGesture{
	public class GoldenSectionExtension {

		// Get length of path
		public static float Length(List<Vector2> points)
		{
			float lengthSqr = 0;
			
			for (int i = 1; i < points.Count; i++)
			{
				lengthSqr += (points[i - 1] - points[i]).LengthSqr;
			}
			
			return MathHelper.Sqrt(lengthSqr);
		}
		
		// Get center of path
		public static Vector2 Center(List<Vector2> points)
		{
			Vector2 result = points.Aggregate(Vector2.Zero, (current, point) => current + point);
			
			result /= points.Count;
			
			return result;
		}
		
		// Rotate path by given angle
		public static List<Vector2> Rotate(List<Vector2> positions, float angle)
		{
			List<Vector2> result = new List<Vector2>(positions.Count);
			Vector2 c = Center(positions);
			
			float cos = (float)Math.Cos(angle);
			float sin = (float)Math.Sin(angle);
			
			foreach (Vector2 p in positions)
			{
				float dx = p.x - c.x;
				float dy = p.y - c.y;
				
				Vector2 rotatePoint = Vector2.Zero;
				rotatePoint.x = dx * cos - dy * sin + c.x;
				rotatePoint.y = dx * sin + dy * cos + c.y;
				
				result.Add(rotatePoint);
			}
			return result;
		}
		
		// Average distance betweens paths
		public static float DistanceTo(List<Vector2> path1, List<Vector2> path2)
		{
			return path1.Select((t, i) => MathHelper.Sqrt((t - path2[i]).LengthSqr)).Average();
		}
		
		// Compute bounding rectangle
		public static Rectangle BoundingRectangle(List<Vector2> points)
		{
			float minx = points.Min(p => p.x);
			float maxx = points.Max(p => p.x);
			float miny = points.Min(p => p.y);
			float maxy = points.Max(p => p.y);
			
			return new Rectangle(minx, miny, maxx - minx, maxy - miny);
		}
		
		// Check bounding rectangle size
		public static bool IsLargeEnough(List<Vector2> positions, float minSize)
		{
			Rectangle boundingRectangle = BoundingRectangle(positions);
			
			return boundingRectangle.Width > minSize && boundingRectangle.Height > minSize;
		}
		
		// Scale path to 1x1
		public static void ScaleToReferenceWorld(List<Vector2> positions)
		{
			Rectangle boundingRectangle = BoundingRectangle(positions);
			for (int i = 0; i < positions.Count; i++)
			{
				Vector2 position = positions[i];
				
				position.x *= (1.0f / boundingRectangle.Width);
				position.y *= (1.0f / boundingRectangle.Height);
				
				positions[i] = position;
			}
		}
		
		// Translate path to origin (0, 0)
		public static void CenterToOrigin(List<Vector2> positions)
		{
			Vector2 center = Center(positions);
			for (int i = 0; i < positions.Count; i++)
			{
				positions[i] -= center;
			}
		}
	}
}
