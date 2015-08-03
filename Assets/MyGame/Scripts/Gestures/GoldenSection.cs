using System.Collections;
using System;
using System.Collections.Generic;
using MyMath;
using WobbrockLib;
using WobbrockLib.Extensions;
using System.Drawing;

namespace TemplateGesture{
	public class GoldenSection {

		public static readonly double DiagonalD = Math.Sqrt(DX * DX + DX * DX);
		public static readonly double HalfDiagonal = 0.5 * DiagonalD;
		private static readonly double Phi = 0.5 * (-1.0 + Math.Sqrt(5.0)); // Golden Ratio
		private const float DX = 1f;
		public static readonly SizeF SquareSize = new SizeF(DX, DX);
		public static readonly PointF Origin = new PointF(0f, 0f);

		static readonly float ReductionFactor = 0.5f * (-1 + (float)Math.Sqrt(5));
		static readonly float Diagonal = (float)Math.Sqrt(2);
		
		public static float Search(List<Vector2> current, List<Vector2> target, float a, float b, float epsilon)
		{
			float x1 = ReductionFactor * a + (1 - ReductionFactor) * b;
			List<Vector2> rotatedList = GoldenSectionExtension.Rotate(current, x1);
			float fx1 = GoldenSectionExtension.DistanceTo(rotatedList, target);
			
			float x2 = (1 - ReductionFactor) * a + ReductionFactor * b;
			rotatedList = GoldenSectionExtension.Rotate(current, x2);
			float fx2 = GoldenSectionExtension.DistanceTo(rotatedList, target);

			do
			{
				if (fx1 < fx2)
				{
					b = x2;
					x2 = x1;
					fx2 = fx1;
					x1 = ReductionFactor * a + (1 - ReductionFactor) * b;
					rotatedList = GoldenSectionExtension.Rotate(current, 0x1);
					fx1 = GoldenSectionExtension.DistanceTo(rotatedList, target);
				}
				else
				{
					a = x1;
					x1 = x2;
					fx1 = fx2;
					x2 = (1 - ReductionFactor) * a + ReductionFactor * b;
					rotatedList = GoldenSectionExtension.Rotate(current,x2);
					fx2 = GoldenSectionExtension.DistanceTo(rotatedList, target);
				}
			}
			while (Math.Abs(b - a) > epsilon);
			
			float min = Math.Min(fx1, fx2);
			
			return 1.0f - 2.0f * min / Diagonal;
		}
		
		static List<Vector2> ProjectListToDefinedCount(List<Vector2> positions, int n)
		{
			List<Vector2> source = new List<Vector2>(positions);
			List<Vector2> destination = new List<Vector2>
			{
				source[0]
			};
			
			// define the average length of each segment
			float averageLength = GoldenSectionExtension.Length(positions) / (n - 1);
			float currentDistance = 0;
			
			
			for (int index = 1; index < source.Count; index++)
			{
				Vector2 pt1 = source[index - 1];
				Vector2 pt2 = source[index];
				
				float distance = MathHelper.Sqrt((pt1 - pt2).LengthSqr);
				// If the distance between the 2 points is greater than average length, we introduce a new point
				if ((currentDistance + distance) >= averageLength)
				{
					Vector2 newPoint = pt1 + ((pt2 - pt1) *(averageLength - currentDistance) / distance) ;
					
					destination.Add(newPoint);
					source.Insert(index, newPoint);
					currentDistance = 0;
				}
				else
				{
					// merging points by ignoring it
					currentDistance += distance;
				}
			}
			
			if (destination.Count < n)
			{
				destination.Add(source[source.Count - 1]);
			}
			
			return destination;
		}
		
		// A bit of trigonometry
		public static float GetAngleBetween(Vector2 start, Vector2 end)
		{
			if (start.x != end.x)
			{
				return (float)Math.Atan2(end.y - start.y, end.x - start.x);
			}
			
			if (end.y > start.y)
				return MathHelper.PiOver2;
			
			return -MathHelper.PiOver2;
		}
		
