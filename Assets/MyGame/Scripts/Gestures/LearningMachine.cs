using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using MyMath;
using System.Runtime.Serialization;
using Kinect;
using System.Xml;
using System.Diagnostics;
using UnityEngine;
using System;
using WobbrockLib;
using WobbrockLib.Extensions;
using System.Drawing;

namespace TemplateGesture{
	public class LearningMachine
	{	
		public readonly static int sampleCount = 100;
		private static LearningMachine instance = new LearningMachine ();

		private static NuiSkeletonFrame[] skeletonFrame;

		private static List<RecordedData> pos;

		private static string folderPath = "Assets/MyGame/Recordings/";
		private static ResultList rl = new ResultList();

		private static readonly double lineMin = 0.99;
		private static double radiance45;
		private static readonly float minScore = 0.8f;

		public static float MinScore{
			get{
				return minScore;
			}
		}

		public static LearningMachine Instance{
			get{
				return instance;
			}
		}

		public static ResultList ResultList{
			get{
				return rl;
			}
		}

		public static List<RecordedData> Pos{
			get{
				return pos;
			}
		}

		public static void Initialize(){
			pos = new List<RecordedData> ();
			radiance45 = GoldenSectionExtension.DegreeToRadian(45);

			foreach (string file in Directory.GetFiles(folderPath, "*.data"))
				LoadGestureNew (file);
		}



