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

namespace TemplateGesture{
	public class LearningMachine
	{	
		public readonly static int sampleCount = 100;
		private static LearningMachine instance = new LearningMachine ();

		private static NuiSkeletonFrame[] skeletonFrame;

		private static List<RecordedData> pos;

		private static string folderPath = "Assets/MyGame/Recordings/";
		private static ResultList rl = new ResultList();

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

			foreach (string file in Directory.GetFiles(folderPath, "*.data"))
				LoadGestureNew (file);
		}



		public static ResultList Match(List<TimePointF> tpll, List<TimePointF> tplr, 
		                               List<TimePointF> zy_tpll, List<TimePointF> zy_tplr, 
		                               List<TimePointF> zx_tpll, List<TimePointF> zx_tplr, 
		                               Constrain[]c, float threshold,float minSize)
		{
			int i = 0;

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


				double score = 0;
				double zy_score = 0;
				double zx_score = 0;


				if(p.constrain.Count > 0 && !GestureConstrains.MeetConstrains(c, p.constrain)){
					score = -4;
//					UnityEngine.Debug.Log(p.gestureName);
				}

				else{
					score = p.Match(tpll, tplr, threshold, minSize, 1);
					zy_score = p.Match(zy_tpll, zy_tplr, threshold, minSize, 2);
					zx_score = p.Match(zx_tpll, zx_tplr, threshold, minSize, 3);
				}

//				if(zy_score > 0.8 && zx_score > 0.8)
//					UnityEngine.Debug.Log(zy_score);
//					UnityEngine.Debug.Log(zx_score);

				rl.UpdateResult(i++, p.gestureName, (float)score);

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
				int numPts = XmlConvert.ToInt32(reader.GetAttribute("NumPts"));
				string gesName = reader.GetAttribute("GesName");

				RecordedData rd = new RecordedData(gesName, sampleCount);

				int num = numPts / 2;

				List<TimePointF> pr = new List<TimePointF>(num);
				List<TimePointF> pl = new List<TimePointF>(num);

				List<TimePointF> zy_pr = new List<TimePointF>(num);
				List<TimePointF> zy_pl = new List<TimePointF>(num);

				List<TimePointF> zx_pr = new List<TimePointF>(num);
				List<TimePointF> zx_pl = new List<TimePointF>(num);


				while(reader.Read()){
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
						zx_p.X = XmlConvert.ToSingle(reader.GetAttribute("Z"));
						zx_p.Y = XmlConvert.ToSingle(reader.GetAttribute("Y"));
						zx_pl.Add(zx_p);

					}else if(reader.LocalName == "RightHandPoints"){

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
						zx_p.X = XmlConvert.ToSingle(reader.GetAttribute("Z"));
						zx_p.Y = XmlConvert.ToSingle(reader.GetAttribute("Y"));
						zx_pr.Add(zx_p);

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
	}
}

