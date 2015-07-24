using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Drawing;
using WobbrockLib;
using WobbrockLib.Extensions;

namespace TemplateGesture{
	public class DynamicTimeWraping{

		public static readonly double DiagonalD = Math.Sqrt(DX * DX + DX * DX);
		public static readonly double HalfDiagonal = 0.5 * DiagonalD;
		private static readonly double Phi = 0.5 * (-1.0 + Math.Sqrt(5.0)); // Golden Ratio
		private const float DX = 250f;
		public static readonly SizeF SquareSize = new SizeF(DX, DX);
		public static readonly PointF Origin = new PointF(0f, 0f);
		
		static readonly float ReductionFactor = 0.5f * (-1 + (float)Math.Sqrt(5));
		static readonly float Diagonal = (float)Math.Sqrt(2);

		static public float DistanceBetween2Pointf(PointF a, PointF b)
		{
			return (float)(Math.Pow((a.X - b.X), 2) + Math.Pow((a.Y - b.Y), 2));
		}
		
		public static float[,] GetDistanceMatrix(List<PointF> time_series_A, List<PointF> time_series_B, int lengthA, int lengthB) {
			
			float[,] dMatrix = new float[lengthB, lengthA];
			
			for (int i = 0; i < lengthB; i++)
			{
				for (int j = 0; j < lengthA; j++)
				{
					dMatrix[i, j] = DistanceBetween2Pointf(time_series_A[j], time_series_B[i]);
				}
			}

			return dMatrix;
		}
		public static float[,] GetDTWMatrix(float[,] dMatrix, int lengthA, int lengthB) {
			
			float[,] rMatrix = new float[lengthB, lengthA];
			
			rMatrix[0, 0] = dMatrix[0, 0];
			for (int i = 1; i < lengthB; i++)
			{
				rMatrix[i, 0] = rMatrix[i - 1, 0] + dMatrix[i, 0];
			}
			for (int i = 1; i < lengthA; i++)
			{
				rMatrix[0, i] = rMatrix[0, i - 1] + dMatrix[0, i];
			}
			
			for (int i = 1; i < lengthB; i++)
			{
				for (int j = 1; j < lengthA; j++)
				{
					
					rMatrix[i, j] = dMatrix[i, j] + Math.Min(Math.Min(rMatrix[i - 1, j - 1], rMatrix[i, j - 1]), rMatrix[i - 1, j]);
					
				}
			}
			
			return rMatrix;
		}
	
		public static List<UnityEngine.Vector2> GetOptimalPath(float[,] dtwMatrix, int lengthA, int lengthB) {
			
			List<UnityEngine.Vector2> res = new List<UnityEngine.Vector2>();
			
			int row = lengthB - 1;
			int col = lengthA - 1;
			while (row > 0 && col > 0)
			{
				if (dtwMatrix[row -1 , col] == Math.Min(Math.Min(dtwMatrix[row,col-1], dtwMatrix[row -1, col]), dtwMatrix[row -1, col -1])) {
					row -= 1;
				}
				else if (dtwMatrix[row, col - 1] == Math.Min(Math.Min(dtwMatrix[row, col - 1], dtwMatrix[row - 1, col]), dtwMatrix[row - 1, col - 1]))
				{
					col -= 1;
				}
				else
				{
					row -= 1;
					col -= 1;
				}
				res.Add(new UnityEngine.Vector2(row, col));
			}
			
			return res;
		}

		public static float GetPathLength(float[,] dMat, List<UnityEngine.Vector2> path){
			float res = 0;

			foreach (var p in path) {
				res += dMat[(int)p.x,(int)p.y];
			}

			return res;
		}

		public static List<PointF> DTWPack(List<TimePointF> pos, int sampleCount){
			
			//			List<TimePointF> rawPoints = new List<TimePointF> (pos);
			double I = GeotrigEx.PathLength (pos) / (sampleCount);
			List<PointF> localPoints = TimePointF.ConvertList (SeriesEx.ResampleInSpace (pos, I));
			//			double radians = GeotrigEx.Angle (GeotrigEx.Centroid (localPoints), localPoints [0], false);
			//			localPoints = GeotrigEx.RotatePoints (localPoints, -radians);
			localPoints = NormalizeTo (localPoints, 100);
			localPoints = TranslateTo (localPoints, Origin);
			
			
			return localPoints;
		}

		static List<PointF> TranslateTo (List<PointF> points, PointF dest){

			List<PointF> res = new List<PointF> (points);
			Vector2 movement = new Vector2(dest.X - res[0].X, dest.Y - res[0].Y);
//			Debug.Log (movement);
			for (int i =0; i < res.Count; i ++) {
				Vector2 newP = new Vector2(res[i].X, res[i].Y) + movement;
				res[i] = new PointF(newP.x, newP.y);
			}

			return res; 
		}

		static List<PointF> NormalizeTo (List<PointF> points, int size){
			List<PointF> res = new List<PointF> (points);

			return res;
		}
	}
}