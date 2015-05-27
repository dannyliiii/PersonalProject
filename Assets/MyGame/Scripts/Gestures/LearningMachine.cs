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

namespace TemplateGesture{
	public class LearningMachine
	{
		private static LearningMachine instance = new LearningMachine ();

		private static NuiSkeletonFrame[] skeletonFrame;

		private static List<RecordedPath> paths;
		private static List<RecordedPath> rawData;

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
		public static List<RecordedPath> RawData{
			get{
				return rawData;
			}
		}
		public static List<RecordedPath> Paths{
			get{
				return paths;
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
			paths = new List<RecordedPath>();
			rawData = new List<RecordedPath>();

			pos = new List<RecordedData> ();
			rawPos = new List<RecordedData> ();

			foreach (string file in Directory.GetFiles(folderPath, "*.xml"))
			{
				LoadGesture(file);
			}
			foreach (string file in Directory.GetFiles(folderPath, "*.raw"))
				LoadRawData (file);
		}
//		public LearningMachine()
//		{
//			paths = new List<RecordedPath>();
//			rawData = new List<RecordedPath>();
//
//			foreach (string file in Directory.GetFiles(folderPath, "*.xml"))
//			{
//				LoadGesture(file);
//			}
//			foreach (string file in Directory.GetFiles(folderPath, "*.raw"))
//				LoadRawData (file);
//		}

		public static ResultList Match(List<MyMath.Vector2> entries, float threshold, float minimalScore, float minSize)
		{
			foreach (RecordedPath p in paths) {
				float score = p.Match(entries, threshold, minimalScore, minSize);
				//print score of z for testing
				if(p.gestureName == "a" && score > 0){
					//UnityEngine.Debug.Log(p.gestureName);
					//UnityEngine.Debug.Log(score);
				}

				if(score >= 0)
					rl.UpdateResult(p.gestureName, score);
//					rl.AddResult(p.gestureName, score);
			}
			//return Paths.Any(path => path.Match(entries, threshold, minimalScore, minSize));
			return rl;
		}
		
		public static void AddPath(RecordedPath path)
		{
			path.CloseAndPrepare();
			Paths.Add(path);
		}

		public static void AddRawData(RecordedPath rawData)
		{
			rawData.CloseAndPrepare();
			Paths.Add(rawData);
		}

		public static bool LoadGesture(string filePath)
		{
			bool success = true;
			XmlTextReader reader = null;
			try
			{
				reader = new XmlTextReader(filePath);
				reader.WhitespaceHandling = WhitespaceHandling.None;
				reader.MoveToContent();

				System.Diagnostics.Debug.Assert(reader.LocalName == "ProcessedData");
//				string name = reader.GetAttribute("Name");

				int numPts = XmlConvert.ToInt32(reader.GetAttribute("NumPts"));
				string gesName = reader.GetAttribute("GesName");
				RecordedPath rp = new RecordedPath(numPts, gesName);

				RecordedData rd = new RecordedData(gesName);
	
				while(reader.Read()){
					if(reader.LocalName == "LeftHandPoints"){
						MyMath.Vector2 p = MyMath.Vector2.Zero;
						p.x = XmlConvert.ToSingle(reader.GetAttribute("X"));
						p.y = XmlConvert.ToSingle(reader.GetAttribute("Y"));
						rp.Points.Add(p);
						rd.LPoints.Add(p);
					}else if(reader.LocalName == "RightHandPoints"){
						MyMath.Vector2 p = MyMath.Vector2.Zero;
						p.x = XmlConvert.ToSingle(reader.GetAttribute("X"));
						p.y = XmlConvert.ToSingle(reader.GetAttribute("Y"));
						rp.Points.Add(p);
						rd.RPoints.Add(p);
					}
				}

//				reader.Read(); // advance to the first Point
//				System.Diagnostics.Debug.Assert(reader.LocalName == "LeftHandPoints");
//				
//				while (reader.NodeType != XmlNodeType.EndElement)
//				{
//
//					reader.ReadStartElement("LeftHandPoints");
//				}
//
//				System.Diagnostics.Debug.Assert(reader.LocalName == "RightHandPoints");
//				
//				while (reader.NodeType != XmlNodeType.EndElement)
//				{
//					MyMath.Vector2 p = MyMath.Vector2.Zero;
//					p.x = XmlConvert.ToSingle(reader.GetAttribute("X"));
//					p.y = XmlConvert.ToSingle(reader.GetAttribute("Y"));
//					rp.Points.Add(p);
//					reader.ReadStartElement("RightHandPoints");
//					
//				}

				paths.Add(rp);
				pos.Add(rd);
				rl.AddResult(gesName, 0);

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
				//				string name = reader.GetAttribute("Name");
				
				int numPts = XmlConvert.ToInt32(reader.GetAttribute("NumPts"));
				string gesName = reader.GetAttribute("GesName");
				RecordedPath rp = new RecordedPath(numPts, gesName);
				RecordedData rd = new RecordedData(gesName);

				while(reader.Read()){
					if(reader.LocalName == "LeftHandPoints"){
						MyMath.Vector2 p = MyMath.Vector2.Zero;
						p.x = XmlConvert.ToSingle(reader.GetAttribute("X"));
						p.y = XmlConvert.ToSingle(reader.GetAttribute("Y"));
						rp.Points.Add(p);
						rd.LPoints.Add(p);
					}else if(reader.LocalName == "RightHandPoints"){
						MyMath.Vector2 p = MyMath.Vector2.Zero;
						p.x = XmlConvert.ToSingle(reader.GetAttribute("X"));
						p.y = XmlConvert.ToSingle(reader.GetAttribute("Y"));
						rp.Points.Add(p);
						rd.RPoints.Add(p);
					}
				}

//				reader.Read(); // advance to the first Point
//
//				System.Diagnostics.Debug.Assert(reader.LocalName == "LeftHandPoints");
//				
//				while (reader.NodeType != XmlNodeType.EndElement)
//				{
//					MyMath.Vector2 p = MyMath.Vector2.Zero;
//					p.x = XmlConvert.ToSingle(reader.GetAttribute("X"));
//					p.y = XmlConvert.ToSingle(reader.GetAttribute("Y"));
//					rp.Points.Add(p);
//					reader.ReadStartElement("LeftHandPoints");
//
//				}
//
//				System.Diagnostics.Debug.Assert(reader.LocalName == "RightHandPoints");
//				
//				while (reader.NodeType != XmlNodeType.EndElement)
//				{
//					MyMath.Vector2 p = MyMath.Vector2.Zero;
//					p.x = XmlConvert.ToSingle(reader.GetAttribute("X"));
//					p.y = XmlConvert.ToSingle(reader.GetAttribute("Y"));
//					rp.Points.Add(p);
//					reader.ReadStartElement("RightHandPoints");
//					
//				}

				rawData.Add(rp);
				rd.RSampleCount = rd.RPoints.Count;
				rd.LSampleCount = rd.LPoints.Count;
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
	}
}

