using System.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using MyMath;
using UnityEngine;
using WobbrockLib;
using WobbrockLib.Extensions;
using System.Drawing;

namespace TemplateGesture{

	public enum Plane : int{
		xy,
		zy,
		xz
	}

	public class RecordedData {
		public string gestureName;

		List<MyMath.Vector3> points_3D_L;
		List<MyMath.Vector3> points_3D_R;

		List<PointF> lpf_DTW;
		List<PointF> rpf_DTW;
//		double lengthL_DTW;
//		double lengthR_DTW;
//		List<MyMath.Vector2> lPoints_DTW;
//		List<MyMath.Vector2> rPoints_DTW;

		List<MyMath.Vector2> lPoints;
		List<MyMath.Vector2> rPoints;
		List<MyMath.Vector2> zy_lPoints;
		List<MyMath.Vector2> zy_rPoints;
		List<MyMath.Vector2> zx_lPoints;
		List<MyMath.Vector2> zx_rPoints;
		List<PointF> lpf;
		List<PointF> rpf;
		int sampleCount;
		Plane plane;
		bool isLineL; // 0: false, 1: true
		bool isLineR;
		bool isLineL_xz; // 0: false, 1: true
		bool isLineR_xz;
		bool isLineL_yz; // 0: false, 1: true
		bool isLineR_yz;
		public List<Constrain> constrain;
		double angleL;
		double angleR;
		double angleL_XZ;
		double angleR_XZ;
		double angleL_YZ;
		double angleR_YZ;
		bool oneHanded;
		int linePlane;
		double lengthL;
		double lengthR;

		public List<MyMath.Vector3> Points_3D_L{
			get{return points_3D_L;}
			set{points_3D_L = value;}
		}
		public List<MyMath.Vector3> Points_3D_R{
			get{return points_3D_R;}
			set{points_3D_R = value;}
		}
		public List<PointF> LP_DTW{
			get{return lpf_DTW;}
			set{lpf_DTW = value;}
		}
		
		public List<PointF> RP_DTW{
			get{return rpf_DTW;}
			set{rpf_DTW = value;}
		}
//		public List<MyMath.Vector2> LPoints_DTW
//		{
//			get { return lPoints_DTW; }
//			set { lPoints_DTW = value; }
//		}
//		
//		public List<MyMath.Vector2> RPoints_DTW
//		{
//			get { return rPoints_DTW; }
//			set { rPoints_DTW = value; }
//		}
//		public double LengthL_DTW{
//			get{return lengthL_DTW;}
//			set{lengthL_DTW = value;}
//		}
//		public double LengthR_DTW{
//			get{return lengthR_DTW;}
//			set{lengthR_DTW = value;}
//		}

		//----------------------- for the z-y plane ------------------
		List<PointF> zy_lpf;
		List<PointF> zy_rpf;
		public List<PointF> ZY_LP{
			get{return zy_lpf;}
			set{zy_lpf = value;}
		}
		
		public List<PointF> ZY_RP{
			get{return zy_rpf;}
			set{zy_rpf = value;}
		}
		public bool IsLineL_YZ{
			get{return isLineL_yz;}
			set{isLineL_yz = value;}
		}
		public bool IsLineR_YZ{
			get{return isLineR_yz;}
			set{isLineR_yz = value;}
		}
		public double AngleL_YZ{
			get{return angleL_YZ;}
			set{angleL_YZ = value;}
		}
		public double AngleR_YZ{
			get{return angleR_YZ;}
			set{angleR_YZ = value;}
		}
		//----------------------- for the z-y plane ------------------

		//----------------------- for the z-x plane ------------------
		List<PointF> zx_lpf;
		List<PointF> zx_rpf;
		public List<PointF> ZX_LP{
			get{return zx_lpf;}
			set{zx_lpf = value;}
		}
		
		public List<PointF> ZX_RP{
			get{return zx_rpf;}
			set{zx_rpf = value;}
		}
		public bool IsLineL_XZ{
			get{return isLineL_xz;}
			set{isLineL_xz = value;}
		}
		public bool IsLineR_XZ{
			get{return isLineR_xz;}
			set{isLineR_xz = value;}
		}
		public double AngleL_XZ{
			get{return angleL_XZ;}
			set{angleL_XZ = value;}
		}
		public double AngleR_XZ{
			get{return angleR_XZ;}
			set{angleR_XZ = value;}
		}
		//----------------------- for the z-x plane ------------------

