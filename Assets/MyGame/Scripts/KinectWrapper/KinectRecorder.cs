using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Xml;
using System.Text;
using System;
using Kinect;
using MyMath;
using TemplateGesture;
using UnityEngine.UI;
using WobbrockLib;
using System.Drawing;

public class KinectRecorder : MonoBehaviour {
	
	public DeviceOrEmulator devOrEmu;
	private KinectInterface kinect;
	
	private string outputFile = "Assets/MyGame/Recordings/";
	public string suffix = ".xml";
	//public string playbackSuffix = ".data";
	public string rawSuffix = ".raw";
	public InputField nameField;
	public Canvas inputCanvas;
	
	private static bool isRecording = false;
	private ArrayList currentData = new ArrayList();
	//private List<JointPosition> points;
	private List<MyMath.Vector3> rhPos;
	private List<MyMath.Vector3> lhPos;

//	private List<TimePointF> rTimePoints;
//	private List<TimePointF> lTimePoints;
	
	// Use this for initialization
	void Awake () {
		inputCanvas.gameObject.SetActive (false);
		kinect = devOrEmu.getKinect();
	//	points = new List<JointPosition> (256);
		rhPos = new List<MyMath.Vector3> (256);
		lhPos = new List<MyMath.Vector3> (256);

//		List<TimePointF> rTimePoints = new List<TimePointF>();
//		List<TimePointF> lTimePoints = new List<TimePointF>();
	}
	
	// Update is called once per frame
	void Update () {
		if(!isRecording){
			if(Input.GetKeyDown(KeyCode.F10)){
				StartRecord();
			}
			if(Input.GetKeyDown(KeyCode.Return)){
				StopRecord();
				inputCanvas.gameObject.SetActive (false);
			}
		} else {
			if(Input.GetKeyDown(KeyCode.F10)){
				isRecording = false;
				inputCanvas.gameObject.SetActive (true);
				nameField.ActivateInputField ();
			}
			if (kinect.pollSkeleton()){

				//playback data
				currentData.Add(kinect.getSkeleton());

				// to record the first tracked player's right hand positon;
				for (int ii = 0; ii < Kinect.Constants.NuiSkeletonCount; ii++)
				{
					if (kinect.getSkeleton().SkeletonData[ii].eTrackingState == Kinect.NuiSkeletonTrackingState.SkeletonTracked)
					{

						Vector4 rightHand = kinect.getSkeleton().SkeletonData[ii].SkeletonPositions[(int)NuiSkeletonPositionIndex.HandRight];
						Vector4 leftHand = kinect.getSkeleton().SkeletonData[ii].SkeletonPositions[(int)NuiSkeletonPositionIndex.HandLeft];

//						long unixTimeStamp = (long)(System.DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds;
//						points.Add(new JointPosition(rightHand.x, rightHand.y, rightHand.z, unixTimeStamp));
						rhPos.Add(new MyMath.Vector3(rightHand.x , rightHand.y, rightHand.z));
						lhPos.Add(new MyMath.Vector3(leftHand.x , leftHand.y, leftHand.z));

//						TimePointF rp = TimePointF.Empty;
//						TimePointF lp = TimePointF.Empty;
//						rp.X = rightHand.x;
//						rp.Y = rightHand.y;
//						lp.X = leftHand.x;
//						lp.Y = leftHand.y;
//						rp.Time = lp.Time = System.DateTime.Now;
//
//						rTimePoints.Add(rp);
//						lTimePoints.Add(lp);

						break;

					}
				}

			}
		}
	}

	public static bool IsRecording{
		get{
			return isRecording;

		}

	}

	void StartRecord() {
		isRecording = true;
//		points.Clear ();
		rhPos.Clear ();
		lhPos.Clear ();
		Debug.Log("start recording");
	}
	
	void StopRecord() {
			
		isRecording = false;
		string gestureName = nameField.text;
		string filePath = outputFile + gestureName;
		
		SaveGesture (filePath, gestureName);
//		SavePlayback (filePath, gestureName);
//		SaveRawDate (filePath, gestureName);
		Debug.Log("stop recording");
	}