		public static ResultList Match(List<TimePointF> tpll, List<TimePointF> tplr, 
		                               List<TimePointF> zy_tpll, List<TimePointF> zy_tplr, 
		                               List<TimePointF> zx_tpll, List<TimePointF> zx_tplr, 
		                               Constrain[]c, float threshold,float minSize)
		{
			int i = 0;

			bool[] lList = new bool[3];
			bool[] rList = new bool[3];


			lList [0] = IsLine (tpll);
			lList [1] = IsLine (zy_tpll);
			lList [2] = IsLine (zx_tpll);
			rList [0] = IsLine (tplr);
			rList [1] = IsLine (zy_tpll);
			rList [2] = IsLine (zx_tpll);

//			bool l = false;
//			bool r = false;
//			List<PointF> listPFL = GoldenSectionExtension.ListTimePointF2ListPointF(tpll);
//			List<PointF> listPFR = GoldenSectionExtension.ListTimePointF2ListPointF(tplr);
//			
//			double radiance45 = GoldenSectionExtension.DegreeToRadian(45);
//			double radiansL = radiance45 - Math.Atan2(listPFL[0].Y - listPFL[listPFL.Count - 1].Y, listPFL[0].X - listPFL[listPFL.Count - 1].X);
//			double radiansR = radiance45 - Math.Atan2(listPFR[0].Y - listPFR[listPFR.Count - 1].Y, listPFR[0].X - listPFR[listPFR.Count - 1].X);
//			
//			listPFL = GeotrigEx.RotatePoints(listPFL, -radiansL);
//			listPFR = GeotrigEx.RotatePoints(listPFR, -radiansR);
//			
//			double correlationL = GetCorrelation(listPFL);
//			double correlationR = GetCorrelation(listPFR);
//
//			if(Math.Abs(correlationL) > lineMin){
//				l = true;
//			}
//			if(Math.Abs(correlationR) > lineMin){
//				r = true;
//			}

			foreach (RecordedData p in pos) {

//				UnityEngine.Debug.Log(p.gestureName);
//				foreach(var v in p.constrain){
//					UnityEngine.Debug.Log(v);
//				}
//				UnityEngine.Debug.Log("==========");
//
//				UnityEngine.Debug.Log("c");
//				foreach(var v in c){
//					UnityEngine.Debug.Log(v);
//				}
//				UnityEngine.Debug.Log("==========");
//				if((p.IsLineL == l) && (p.IsLineR == r)){
//
//				}
				double score = 0;
				double xy_score = 0;
				double zy_score = 0;
				double zx_score = 0;
				double rL = radiance45 - Math.Atan2(tpll[0].Y - tpll[tpll.Count - 1].Y, tpll[0].X - tpll[tpll.Count - 1].X);
				double rR = radiance45 - Math.Atan2(tplr[0].Y - tplr[tplr.Count - 1].Y, tplr[0].X - tplr[tplr.Count - 1].X);
				int planeCount = 1;
				if(p.OnaHanded){
					score = p.OneHandedMatch(tplr, threshold, minSize, 1);
				}
				else{
					if( p.constrain.Count > 0 && !GestureConstrains.MeetConstrains(c, p.constrain)){
						score = -4;
					}
					else{
	//					if(l[(int)p.Plane] & r[(int)p.Plane]){
						if(lList[0] & rList[0]){
							if(p.IsLineL && p.IsLineR){
								rL = Math.Abs(GoldenSectionExtension.RadiansToDegree(rL) - p.AngleL);
								rR = Math.Abs(GoldenSectionExtension.RadiansToDegree(rR) - p.AngleR);
								score = 2;
	//							UnityEngine.Debug.Log(p.gestureName);
	//							UnityEngine.Debug.Log(rL + rR);
							}
							else{
								score = -5;
							}
						}
						else{
							xy_score = p.Match(tpll, tplr, threshold, minSize, 1);
							zy_score = p.Match(zy_tpll, zy_tplr, threshold, minSize, 2);
							zx_score = p.Match(zx_tpll, zx_tplr, threshold, minSize, 3);
						}
					}

//					if(p.gestureName == "hdel8"){
//						UnityEngine.Debug.Log(p.gestureName);
//	//					UnityEngine.Debug.Log(xy_score);
//						UnityEngine.Debug.Log(zx_score);
//	//					UnityEngine.Debug.Log(zy_score);
//					}

					if(score != 2){
						if((int)p.Plane == 0){
							score = xy_score;
						}
						if((int)p.Plane == 1){
							score = zx_score;
						}
						if((int)p.Plane == 2){
							score = zy_score;
						}
					}
	//				bool xy = false;
	//				bool xz = false;
	//				bool yz = false;
	//				if(xy_score >= minScore){
	//					planeCount ++;
	//					score += xy_score;
	//					xy = true;
	//				}
	//
	//				if(zy_score >= minScore){
	//					planeCount ++;
	//					score += zy_score;
	//					score /= 2;
	//					yz = true;
	//				}
	//
	//				if(zx_score >= minScore){
	//					planeCount ++;
	//					score += zx_score;
	//					score /= 2;
	//					xz = true;
	//				}
					//for testing
	//				if(p.gestureName == "HD Pool Entry Dive"){
	//					UnityEngine.Debug.Log(p.gestureName);
	//					UnityEngine.Debug.Log(xy_score);
	//					UnityEngine.Debug.Log(zx_score);
	//					UnityEngine.Debug.Log(zy_score);
	//				}

	//				if(zy_score > 0.8 && zx_score > 0.8)
	//					UnityEngine.Debug.Log(zy_score);
	//					UnityEngine.Debug.Log(zx_score);
	//				if(score < zy_score)
	//					score = zy_score;
	//				if(score < zx_score)
	//					score = zx_score;
				}
				if(p.gestureName == "s"){
					UnityEngine.Debug.Log(p.gestureName);
					UnityEngine.Debug.Log(score);
				}
								
				rl.UpdateResult(i++, p.gestureName, (float)score, rL + rR, planeCount);
			}
			return rl;
		}