		//---------------------- start of getter/setter----------------
		public double LengthL{
			get{return lengthL;}
			set{lengthL = value;}
		}
		public double LengthR{
			get{return lengthR;}
			set{lengthR = value;}
		}
		public Plane Plane{
			get{return plane;}
			set{plane = value;}
		}
		// getter/setter
		public bool IsLineL{
			get{return isLineL;}
			set{isLineL = value;}
		}
		public bool IsLineR{
			get{return isLineR;}
			set{isLineR = value;}
		}

		public double AngleL{
			get{return angleL;}
			set{angleL = value;}
		}
		public double AngleR{
			get{return angleR;}
			set{angleR = value;}
		}

		public bool OnaHanded{
			get{return oneHanded;}
			set{oneHanded = value;}
		}

		public int Size{
			get{
				return lPoints.Count;
			}
		}
		public int SampleCount{
			get{
				return sampleCount;
			}
			set{
				sampleCount = value;
			}
		}

		public List<MyMath.Vector2> LPoints
		{
			get { return lPoints; }
			set { lPoints = value; }
		}

		public List<MyMath.Vector2> RPoints
		{
			get { return rPoints; }
			set { rPoints = value; }
		}

		public List<MyMath.Vector2> ZY_LPoints
		{
			get { return zy_lPoints; }
			set { zy_lPoints = value; }
		}
		
		public List<MyMath.Vector2> ZY_RPoints
		{
			get { return zy_rPoints; }
			set { zy_rPoints = value; }
		}

		public List<MyMath.Vector2> ZX_LPoints
		{
			get { return zx_lPoints; }
			set { zx_lPoints = value; }
		}
		
		public List<MyMath.Vector2> ZX_RPoints
		{
			get { return zx_rPoints; }
			set { zx_rPoints = value; }
		}

		public List<PointF> LP{
			get{return lpf;}
			set{lpf = value;}
		}

		public List<PointF> RP{
			get{return rpf;}
			set{rpf = value;}
		}
		
		public RecordedData(string name,int sc)
		{
			this.sampleCount = sc;
			this.gestureName = name;
			lPoints = new List<MyMath.Vector2> ();
			rPoints = new List<MyMath.Vector2> ();
			zy_lPoints = new List<MyMath.Vector2> ();
			zy_rPoints = new List<MyMath.Vector2> ();
			zx_lPoints = new List<MyMath.Vector2> ();
			zx_rPoints = new List<MyMath.Vector2> ();
			rpf = new List<PointF> ();
			lpf = new List<PointF> ();
			zy_lpf = new List<PointF> ();
			zy_rpf = new List<PointF> ();
			zx_lpf = new List<PointF> ();
			zx_rpf = new List<PointF> ();
			constrain = new List<Constrain>();

			points_3D_L = new List<MyMath.Vector3>();
			points_3D_R = new List<MyMath.Vector3>();
		}

