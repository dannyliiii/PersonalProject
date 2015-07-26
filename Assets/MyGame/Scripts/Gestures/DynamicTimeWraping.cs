using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Drawing;
using WobbrockLib;
using WobbrockLib.Extensions;
using MyMath;

namespace TemplateGesture{
	public class DynamicTimeWraping{
	
		public static readonly PointF Origin = new PointF(0f, 0f);
		static MyMath.Vector2 size = new MyMath.Vector2(500.0f, 500.0f);

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

			localPoints = Normalize (localPoints);
			localPoints = TranslateTo (localPoints, Origin);

			return localPoints;
		}

		static List<PointF> TranslateTo (List<PointF> points, PointF dest){

			List<PointF> res = new List<PointF> (points);
			MyMath.Vector2 movement = new MyMath.Vector2(dest.X - res[0].X, dest.Y - res[0].Y);
//			Debug.Log (movement);
			for (int i =0; i < res.Count; i ++) {
				MyMath.Vector2 newP = new MyMath.Vector2(res[i].X, res[i].Y) + movement;
				res[i] = new PointF(newP.x, newP.y);
			}

			return res; 
		}

		static List<PointF> Normalize (List<PointF> points){

			MyMath.Vector2 listSize = GetSize(points);
			float x,y;
			if(listSize.x > size.x){
				x = size.x / listSize.x;
			}else{
				x = listSize.x / size.x;
			}

			if(listSize.y > size.y){
				y = size.y / listSize.y;
			}else{
				y = listSize.y / size.y;
			}
			Matrix2 scale = new Matrix2(x, 0, 0, y);

			List<PointF> res = new List<PointF> (points);

			for(int i = 0; i < res.Count; i ++){
				MyMath.Vector2 temp = new MyMath.Vector2(res[i].X, res[i].Y);
				temp = temp.MultiplyBy(scale);
				res[i] = new PointF(temp.x, temp.y);
			}

			return res;
		}

		static MyMath.Vector2 GetSize(List<PointF> points){
			float left = points[0].X;
			float right = points[0].X;
			float top = points[0].Y;
			float bottom = points[0].Y;
			for(int i = 1; i < points.Count; i ++){
				if(points[i].X > right){
					right = points[i].X;
				}else{
					left = points[i].X;
				}
				if(points[i].Y > top){
					top = points[i].Y;
				}else{
					bottom = points[i].Y;
				}
			}

			return new MyMath.Vector2(Math.Abs(left - right), Math.Abs(top - bottom));
		}
	}
}