		public static bool LoadGestureNew(string filePath)
		{
			bool success = true;
			XmlTextReader reader = null;
			try
			{
				reader = new XmlTextReader(filePath);
				reader.WhitespaceHandling = WhitespaceHandling.None;
				reader.MoveToContent();
				
				System.Diagnostics.Debug.Assert(reader.LocalName == "Gesture");
//				int numPts = XmlConvert.ToInt32(reader.GetAttribute("NumPts"));
				string gesName = reader.GetAttribute("GesName");

				RecordedData rd = new RecordedData(gesName, sampleCount);

//				int num = numPts / 2;

				List<TimePointF> pr = new List<TimePointF>();
				List<TimePointF> pl = new List<TimePointF>();

				List<TimePointF> zy_pr = new List<TimePointF>();
				List<TimePointF> zy_pl = new List<TimePointF>();

				List<TimePointF> zx_pr = new List<TimePointF>();
				List<TimePointF> zx_pl = new List<TimePointF>();

				rd.Plane = 0;
				rd.OnaHanded = false;
				while(reader.Read()){
					if(reader.LocalName == "OneHanded"){
						rd.OnaHanded = XmlConvert.ToBoolean(reader.GetAttribute("Value"));
					}
					if(reader.LocalName == "Plane"){
						rd.Plane = (Plane)XmlConvert.ToSingle(reader.GetAttribute("Value"));
					}
					if(reader.LocalName == "Constrains"){
						rd.constrain.Add((Constrain)XmlConvert.ToSingle(reader.GetAttribute("Value")));
					}
					if(reader.LocalName == "LeftHandPoints"){

						MyMath.Vector2 rp = MyMath.Vector2.Zero;
						rp.x = -XmlConvert.ToSingle(reader.GetAttribute("X"));
						rp.y = XmlConvert.ToSingle(reader.GetAttribute("Y"));
						rd.LPoints.Add(rp);

						TimePointF p = TimePointF.Empty;
						p.X = XmlConvert.ToSingle(reader.GetAttribute("X"));
						p.Y = XmlConvert.ToSingle(reader.GetAttribute("Y"));
						pl.Add(p);

						TimePointF zy_p = TimePointF.Empty;
						zy_p.X = XmlConvert.ToSingle(reader.GetAttribute("Z"));
						zy_p.Y = XmlConvert.ToSingle(reader.GetAttribute("Y"));
						zy_pl.Add(zy_p);

						TimePointF zx_p = TimePointF.Empty;
						zx_p.X = XmlConvert.ToSingle(reader.GetAttribute("X"));
						zx_p.Y = XmlConvert.ToSingle(reader.GetAttribute("Z"));
						rd.ZX_LPoints.Add(new MyMath.Vector2(zx_p.X, zx_p.Y));
						zx_pl.Add(zx_p);

					}
					if(reader.LocalName == "RightHandPoints"){

						MyMath.Vector2 rp = MyMath.Vector2.Zero;
						rp.x = -XmlConvert.ToSingle(reader.GetAttribute("X"));
						rp.y = XmlConvert.ToSingle(reader.GetAttribute("Y"));
						rd.RPoints.Add(rp);

						TimePointF p = TimePointF.Empty;
						p.X = XmlConvert.ToSingle(reader.GetAttribute("X"));
						p.Y = XmlConvert.ToSingle(reader.GetAttribute("Y"));
						pr.Add(p);

						TimePointF zy_p = TimePointF.Empty;
						zy_p.X = XmlConvert.ToSingle(reader.GetAttribute("Z"));
						zy_p.Y = XmlConvert.ToSingle(reader.GetAttribute("Y"));
						zy_pr.Add(zy_p);

						TimePointF zx_p = TimePointF.Empty;
						zx_p.X = XmlConvert.ToSingle(reader.GetAttribute("X"));
						zx_p.Y = XmlConvert.ToSingle(reader.GetAttribute("Z"));
						zx_pr.Add(zx_p);
						rd.ZX_RPoints.Add(new MyMath.Vector2(zx_p.X, zx_p.Y));
						

					}
				}

				rd.LP = GoldenSection.DollarOnePack(pl, LearningMachine.sampleCount);
				rd.RP = GoldenSection.DollarOnePack(pr, LearningMachine.sampleCount);

				rd.ZY_LP = GoldenSection.DollarOnePack(zy_pl, LearningMachine.sampleCount);
				rd.ZY_RP = GoldenSection.DollarOnePack(zy_pr, LearningMachine.sampleCount);

				rd.ZX_LP = GoldenSection.DollarOnePack(zx_pl, LearningMachine.sampleCount);
				rd.ZX_RP = GoldenSection.DollarOnePack(zx_pr, LearningMachine.sampleCount);

//				//calculate the angles and variances
//				List<double> angleR = new List<double> ();
//				List<double> angleL = new List<double> ();
//				
//				double sumL = 0;
//				double sumR = 0;
//				for (int i = 1; i < pr.Count; i ++) {
//
//					double l = Math.Atan2(pr[0].Y - pr[i].Y, pr[0].X - pr[i].X);
//					double r = Math.Atan2(pl[0].Y - pl[i].Y, pl[0].X - pl[i].X);
//					l = RadianToDegree(l);
//					r = RadianToDegree(r);
//
//					angleR.Add(r);
//					angleL.Add(l);
//					sumL += l;
//					sumR += r;
//				}
//				double avgR = sumR / (pr.Count - 1);
//				double avgL = sumL / (pl.Count - 1);
//				
//				double varianceSumR = 0;
//				double varianceSumL = 0;
//				for (int i = 0; i < angleR.Count; i++)
//				{
//					varianceSumR += (angleR[i] - avgR) * (angleR[i] - avgR);
//					varianceSumL += (angleL[i] - avgL) * (angleL[i] - avgL);
//				}
//				
//				double varianceR = varianceSumR / angleR.Count;
//				double varianceL = varianceSumL / angleL.Count;
			
				//calculate the correlation
				List<PointF> listPFL = GoldenSectionExtension.ListTimePointF2ListPointF(pl);
				List<PointF> listPFR = GoldenSectionExtension.ListTimePointF2ListPointF(pr);

				List<PointF> listPFL_ZX = GoldenSectionExtension.ListTimePointF2ListPointF(zy_pl);
				List<PointF> listPFR_ZX = GoldenSectionExtension.ListTimePointF2ListPointF(zy_pr);
				List<PointF> listPFL_ZY = GoldenSectionExtension.ListTimePointF2ListPointF(zx_pl);
				List<PointF> listPFR_ZY = GoldenSectionExtension.ListTimePointF2ListPointF(zx_pr);

				double radiansL = radiance45 - Math.Atan2(listPFL[0].Y - listPFL[listPFL.Count - 1].Y, listPFL[0].X - listPFL[listPFL.Count - 1].X);
				double radiansR = radiance45 - Math.Atan2(listPFR[0].Y - listPFR[listPFR.Count - 1].Y, listPFR[0].X - listPFR[listPFR.Count - 1].X);
				double radiansL_ZX = radiance45 - Math.Atan2(listPFL[0].Y - listPFL[listPFL.Count - 1].Y, listPFL[0].X - listPFL[listPFL.Count - 1].X);
				double radiansR_ZX = radiance45 - Math.Atan2(listPFR[0].Y - listPFR[listPFR.Count - 1].Y, listPFR[0].X - listPFR[listPFR.Count - 1].X);
				double radiansL_ZY = radiance45 - Math.Atan2(listPFL[0].Y - listPFL[listPFL.Count - 1].Y, listPFL[0].X - listPFL[listPFL.Count - 1].X);
				double radiansR_ZY = radiance45 - Math.Atan2(listPFR[0].Y - listPFR[listPFR.Count - 1].Y, listPFR[0].X - listPFR[listPFR.Count - 1].X);

				listPFL = GeotrigEx.RotatePoints(listPFL, radiansL);
				listPFR = GeotrigEx.RotatePoints(listPFR, radiansR);
				listPFL_ZX = GeotrigEx.RotatePoints(listPFL_ZX, radiansL_ZX);
				listPFR_ZX = GeotrigEx.RotatePoints(listPFR_ZX, radiansR_ZX);
				listPFL_ZY = GeotrigEx.RotatePoints(listPFL_ZY, radiansL_ZY);
				listPFR_ZY = GeotrigEx.RotatePoints(listPFR_ZY, radiansR_ZY);

				double correlationL = GetCorrelation(listPFL);
				double correlationR = GetCorrelation(listPFR);
				double correlationL_ZX = GetCorrelation(listPFL_ZX);
				double correlationR_ZX = GetCorrelation(listPFR_ZX);
				double correlationL_ZY = GetCorrelation(listPFL_ZY);
				double correlationR_ZY = GetCorrelation(listPFR_ZY);

				if(Math.Abs(correlationL) > lineMin){
					rd.IsLineL = true;
					rd.AngleL = GoldenSectionExtension.RadiansToDegree(radiansL);
				}else{
					rd.IsLineL = false;
				}

				if(Math.Abs(correlationR) > lineMin){
					rd.IsLineR = true;
					rd.AngleR = GoldenSectionExtension.RadiansToDegree(radiansR);
				}else{
					rd.IsLineR = false;
				}

				if(Math.Abs(correlationL_ZX) > lineMin){
					rd.IsLineL_XZ = true;
					rd.AngleL_XZ = GoldenSectionExtension.RadiansToDegree(radiansL);
				}else{
					rd.IsLineL_XZ = false;
				}
				
				if(Math.Abs(correlationR_ZX) > lineMin){
					rd.IsLineR_XZ = true;
					rd.AngleR_XZ = GoldenSectionExtension.RadiansToDegree(radiansR);
				}else{
					rd.IsLineR_XZ = false;
				}

				if(Math.Abs(correlationL_ZY) > lineMin){
					rd.IsLineL_YZ = true;
					rd.AngleL_YZ = GoldenSectionExtension.RadiansToDegree(radiansL);
				}else{
					rd.IsLineL_YZ = false;
				}
				
				if(Math.Abs(correlationR_ZY) > lineMin){
					rd.IsLineR_YZ = true;
					rd.AngleR_YZ = GoldenSectionExtension.RadiansToDegree(radiansR);
				}else{
					rd.IsLineR_YZ = false;
				}
//				UnityEngine.Debug.Log(GoldenSectionExtension.RadiansToDegree(radiansL));
//				UnityEngine.Debug.Log(GoldenSectionExtension.RadiansToDegree(radiansR));
				pos.Add(rd);
				
				rl.AddResult(gesName, 2);

			}
			catch (XmlException xex)
			{
				UnityEngine.Debug.Log(xex.Message);
				success = false;
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.Log(ex.Message);
				success = false;
			}
			finally
			{
				if (reader != null)
					reader.Close();
			}
			return success;
		}