	public bool SaveGesture(string path, string gestureName){

		if (lhPos.Count <= 0 && rhPos.Count<= 0) {
			Debug.Log("No points.");
			return false;
		}
	
		//List<MyMath.Vector2> rawData = GoldenSection.Scale (jointPos, jointPos.Count);

//		List<MyMath.Vector3> l, r;
		
//		string pfilePath = path + rawSuffix;

//		l = lhPos;
//		r = rhPos;
	
		string filePath = path + suffix;
		string rFilePath = path + rawSuffix;
		string newFilePath = path + ".data";

		List<MyMath.Vector2> l = GoldenSection.Pack(lhPos, LearningMachine.sampleCount);
		List<MyMath.Vector2> r = GoldenSection.Pack(rhPos, LearningMachine.sampleCount);

//		List<PointF> lp = GoldenSection.DollarOnePack (rTimePoints);
//		List<PointF> rp = GoldenSection.DollarOnePack (lTimePoints);

		//do xml writing
		bool success = true;
		XmlTextWriter writer = null;
		XmlTextWriter rWriter = null;
		
		try
		{
			// save the gesture data as an Xml file
			writer = new XmlTextWriter(rFilePath, Encoding.UTF8);
			writer.Formatting = Formatting.Indented;
			writer.WriteStartDocument(true);

			writer.WriteStartElement("RawData");
			writer.WriteAttributeString("GesName", gestureName);
			writer.WriteAttributeString("NumPts", XmlConvert.ToString(l.Count + r.Count));
			//writer.WriteAttributeString("Millseconds", XmlConvert.ToString(points[points.Count - 1].time - points[0].time));
			writer.WriteAttributeString("Date", System.DateTime.Now.ToLongDateString());
			writer.WriteAttributeString("TimeOfDay", System.DateTime.Now.ToLongTimeString());

			foreach (MyMath.Vector3 p in lhPos)
			{
				writer.WriteStartElement("LeftHandPoints");
				writer.WriteAttributeString("X", XmlConvert.ToString(-p.x));
				writer.WriteAttributeString("Y", XmlConvert.ToString(p.y));
				writer.WriteAttributeString("Z", XmlConvert.ToString(p.z));
				writer.WriteEndElement(); 
			}

			foreach (MyMath.Vector3 p in rhPos)
			{
				writer.WriteStartElement("RightHandPoints");
				writer.WriteAttributeString("X", XmlConvert.ToString(-p.x));
				writer.WriteAttributeString("Y", XmlConvert.ToString(p.y));
				writer.WriteAttributeString("Z", XmlConvert.ToString(p.z));
				writer.WriteEndElement(); 
			}
			writer.WriteEndDocument(); // </RawData>


			//save processed data
			rWriter = new XmlTextWriter(filePath, Encoding.UTF8);
			rWriter.Formatting = Formatting.Indented;
			rWriter.WriteStartDocument(true);
			
			rWriter.WriteStartElement("ProcessedData");
			rWriter.WriteAttributeString("GesName", gestureName);
			rWriter.WriteAttributeString("NumPts", XmlConvert.ToString(l.Count + r.Count));
			//rWriter.WriteAttributeString("Millseconds", XmlConvert.ToString(points[points.Count - 1].time - points[0].time));
			rWriter.WriteAttributeString("Date", System.DateTime.Now.ToLongDateString());
			rWriter.WriteAttributeString("TimeOfDay", System.DateTime.Now.ToLongTimeString());

			foreach (MyMath.Vector2 p in l)
			{
				rWriter.WriteStartElement("LeftHandPoints");
				rWriter.WriteAttributeString("X", XmlConvert.ToString(p.x));
				rWriter.WriteAttributeString("Y", XmlConvert.ToString(p.y));
				rWriter.WriteEndElement(); 
			}
			
			foreach (MyMath.Vector2 p in r)
			{
				rWriter.WriteStartElement("RightHandPoints");
				rWriter.WriteAttributeString("X", XmlConvert.ToString(p.x));
				rWriter.WriteAttributeString("Y", XmlConvert.ToString(p.y));
				rWriter.WriteEndElement(); 
			}
			rWriter.WriteEndDocument();

			//save data new

			rWriter = new XmlTextWriter(newFilePath, Encoding.UTF8);
			rWriter.Formatting = Formatting.Indented;
			rWriter.WriteStartDocument(true);
			
			rWriter.WriteStartElement("Gesture");
			rWriter.WriteAttributeString("GesName", gestureName);
			rWriter.WriteAttributeString("NumPts", XmlConvert.ToString(l.Count + r.Count));
			//rWriter.WriteAttributeString("Millseconds", XmlConvert.ToString(points[points.Count - 1].time - points[0].time));
			rWriter.WriteAttributeString("Date", System.DateTime.Now.ToLongDateString());
			rWriter.WriteAttributeString("TimeOfDay", System.DateTime.Now.ToLongTimeString());

			foreach (var p in lhPos)
			{
				rWriter.WriteStartElement("LeftHandPoints");
				rWriter.WriteAttributeString("X", XmlConvert.ToString(p.x));
				rWriter.WriteAttributeString("Y", XmlConvert.ToString(p.y));
				rWriter.WriteAttributeString("Z", XmlConvert.ToString(p.z));
				rWriter.WriteEndElement(); 
			}
			
			foreach (var p in rhPos)
			{
				rWriter.WriteStartElement("RightHandPoints");
				rWriter.WriteAttributeString("X", XmlConvert.ToString(p.x));
				rWriter.WriteAttributeString("Y", XmlConvert.ToString(p.y));
				rWriter.WriteAttributeString("Z", XmlConvert.ToString(p.z));
				rWriter.WriteEndElement(); 
			}
			rWriter.WriteEndDocument();

			
		}
		catch (XmlException xex)
		{
			Debug.Log(xex.Message);
			success = false;
		}
		catch (Exception ex)
		{
			Debug.Log(ex.Message);
			success = false;
		}
		finally
		{
			//Debug.Log (gestureName);
			if (rWriter != null )
				rWriter.Close();
			if (writer != null){
				writer.Close();
			}
			LearningMachine.LoadGesture(filePath);
			LearningMachine.LoadRawData(rFilePath);
			LearningMachine.LoadGestureNew(newFilePath);
		}
		return success; // Xml file successfully written (or not)
	
	}

//	public bool SaveGesture(string path, string gestureName){
//
//
//		if (points.Count <= 0) {
//			Debug.Log("No points.");
//			return false;
//		}
//
//		string filePath = path + suffix;
//
//		List<MyMath.Vector2> locals = GoldenSection.Pack(jointPos, jointPos.Count );
//
//		//do xml writingSortDescending
//		bool success = true;
//		XmlTextWriter writer = null;
//
//		try
//		{
//			// save the gesture data as an Xml file
//			writer = new XmlTextWriter(filePath, Encoding.UTF8);
//			writer.Formatting = Formatting.Indented;
//			writer.WriteStartDocument(true);
//			writer.WriteStartElement("Gesture");
//			writer.WriteAttributeString("GesName", gestureName);
//			writer.WriteAttributeString("NumPts", XmlConvert.ToString(points.Count));
//			writer.WriteAttributeString("Millseconds", XmlConvert.ToString(points[points.Count - 1].time - points[0].time));
//			writer.WriteAttributeString("Date", System.DateTime.Now.ToLongDateString());
//			writer.WriteAttributeString("TimeOfDay", System.DateTime.Now.ToLongTimeString());
//			
////			// write out the raw individual points
////			foreach (JointPosition p in points)
////			{
////				writer.WriteStartElement("Point");
////				writer.WriteAttributeString("X", XmlConvert.ToString(p.x));
////				writer.WriteAttributeString("Y", XmlConvert.ToString(p.y));
////				writer.WriteAttributeString("T", XmlConvert.ToString(p.time));
////				writer.WriteEndElement(); // <Point />
////			}
//
//			// write out the raw individual points
//			foreach (MyMath.Vector2 p in locals)
//			{
//				writer.WriteStartElement("Point");
//				writer.WriteAttributeString("X", XmlConvert.ToString(p.x));
//				writer.WriteAttributeString("Y", XmlConvert.ToString(p.y));
//				writer.WriteEndElement(); // <Point />
//			}
//
//			writer.WriteEndDocument(); // </Gesture>
//		}
//		catch (XmlException xex)
//		{
//			Debug.Log(xex.Message);
//			success = false;
//		}
//		catch (Exception ex)
//		{
//			Debug.Log(ex.Message);
//			success = false;
//		}
//		finally
//		{
//
//			//Debug.Log (gestureName);
//			if (writer != null)
//				writer.Close();
//			LearningMachine.LoadGesture(filePath);
//		}
//		return success; // Xml file successfully written (or not)
//
//	}

//	public bool SavePlayback(string path, string gestureName){
//
//		bool success = true;
//		string filePath = (path + playbackSuffix).ToString ();
//		FileStream output = new FileStream (filePath, FileMode.Create);
//		try{
//			BinaryFormatter bf = new BinaryFormatter ();
//
//			SerialSkeletonFrame[] data = new SerialSkeletonFrame[currentData.Count];
//			for (int i = 0; i < currentData.Count; i ++) {
//				data[i] = new SerialSkeletonFrame((NuiSkeletonFrame)currentData[i]);
//			}
//			bf.Serialize (output, data);
//		}
//		catch(Exception ex){
//			Debug.Log(ex.Message);
//			success = false;
//		}
//		finally{
//			output.Close ();
//		}
//		return success;
//	}
}