		// Resample to required length then rotate to get first point at 0 radians, scale to 1x1 and finally center the path to (0,0)
		public static List<Vector2> Pack(List<Vector3> positions, int samplesCount)
		{
			List<Vector2> locals = ProjectListToDefinedCount(ToLV2(positions), samplesCount);
			
			float angle = GetAngleBetween(GoldenSectionExtension.Center(locals), Vector3.ToVector2(positions[0]));
			locals = GoldenSectionExtension.Rotate(locals, -angle);
			
			GoldenSectionExtension.ScaleToReferenceWorld(locals);
			GoldenSectionExtension.CenterToOrigin(locals);
			
			return locals;
		}

		public static List<Vector2> Pack(List<Vector2> positions, int samplesCount)
		{
			List<Vector2> locals = ProjectListToDefinedCount(positions, samplesCount);
			
			float angle = GetAngleBetween(GoldenSectionExtension.Center(locals), positions[0]);
			locals = GoldenSectionExtension.Rotate(locals, -angle);
			
			GoldenSectionExtension.ScaleToReferenceWorld(locals);
			GoldenSectionExtension.CenterToOrigin(locals);
			
			return locals;
		}

		private static List<Vector2> ToLV2(List<Vector3> pos){
			List<Vector2> res = new List<Vector2>(256);

			for (int i = 0; i < pos.Count; i ++) {
				Vector2 v2 = new Vector2(pos[i].x, pos[i].y);
				res.Add(v2);
			}

			return res;
		}

		public static List<Vector2> Scale(List<Vector3> positions, int samplesCount){
			List<Vector2> locals = ToLV2(positions);

			for (int i = 0; i < locals.Count; i ++) {
				locals[i].x = -locals[i].x;
			}

			GoldenSectionExtension.ScaleToReferenceWorld (locals);
			GoldenSectionExtension.CenterToOrigin (locals);

			return locals;
		}

		public static List<PointF> DollarOnePack(List<TimePointF> pos, int sampleCount){

//			List<TimePointF> rawPoints = new List<TimePointF> (pos);
			double I = GeotrigEx.PathLength (pos) / (sampleCount);
			List<PointF> localPoints = TimePointF.ConvertList (SeriesEx.ResampleInSpace (pos, I));
//			double radians = GeotrigEx.Angle (GeotrigEx.Centroid (localPoints), localPoints [0], false);
//			localPoints = GeotrigEx.RotatePoints (localPoints, -radians);
			localPoints = GeotrigEx.ScaleTo (localPoints, SquareSize);
			localPoints = GeotrigEx.TranslateTo (localPoints, Origin, true);


			return localPoints;
		}

		public static double[] GoldenSectionSearch(List<PointF> pts1, List<PointF> pts2, double a, double b, double threshold)
		{
			double x1 = Phi * a + (1 - Phi) * b;
			List<PointF> newPoints = GeotrigEx.RotatePoints(pts1, x1);
			double fx1 = PathDistance(newPoints, pts2);
			
			double x2 = (1 - Phi) * a + Phi * b;
			newPoints = GeotrigEx.RotatePoints(pts1, x2);
			double fx2 = PathDistance(newPoints, pts2);
			
			double i = 2.0; // calls to pathdist
			while (Math.Abs(b - a) > threshold)
			{
				if (fx1 < fx2)
				{
					b = x2;
					x2 = x1;
					fx2 = fx1;
					x1 = Phi * a + (1 - Phi) * b;
					newPoints = GeotrigEx.RotatePoints(pts1, x1);
					fx1 = PathDistance(newPoints, pts2);
				}
				else
				{
					a = x1;
					x1 = x2;
					fx1 = fx2;
					x2 = (1 - Phi) * a + Phi * b;
					newPoints = GeotrigEx.RotatePoints(pts1, x2);
					fx2 = PathDistance(newPoints, pts2);
				}
				i++;
			}
			return new double[3] { Math.Min(fx1, fx2), GeotrigEx.Radians2Degrees((b + a) / 2.0), i }; // distance, angle, calls to pathdist
		}

		public static double PathDistance(List<PointF> path1, List<PointF> path2)
		{
			double distance = 0;
			for (int i = 0; i < Math.Min(path1.Count, path2.Count); i++)
			{
				distance += GeotrigEx.Distance(path1[i], path2[i]);
			}
			return distance / path1.Count;
		}
		
	}
}
