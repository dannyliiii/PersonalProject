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

public class KinectRecorder : MonoBehaviour {
	
	public DeviceOrEmulator devOrEmu;
	private KinectInterface kinect;
	
	private string outputFile = "Assets/MyGame/Recordings/";
	public string suffix = ".xml";
	public string playbackSuffix = ".data";
	public string rawSuffix = ".raw";
	public InputField nameField;
	public Canvas inputCanvas;
	
	private static bool isRecording = false;
	private ArrayList currentData = new ArrayList();
	private List<JointPosition> points;
	private List<MyMath.Vector2> jointPos;
	
	// Use this for initialization
	void Awake () {
		inputCanvas.gameObject.SetActive (false);
		kinect = devOrEmu.getKinect();
		points = new List<JointPosition> (256);
		jointPos = new List<MyMath.Vector2> (256);
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
						long unixTimeStamp = (long)(System.DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds;
						points.Add(new JointPosition(rightHand.x, rightHand.y, rightHand.z, unixTimeStamp));
						jointPos.Add(new MyMath.Vector2(rightHand.x , rightHand.y));
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
		points.Clear ();

		Debug.Log("start recording");
	}
	
	void StopRecord() {
			
		isRecording = false;

		string gestureName = nameField.text;
		string filePath = outputFile + gestureName;
		
		SaveGesture (filePath, gestureName);
		SavePlayback (filePath, gestureName);
		SaveRawDate (filePath, gestureName);
		Debug.Log("stop recording");
	}

	public bool SaveRawDate(string path, string gestureName){
		
		if (points.Count <= 0) {
			Debug.Log("No points.");
			return false;
		}
		
		string filePath = path + rawSuffix;
		List<MyMath.Vector2> rawData = GoldenSection.Scale (jointPos, jointPos.Count);
		
		//do xml writing
		bool success = true;
		XmlTextWriter writer = null;
		
		try
		{
			// save the gesture data as an Xml file
			writer = new XmlTextWriter(filePath, Encoding.UTF8);
			writer.Formatting = Formatting.Indented;
			writer.WriteStartDocument(true);
			writer.WriteStartElement("RawData");
			writer.WriteAttributeString("GesName", gestureName);
			writer.WriteAttributeString("NumPts", XmlConvert.ToString(points.Count));
			writer.WriteAttributeString("Millseconds", XmlConvert.ToString(points[points.Count - 1].time - points[0].time));
			writer.WriteAttributeString("Date", System.DateTime.Now.ToLongDateString());
			writer.WriteAttributeString("TimeOfDay", System.DateTime.Now.ToLongTimeString());

			foreach (MyMath.Vector2 p in rawData)
			{
				writer.WriteStartElement("Point");
				writer.WriteAttributeString("X", XmlConvert.ToString(p.x));
				writer.WriteAttributeString("Y", XmlConvert.ToString(p.y));
				writer.WriteEndElement(); // <Point />
			}
			
			writer.WriteEndDocument(); // </RawData>
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
			Debug.Log (gestureName);
			if (writer != null)
				writer.Close();
		}
		return success; // Xml file successfully written (or not)
	
	}

	public bool SaveGesture(string path, string gestureName){


		if (points.Count <= 0) {
			Debug.Log("No points.");
			return false;
		}

		string filePath = path + suffix;

		List<MyMath.Vector2> locals = GoldenSection.Pack(jointPos, jointPos.Count );

		//do xml writing
		bool success = true;
		XmlTextWriter writer = null;

		try
		{
			// save the gesture data as an Xml file
			writer = new XmlTextWriter(filePath, Encoding.UTF8);
			writer.Formatting = Formatting.Indented;
			writer.WriteStartDocument(true);
			writer.WriteStartElement("Gesture");
			writer.WriteAttributeString("GesName", gestureName);
			writer.WriteAttributeString("NumPts", XmlConvert.ToString(points.Count));
			writer.WriteAttributeString("Millseconds", XmlConvert.ToString(points[points.Count - 1].time - points[0].time));
			writer.WriteAttributeString("Date", System.DateTime.Now.ToLongDateString());
			writer.WriteAttributeString("TimeOfDay", System.DateTime.Now.ToLongTimeString());
			
//			// write out the raw individual points
//			foreach (JointPosition p in points)
//			{
//				writer.WriteStartElement("Point");
//				writer.WriteAttributeString("X", XmlConvert.ToString(p.x));
//				writer.WriteAttributeString("Y", XmlConvert.ToString(p.y));
//				writer.WriteAttributeString("T", XmlConvert.ToString(p.time));
//				writer.WriteEndElement(); // <Point />
//			}

			// write out the raw individual points
			foreach (MyMath.Vector2 p in locals)
			{
				writer.WriteStartElement("Point");
				writer.WriteAttributeString("X", XmlConvert.ToString(p.x));
				writer.WriteAttributeString("Y", XmlConvert.ToString(p.y));
				writer.WriteEndElement(); // <Point />
			}

			writer.WriteEndDocument(); // </Gesture>
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
			Debug.Log (gestureName);
			if (writer != null)
				writer.Close();
		}
		return success; // Xml file successfully written (or not)

	}

	public bool SavePlayback(string path, string gestureName){

		bool success = true;
		string filePath = (path + playbackSuffix).ToString ();
		FileStream output = new FileStream (filePath, FileMode.Create);
		try{
			BinaryFormatter bf = new BinaryFormatter ();

			SerialSkeletonFrame[] data = new SerialSkeletonFrame[currentData.Count];
			for (int i = 0; i < currentData.Count; i ++) {
				data[i] = new SerialSkeletonFrame((NuiSkeletonFrame)currentData[i]);
			}
			bf.Serialize (output, data);
		}
		catch(Exception ex){
			Debug.Log(ex.Message);
			success = false;
		}
		finally{
			output.Close ();
		}
		return success;
	}
}
