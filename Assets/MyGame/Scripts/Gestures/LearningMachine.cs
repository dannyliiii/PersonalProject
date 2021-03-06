﻿using System.Collections.Generic;
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

		private static readonly double lineMin = 0.9;
		private static double radiance45;
		private static readonly float minScore = 0.8f;
		private static readonly float minScoreOneHanded = 0.85f;
		private static readonly float minScoreDTW = 0.85f;
		
		public static float MinScoreDTW{
			get{
				return minScoreDTW;
			}
		}
		public static float MinScore{
			get{
				return minScore;
			}
		}
		public static float MinScoreOneHanded{
			get{
				return minScoreOneHanded;
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
		                               List<MyMath.Vector3> l_3d, List<MyMath.Vector3> r_3d,
		                               Constrain[]c, float threshold,float minSize, bool oneHanded, int method)
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
			UnityEngine.Debug.Log("=================");
			foreach (RecordedData p in pos) {
		
				double score = 0;
				double xy_score = 0;
				double zy_score = 0;
				double zx_score = 0;
				double rL = radiance45 - Math.Atan2(tpll[0].Y - tpll[tpll.Count - 1].Y, tpll[0].X - tpll[tpll.Count - 1].X);
				double rR = radiance45 - Math.Atan2(tplr[0].Y - tplr[tplr.Count - 1].Y, tplr[0].X - tplr[tplr.Count - 1].X);
				int planeCount = 1;
				//checking constraints
				if( p.constrain.Count > 0 && !GestureConstrains.MeetConstrains(c, p.constrain)){
					score = -4;
					UnityEngine.Debug.Log(p.gestureName.ToString() + " " + score.ToString());
					rl.UpdateResult(i++, p.gestureName, (float)score);
					continue;
				}
				// processing according to the method
				if(method == 2){
					if(oneHanded){
						if(p.OnaHanded)
							score = p.OneHandedMatch(tplr, threshold, minSize, 1, method);
						else{
							score = -6;
						}
					}
					else{
						if(p.OnaHanded){
							score  = -5;
						}else{
							score = p.Match(zx_tpll, zx_tplr, threshold, minSize, 1, method);
						}
					}
					UnityEngine.Debug.Log(p.gestureName.ToString() + " " + score.ToString());
					rl.UpdateResult(i++, p.gestureName, (float)score);
				}else if(method == 3){
					if(oneHanded && !p.OnaHanded){
						score = -6;
					}else if(!oneHanded && p.OnaHanded){
						score  = -5;
					}else{
						score = p.Match3D(l_3d, r_3d, oneHanded);
					}
					UnityEngine.Debug.Log(p.gestureName.ToString() + " " + score.ToString());
					rl.UpdateResult(i++, p.gestureName, (float)score);
				}
				else{
					if(oneHanded && p.OnaHanded){
						score = p.OneHandedMatch(tplr, threshold, minSize, 1, method);
					}
					else{

						if( p.OnaHanded){
							score  = -5;
						}
						else{
							//checking lines
							if(p.IsLineL && p.IsLineR){
//								UnityEngine.Debug.Log(p.gestureName);
//								
//								UnityEngine.Debug.Log(p.IsLineL);
//								UnityEngine.Debug.Log(p.IsLineR);
								if(lList[0] & rList[0]){
									rL = Math.Abs(GoldenSectionExtension.RadiansToDegree(rL) - p.AngleL);
									rR = Math.Abs(GoldenSectionExtension.RadiansToDegree(rR) - p.AngleR);
									score = 2;
								}
								else{
									score = -7;
								}
								UnityEngine.Debug.Log(p.gestureName.ToString() + " " + score.ToString());
								rl.UpdateResult(i++, p.gestureName, (float)score, rL + rR, planeCount);
								continue;
							}else{
								xy_score = p.Match(tpll, tplr, threshold, minSize, 1, method);
								zy_score = p.Match(zy_tpll, zy_tplr, threshold, minSize, 2, method);
								zx_score = p.Match(zx_tpll, zx_tplr, threshold, minSize, 3, method);
							}
						}

						if(score != 2 && score != -5 && score != -7){
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
						UnityEngine.Debug.Log(p.gestureName.ToString() + " " + score.ToString());
						rl.UpdateResult(i++, p.gestureName, (float)score);	
					}
//					if(p.gestureName == "s"){
//						UnityEngine.Debug.Log(p.gestureName);
//						UnityEngine.Debug.Log(score);
//					}
				}
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

				List<MyMath.Vector3> tempL_3D = new List<MyMath.Vector3>();
				List<MyMath.Vector3> tempR_3D = new List<MyMath.Vector3>();

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

						MyMath.Vector3 p3l = MyMath.Vector3.Zero;
						p3l.x = XmlConvert.ToSingle(reader.GetAttribute("X"));
						p3l.y = XmlConvert.ToSingle(reader.GetAttribute("Y"));
						p3l.z = XmlConvert.ToSingle(reader.GetAttribute("Z"));
						tempL_3D.Add(p3l);

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
						
						MyMath.Vector3 p3r = MyMath.Vector3.Zero;
						p3r.x = XmlConvert.ToSingle(reader.GetAttribute("X"));
						p3r.y = XmlConvert.ToSingle(reader.GetAttribute("Y"));
						p3r.z = XmlConvert.ToSingle(reader.GetAttribute("Z"));
						tempR_3D.Add(p3r);
					}
				}

				rd.Points_3D_L = DynamicTimeWraping.DTWPack3D(tempL_3D, LearningMachine.sampleCount);
				rd.Points_3D_R = DynamicTimeWraping.DTWPack3D(tempR_3D, LearningMachine.sampleCount);

				rd.LengthL_3D = DynamicTimeWraping.PathLength3D(rd.Points_3D_L);
				rd.LengthR_3D = DynamicTimeWraping.PathLength3D(rd.Points_3D_R);

				MyMath.Vector3 sizeL = DynamicTimeWraping.GetSize3D(rd.Points_3D_L);
				MyMath.Vector3 sizeR = DynamicTimeWraping.GetSize3D(rd.Points_3D_R);

//				UnityEngine.Debug.Log(rd.gestureName);	
//				UnityEngine.Debug.Log(rd.LengthL_3D);
//				UnityEngine.Debug.Log(rd.LengthR_3D);


				rd.LP_DTW = DynamicTimeWraping.DTWPack(pl, LearningMachine.sampleCount);
				rd.RP_DTW = DynamicTimeWraping.DTWPack(pr, LearningMachine.sampleCount);
//				rd.LP_DTW = DynamicTimeWraping.NormalizeDTW(pl);
//				rd.RP_DTW = DynamicTimeWraping.NormalizeDTW(pr);

				rd.LP = GoldenSection.DollarOnePack(pl, LearningMachine.sampleCount);
				rd.RP = GoldenSection.DollarOnePack(pr, LearningMachine.sampleCount);

				rd.ZY_LP = GoldenSection.DollarOnePack(zy_pl, LearningMachine.sampleCount);
				rd.ZY_RP = GoldenSection.DollarOnePack(zy_pr, LearningMachine.sampleCount);

				rd.ZX_LP = GoldenSection.DollarOnePack(zx_pl, LearningMachine.sampleCount);
				rd.ZX_RP = GoldenSection.DollarOnePack(zx_pr, LearningMachine.sampleCount);
			
				rd.LengthL = DynamicTimeWraping.PathLength(rd.LP_DTW);
				rd.LengthR = DynamicTimeWraping.PathLength(rd.RP_DTW);
//				UnityEngine.Debug.Log(rd.gestureName);	
//				UnityEngine.Debug.Log(rd.LengthL);
//				UnityEngine.Debug.Log(rd.LengthR);

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
//				UnityEngine.Debug.Log(gesName);
//				UnityEngine.Debug.Log(correlationL);
//				UnityEngine.Debug.Log(correlationR);

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
//			UnityEngine.Debug.Log(correlation);

			if(Math.Abs(correlation) > lineMin){
				res = true;
			}

			return res;
		}
	}
}