		public double Match(List<TimePointF> tpfll, List<TimePointF> tpflr, float threshold, float minSize, int plane, int method)
		{
//			if (tpfll.Count < LearningMachine.sampleCount / 2)
//				return -1;

			//			if (!GoldenSectionExtension.IsLargeEnough(tpflr, minSize)|| !GoldenSectionExtension.IsLargeEnough(tpfll, minSize))
//				return -2;

			List<PointF> pfl, pfr; 

			List<PointF> rdl, rdr;

			// 1: xy_plane 2: zy_plane 2: zx_plane
			if (plane == 1) {
				rdr = new List<PointF> (rpf);
				rdl = new List<PointF> (lpf);
			} else if (plane == 2){
				rdr = new List<PointF> (zy_rpf);
				rdl = new List<PointF> (zy_lpf);
			}
			else{
				rdr = new List<PointF> (zx_rpf);
				rdl = new List<PointF> (zx_lpf);
			}
			if(method == 1){
				pfl = GoldenSection.DollarOnePack (tpfll, sampleCount);
				pfr = GoldenSection.DollarOnePack (tpflr, sampleCount); 

				double[] bestl = GoldenSection.GoldenSectionSearch(
					pfl,                             // to rotate
					rdl,                           // to match
					GeotrigEx.Degrees2Radians(-45.0),   // lbound
					GeotrigEx.Degrees2Radians(+45.0),   // ubound
					GeotrigEx.Degrees2Radians(2.0)      // threshold
					);
				
				double scorel = 1.0 - bestl[0] / GoldenSection.HalfDiagonal;

				double[] bestr = GoldenSection.GoldenSectionSearch(
					pfr,                             // to rotate
					rdr,                           // to match
					GeotrigEx.Degrees2Radians(-45.0),   // lbound
					GeotrigEx.Degrees2Radians(+45.0),   // ubound
					GeotrigEx.Degrees2Radians(2.0)      // threshold
					);
				
				double scorer = 1.0 - bestr[0] / GoldenSection.HalfDiagonal;
				return (scorel + scorer) * 0.5f;
			}else if(method == 2){

//				pfl = DynamicTimeWraping.NormalizeDTW(tpfll);
//				pfr = DynamicTimeWraping.NormalizeDTW(tpflr);
//				pfl = DynamicTimeWraping.DTWPack(tpfll, sampleCount);
//				pfr = DynamicTimeWraping.DTWPack(tpflr, sampleCount);
				pfl = GoldenSection.DollarOnePack (tpfll, sampleCount);
				pfr = GoldenSection.DollarOnePack (tpflr, sampleCount); 

				float gesLengthL = (float)DynamicTimeWraping.PathLength(pfl);
				float gesLengthR = (float)DynamicTimeWraping.PathLength(pfr);

				int lengthAL = pfl.Count;
//				int lengthBL = lpf_DTW.Count;
				int lengthBL = rdl.Count;
				
//				float[,] dMatrixL = DynamicTimeWraping.GetDistanceMatrix(pfl,lpf_DTW, lengthAL, lengthBL);
				float[,] dMatrixL = DynamicTimeWraping.GetDistanceMatrix(pfl,rdl, lengthAL, lengthBL);
				
//				UnityEngine.Debug.Log (pfl[0]);
//				UnityEngine.Debug.Log (lpf_DTW[0]);

				float[,] rMatrixL = DynamicTimeWraping.GetDTWMatrix(dMatrixL, lengthAL, lengthBL);
				
				List<UnityEngine.Vector2> pathL = DynamicTimeWraping.GetOptimalPath(rMatrixL, lengthAL, lengthBL);

//				float pathLengthL = DynamicTimeWraping.GetPathLength(dMatrixL, pathL);

				int lengthAR = pfr.Count;
//				int lengthBR = rpf_DTW.Count;
				int lengthBR = rdr.Count;
				
//				float[,] dMatrixR = DynamicTimeWraping.GetDistanceMatrix(pfr,rpf_DTW, lengthAR, lengthBR);
				float[,] dMatrixR = DynamicTimeWraping.GetDistanceMatrix(pfr,rdr, lengthAR, lengthBR);
				
				float[,] rMatrixR = DynamicTimeWraping.GetDTWMatrix(dMatrixR, lengthAR, lengthBR);
				
				List<UnityEngine.Vector2> pathR = DynamicTimeWraping.GetOptimalPath(rMatrixR, lengthAR, lengthBR);

//				float pathLengthR = DynamicTimeWraping.GetPathLength(dMatrixR, pathR);
				UnityEngine.Debug.Log(gestureName);
//				UnityEngine.Debug.Log(dMatrixL[lengthBL - 1, lengthAL - 1]);
//				UnityEngine.Debug.Log(dMatrixR[lengthBR - 1, lengthAR - 1]);
				
				UnityEngine.Debug.Log(rMatrixL[lengthBL - 1, lengthAL - 1]);
				UnityEngine.Debug.Log(rMatrixR[lengthBR - 1, lengthAR - 1]);
				UnityEngine.Debug.Log(gesLengthL);
				UnityEngine.Debug.Log(gesLengthR);
//				return (rMatrixL[lengthBL - 1, lengthAL - 1] + rMatrixR[lengthBR - 1, lengthAR - 1] ) * 0.5f;
//				return (rMatrixL[lengthBL - 1, lengthAL - 1] / cl + rMatrixR[lengthBR - 1, lengthAR - 1] / cr) * 0.5f;
				return (rMatrixL[lengthBL - 1, lengthAL - 1] / (lengthL + gesLengthL) + rMatrixR[lengthBR - 1, lengthAR - 1] / (lengthR + gesLengthR)) * 0.5f;
				
			}else{
				return 0;
			}

		}