		private static double RadianToDegree(double angle)
		{
			return angle * (180.0 / Math.PI);
		}

		public static double GetCorrelation(List<PointF> list){
			//calculate the correlation
			float sumAB = 0;
			float sumA = 0;
			float sumB = 0;
			float sumAA = 0;
			float sumBB = 0;
			
			foreach (var p in list){
				sumAB += p.X * p.Y;
				sumA += p.X;
				sumB += p.Y;
				sumAA += p.X * p.X;
				sumBB += p.Y * p.Y;
			}
			
			double res = ((list.Count * sumAB) - (sumA * sumB))
				/ (Math.Sqrt((list.Count * sumAA) - (sumA * sumA))
				   * Math.Sqrt((list.Count * sumBB) - (sumB * sumB)));

			return res;
		}

		private static bool IsLine(List<TimePointF> list){

			bool res = false;
			List<PointF> listP = GoldenSectionExtension.ListTimePointF2ListPointF(list);
			
			double radiance45 = GoldenSectionExtension.DegreeToRadian(45);
			double radians = radiance45 - Math.Atan2(listP[0].Y - listP[listP.Count - 1].Y, listP[0].X - listP[listP.Count - 1].X);
			
			listP = GeotrigEx.RotatePoints(listP, -radians);
			
			double correlation = GetCorrelation(listP);
			
			if(Math.Abs(correlation) > lineMin){
				res = true;
			}

			return res;
		}
	}
}

