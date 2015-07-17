using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Drawing;
//using WobbrockLib;
//using WobbrockLib.Extensions;

namespace TemplateGesture{
	public class DynamicTimeWraping{
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

//			for (int i = 0; i < lengthB; i++)
//			{
//				for (int j = 0; j < lengthA; j++)
//				{
//					Debug.Log(dMatrix[i, j]);
//					Debug.Log(" ");
//				}
//				Debug.Log("\n");
//			}
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
			
//			Debug.Log("===========");
//			for (int i = 0; i < lengthB; i++)
//			{
//				for (int j = 0; j < lengthA; j++)
//				{
//					Debug.Log(rMatrix[i, j]);
//					Debug.Log(" ");
//				}
//				Debug.Log("\n");
//			}
			
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
		
		
//		static void Main(string[] args)
//		{
//			Vector2[] time_series_A = { new Vector2(-0.87f,-0.88f), new Vector2(-0.84f,-0.91f), new Vector2(-0.85f,-0.84f), new Vector2(-0.82f,-0.82f), new Vector2(-0.23f,-0.24f), 
//				new Vector2(1.95f,1.92f), new Vector2(1.36f,1.41f), new Vector2(0.60f,0.51f), new Vector2(0.0f,0.03f), new Vector2(-0.29f,-0.18f)};
//			Vector2[] time_series_B = { new Vector2(-0.60f,-0.46f), new Vector2(-0.65f,-0.62f), new Vector2(-0.71f,-0.68f), new Vector2(-0.58f,-0.63f), new Vector2(-0.17f,-0.32f), 
//				new Vector2(0.77f,0.74f), new Vector2(1.94f,1.97f)};
//			int lengthA = time_series_A.Length;
//			int lengthB = time_series_B.Length;
//			
//			float[,] dMatrix = GetDistanceMatrix(time_series_A, time_series_B, lengthA, lengthB);
//			
//			float[,] rMatrix = GetDTWMatrix(dMatrix, lengthA, lengthB);
//			
//			List<Vector2> path = GetOptimalPath(rMatrix, lengthA, lengthB);
//			
//			foreach (var p in path) {
//				Debug.Log(p.x.ToString() + " " + p.y.ToString());
//			}
//		}
	}
}