		public double OneHandedMatch(List<TimePointF> tpflr, float threshold, float minSize, int plane, int method)
		{
//			if (tpflr.Count < LearningMachine.sampleCount / 2)
//				return -1;
			
			//			if (!GoldenSectionExtension.IsLargeEnough(tpflr, minSize)|| !GoldenSectionExtension.IsLargeEnough(tpfll, minSize))
			//				return -2;

			List<PointF> pfr;
			
			List<PointF> rdr;

			double scorer = 0;

			// 1: xy_plane 2: zy_plane 2: zx_plane
			if (plane == 1) {
				rdr = new List<PointF> (rpf);
			} else if (plane == 2) {
				rdr = new List<PointF> (zy_rpf);
			} else {
				rdr = new List<PointF> (zx_rpf);
			}

			if (method == 1) {

				pfr = GoldenSection.DollarOnePack (tpflr, sampleCount); 
				
				double[] bestr = GoldenSection.GoldenSectionSearch (
				pfr,                             // to rotate
				rdr,                           // to match
				GeotrigEx.Degrees2Radians (-45.0),   // lbound
				GeotrigEx.Degrees2Radians (+45.0),   // ubound
				GeotrigEx.Degrees2Radians (2.0)      // threshold
				);
			
				scorer = 1.0 - bestr [0] / GoldenSection.HalfDiagonal;

			} else {

				pfr = DynamicTimeWraping.DTWPack(tpflr, sampleCount); 
				float gesLengthR = (float)DynamicTimeWraping.PathLength(pfr);
				
				int lengthAR = pfr.Count;
				int lengthBR = rdr.Count;
				
				float[,] dMatrixR = DynamicTimeWraping.GetDistanceMatrix(pfr,rdr, lengthAR, lengthBR);
				
				float[,] rMatrixR = DynamicTimeWraping.GetDTWMatrix(dMatrixR, lengthAR, lengthBR);
				
				List<UnityEngine.Vector2> pathR = DynamicTimeWraping.GetOptimalPath(rMatrixR, lengthAR, lengthBR);
				
				//				float pathLengthR = DynamicTimeWraping.GetPathLength(dMatrixR, pathR);

				Write2File(gestureName, rMatrixR, lengthAR, lengthBR);
				
				scorer = rMatrixR[lengthBR - 1, lengthAR - 1] / (lengthR + gesLengthR);
			}
			
			return scorer;
		}

//		public double GetLength(List<PointF> list){
//			double res = 0;
//			for (int i = 0; i < list.Count - 1; i ++) {
//				res += Math.Sqrt(Math.Pow((list[i].X - list[i+1].X),2) + Math.Pow((list[i].Y - list[i+1].Y),2));
//			}
//			return res;
//		}

		private void Write2File(string name, float[,] rMat, int lengthA, int lengthB){

			string filePath = "Assets/MyGame/Temp/DTWRes.txt";
			List<string> sList = new List<string> (); 
			for (int i = 0; i < lengthB; i ++) {
				string line = "";
				for(int j = 0; j < lengthA; j ++){
					line += rMat[i,j];
					line += " ";
				}
				sList.Add(line);
			}
			try{
				using (System.IO.StreamWriter file =
				       new System.IO.StreamWriter(filePath)){
					file.WriteLine("======================");
					file.WriteLine(name);
					foreach(var s in sList){
						file.WriteLine(s);
					}
				}
			}catch(Exception e){
				UnityEngine.Debug.Log(e);
			}
		}
		
	}
}