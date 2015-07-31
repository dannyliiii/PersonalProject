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
		public static readonly MyMath.Vector3 Origin3D = new MyMath.Vector3(0f, 0f, 0f);
		static MyMath.Vector2 size = new MyMath.Vector2(1f, 1f);
		static MyMath.Vector3 size3D = new MyMath.Vector3(1.0f, 1.0f, 1.0f);
		private const float DX = 250f;
		public static readonly SizeF SquareSize = new SizeF(DX, DX);
		
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
					dMatrix[i, j] = Mathf.Sqrt(DistanceBetween2Pointf(time_series_A[j], time_series_B[i]));
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
//			UnityEngine.Debug.Log ("¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬¬");
			localPoints = GeotrigEx.ScaleTo (localPoints, SquareSize);
//			localPoints = Normalize (localPoints);
//			localPoints = GeotrigEx.TranslateTo (localPoints, Origin, true);
			
			localPoints = TranslateTo (localPoints, Origin);
			return localPoints;
		}

		public static List<PointF> NormalizeDTW(List<TimePointF> pos){
			List<PointF> localPoints = new List<PointF> ();
			foreach (var p in pos) {
				localPoints.Add(new PointF(p.X, p.Y));
			}

			float minX = localPoints [0].X;
			float maxX = localPoints [0].X;
			float minY = localPoints [0].Y;
			float maxY = localPoints [0].Y;
			foreach (var lp in localPoints) {
				if(lp.X > maxX)
					maxX = lp.X;
				if(lp.X < minX)
					minX = lp.X;
				if(lp.Y > maxY)
					maxY = lp.Y;
				if(lp.Y < minY)
					minY = lp.Y;
			}

			for (int i = 0; i < localPoints.Count; i++) {
				localPoints[i] = new PointF((localPoints[i].X - minX )/ (maxX - minX), (localPoints[i].Y - minY )/ (maxY - minY));
			}

//			localPoints = GeotrigEx.TranslateTo (localPoints, Origin, true);
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
			UnityEngine.Debug.Log ("==========");	
			UnityEngine.Debug.Log (listSize.x);
			UnityEngine.Debug.Log (listSize.y);
			float x,y;
			if(listSize.x > size.x){
				x = listSize.x / size.x;
			}else{
				x = size.x / listSize.x;
			}

			if(listSize.y > size.y){
				y = listSize.y / size.y;
			}else{
				y = size.y / listSize.y;
			}
			Matrix2 scale = new Matrix2(x, 0, 0, y);

			List<PointF> res = new List<PointF> (points);
//			UnityEngine.Debug.Log ("===========");
//			UnityEngine.Debug.Log (scale.value[0,0]);
//			UnityEngine.Debug.Log (scale.value[0,1]);
//			UnityEngine.Debug.Log (scale.value[1,0]);
//			UnityEngine.Debug.Log (scale.value[1,1]);

			for(int i = 0; i < res.Count; i ++){
				MyMath.Vector2 temp = new MyMath.Vector2(res[i].X, res[i].Y);
//				UnityEngine.Debug.Log ("~~~~~~~~~~~~~");
//				UnityEngine.Debug.Log(temp.x);
//				UnityEngine.Debug.Log(temp.y);
				temp = temp.MultiplyBy(scale);
//				UnityEngine.Debug.Log(temp.x);
//				UnityEngine.Debug.Log(temp.y);
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

//			UnityEngine.Debug.Log ("~~~~~~~~~~~~~~~~~~~");
//			UnityEngine.Debug.Log (left);
//			UnityEngine.Debug.Log (right);
//			UnityEngine.Debug.Log (top);
//			UnityEngine.Debug.Log (bottom);

			return new MyMath.Vector2(Math.Abs(left - right), Math.Abs(top - bottom));
		}

		public static List<MyMath.Vector3> DTWPack3D(List<MyMath.Vector3> pos, int sampleCount){
			
			float I = PathLength3D(pos) / (sampleCount);
			List<MyMath.Vector3> localPoints = Resample(pos, I);
			
//			localPoints = Normalize3D (localPoints);

			MyMath.Vector3 size = GetSize3D (localPoints);
			localPoints = ScaleTo3D (localPoints, size);

			MyMath.Vector3 centrod = GetCentroidPoint3D(localPoints);
			localPoints = TranslateTo3D (localPoints, Origin3D, centrod);
			
			return localPoints;
		}

		public static float PathLength3D(List<MyMath.Vector3> list){
			float res = 0;

			for(int i = 1; i < list.Count; i++){
				res += GetDistanceBetween2Vector3(list[i], list[i-1]);
			}

			return res;
		}

		public static float PathLength(List<PointF> list){
			float res = 0;
			
			for(int i = 1; i < list.Count; i++){
				res += DistanceBetween2Pointf(list[i], list[i-1]);
			}
			
			return res;
		}

		static float GetDistanceBetween2Vector3(MyMath.Vector3 v1, MyMath.Vector3 v2){
			float length = Mathf.Sqrt(Mathf.Pow((v2.x - v1.x),2) + Mathf.Pow((v2.x - v1.x),2) + Mathf.Pow((v2.x - v1.x),2));

			return length;
		}

		static List<MyMath.Vector3> Resample(List<MyMath.Vector3> list, float I){
			List<MyMath.Vector3> res = new List<MyMath.Vector3>(list);

			float D = 0;
			MyMath.Vector3 point = list[0];
			for(int i = 1; i < res.Count; i ++){
				float d = GetDistanceBetween2Vector3(list[i], list[i-1]);
				if((D + d) >= I){

				}
			}

			return res;
		}

		static List<MyMath.Vector3> TranslateTo3D (List<MyMath.Vector3> points, MyMath.Vector3 dest, MyMath.Vector3 centroid){

			MyMath.Vector3 moveVector = dest - centroid;
			List<MyMath.Vector3> res = new List<MyMath.Vector3> (points);
			MyMath.Vector3 movement = new MyMath.Vector3(moveVector.x - res[0].x, moveVector.y - res[0].y, moveVector.z - res[0].z);
			//			Debug.Log (movement);
			for (int i =0; i < res.Count; i ++) {
				MyMath.Vector3 newP = new MyMath.Vector3(res[i].x, res[i].y, res[i].z) + movement;
				res[i] = newP;
			}
			
			return res; 
		}
		
		static List<MyMath.Vector3> Normalize3D (List<MyMath.Vector3> points){
			
			MyMath.Vector3 listSize = GetSize3D(points);
			float x,y,z;
			if(listSize.x > size3D.x){
				x = size3D.x / listSize.x;
			}else{
				x = listSize.x / size3D.x;
			}
			
			if(listSize.y > size3D.y){
				y = size3D.y / listSize.y;
			}else{
				y = listSize.y / size3D.y;
			}

			if(listSize.z > size3D.z){
				z = size3D.z / listSize.z;
			}else{
				z = listSize.z / size3D.z;
			}

			Matrix3 scale = new Matrix3(x, 0, 0,
			                            0, y, 0,
			                            0, 0, z);
			
			List<MyMath.Vector3> res = new List<MyMath.Vector3> (points);
			
			for(int i = 0; i < res.Count; i ++){
				MyMath.Vector3 temp = new MyMath.Vector3(res[i].x, res[i].y, res[i].z);
				temp = temp.MultiplyBy(scale);
				res[i] = temp;
			}
			
			return res;
		}
		
		static MyMath.Vector3 GetSize3D(List<MyMath.Vector3> points){
			float left = points[0].x;
			float right = points[0].x;
			float top = points[0].y;
			float bottom = points[0].y;
			float front = points[0].z;
			float back = points[0].z;
			for(int i = 1; i < points.Count; i ++){
				if(points[i].x > right){
					right = points[i].x;
				}else{
					left = points[i].x;
				}
				if(points[i].y > top){
					top = points[i].y;
				}else{
					bottom = points[i].y;
				}
				if(points[i].z > front){
					front = points[i].z;
				}else{
					back = points[i].z;
				}
			}
			
			return new MyMath.Vector3(Mathf.Abs(left - right), Mathf.Abs(top - bottom), Mathf.Abs(front - back));
		}

		static MyMath.Vector3 GetCentroidPoint3D(List<MyMath.Vector3> points){
			int n = points.Count;
			float x = 0;
			float y = 0;
			float z = 0;
			foreach (var p in points) {
				x += p.x;
				y += p.y;
				z += p.z;
			}

			return new MyMath.Vector3 (x / n, y / n, z / n);
		}

		static List<MyMath.Vector3> ScaleTo3D(List<MyMath.Vector3> points, MyMath.Vector3 size){
			List<MyMath.Vector3> res = new List<MyMath.Vector3> ();
			float xMutiplier = size.x / size3D.x;
			float yMutiplier = size.y / size3D.y;
			float zMutiplier = size.z / size3D.z;
			for (int i = 0; i < points.Count; i ++) {
				res.Add(new MyMath.Vector3(points[i].x * xMutiplier,
									       points[i].y * yMutiplier,
									       points[i].z * zMutiplier));
			}

			return res;
		}

		public static float[,] GetDistanceMatrix3D(List<MyMath.Vector3> time_series_A, List<MyMath.Vector3> time_series_B, int lengthA, int lengthB) {
			
			float[,] dMatrix = new float[lengthB, lengthA];
			for (int i = 0; i < lengthB; i++)
			{
				for (int j = 0; j < lengthA; j++)
				{
					dMatrix[i, j] = Mathf.Sqrt(GetDistanceBetween2Vector3(time_series_A[j], time_series_B[i]));
				}
			}
			
			return dMatrix;
		}
	}
}