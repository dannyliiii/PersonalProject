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
		private static List<RecordedData> rawPos;

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
		public static List<RecordedData> RawPos{
			get{
				return rawPos;
			}
		}
		public static List<RecordedData> Pos{
			get{
				return pos;
			}
		}

		public static void Initialize(){
			pos = new List<RecordedData> ();
			rawPos = new List<RecordedData> ();

			foreach (string file in Directory.GetFiles(folderPath, "*.data"))
				LoadGestureNew (file);
		}



		public static ResultList Match(List<TimePointF> tpll, List<TimePointF> tplr, List<MyMath.Vector2> entriesL, List<MyMath.Vector2> entriesR, float threshold,float minSize)
		{
			int i = 0;

			foreach (RecordedData p in pos) {

				double score = p.Match(tpll, tplr, entriesL, entriesR, threshold, minSize);
				rl.UpdateResult(i++, p.gestureName, (float)score);

			}
			return rl;
		}
		
		public static bool LoadRawData(string filePath)
		{
			bool success = true;
			XmlTextReader reader = null;
			try
			{
				reader = new XmlTextReader(filePath);
				reader.WhitespaceHandling = WhitespaceHandling.None;
				reader.MoveToContent();
				
				System.Diagnostics.Debug.Assert(reader.LocalName == "RawData");
				//string name = reader.GetAttribute("Name");
				
				int numPts = XmlConvert.ToInt32(reader.GetAttribute("NumPts"));
				string gesName = reader.GetAttribute("GesName");
				RecordedData rd = new RecordedData(gesName, sampleCount);

				while(reader.Read()){
					if(reader.LocalName == "LeftHandPoints"){
						MyMath.Vector2 p = MyMath.Vector2.Zero;
						p.x = XmlConvert.ToSingle(reader.GetAttribute("X"));
						p.y = XmlConvert.ToSingle(reader.GetAttribute("Y"));
						rd.LPoints.Add(p);
					}else if(reader.LocalName == "RightHandPoints"){
						MyMath.Vector2 p = MyMath.Vector2.Zero;
						p.x = XmlConvert.ToSingle(reader.GetAttribute("X"));
						p.y = XmlConvert.ToSingle(reader.GetAttribute("Y"));
						rd.RPoints.Add(p);
					}
				}
				rawPos.Add(rd);

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
				
				List<TimePointF> pr = new List<TimePointF>();
				List<TimePointF> pl = new List<TimePointF>();

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


					}else if(reader.LocalName == "RightHandPoints"){

						MyMath.Vector2 rp = MyMath.Vector2.Zero;
						rp.x = -XmlConvert.ToSingle(reader.GetAttribute("X"));
						rp.y = XmlConvert.ToSingle(reader.GetAttribute("Y"));
						rd.RPoints.Add(rp);

						TimePointF p = TimePointF.Empty;
						p.X = XmlConvert.ToSingle(reader.GetAttribute("X"));
						p.Y = XmlConvert.ToSingle(reader.GetAttribute("Y"));
						pr.Add(p);
					}
				}
		
				rd.LP = GoldenSection.DollarOnePack(pl, LearningMachine.sampleCount);
				rd.RP = GoldenSection.DollarOnePack(pr, LearningMachine.sampleCount);

				pos.Add(rd);
				rawPos.Add(rd);

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

