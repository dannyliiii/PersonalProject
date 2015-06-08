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



		public static ResultList Match(List<TimePointF> tpll, List<TimePointF> tplr, List<TimePointF> zy_tpll, List<TimePointF> zy_tplr, float threshold,float minSize)
		{
			int i = 0;

			foreach (RecordedData p in pos) {

				double score = p.Match(tpll, tplr, threshold, minSize, 1);
				double zy_score = p.Match(zy_tpll, zy_tplr, threshold, minSize, 2);

//				if(zy_score > 0.8)
//					UnityEngine.Debug.Log(zy_score);

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

				while(reader.Read()){
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

					}
				}
		
				rd.LP = GoldenSection.DollarOnePack(pl, LearningMachine.sampleCount);
				rd.RP = GoldenSection.DollarOnePack(pr, LearningMachine.sampleCount);

				rd.ZY_LP = GoldenSection.DollarOnePack(zy_pl, LearningMachine.sampleCount);
				rd.ZY_RP = GoldenSection.DollarOnePack(zy_pr, LearningMachine.sampleCount);

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

	}
}

