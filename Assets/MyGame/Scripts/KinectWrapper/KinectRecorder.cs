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
	public string suffix = ".data";
	public InputField nameField;
	public Canvas inputCanvas;
	
	private static bool isRecording = false;
	private ArrayList currentData = new ArrayList();
	private List<MyMath.Vector3> rhPos;
	private List<MyMath.Vector3> lhPos;
	Vector4 rightHand, leftHand;
	private List<Vector4> joints;
	
	// Use this for initialization
	void Awake () {
		inputCanvas.gameObject.SetActive (false);
		kinect = devOrEmu.getKinect();
		rhPos = new List<MyMath.Vector3> (256);
		lhPos = new List<MyMath.Vector3> (256);

		//to record the position of joints in the latest frame
		joints = new List<Vector4> ((int)NuiSkeletonPositionIndex.Count);
		for (int i = 0; i < (int)NuiSkeletonPositionIndex.Count; i ++) {
			joints[i] = Vector4.zero;
		}
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
				for (int i = 0; i < Kinect.Constants.NuiSkeletonCount; i++)
				{
					if (kinect.getSkeleton().SkeletonData[i].eTrackingState == Kinect.NuiSkeletonTrackingState.SkeletonTracked)
					{

						rightHand = kinect.getSkeleton().SkeletonData[i].SkeletonPositions[(int)NuiSkeletonPositionIndex.HandRight];
						leftHand = kinect.getSkeleton().SkeletonData[i].SkeletonPositions[(int)NuiSkeletonPositionIndex.HandLeft];
//						leftShoulder = kinect.getSkeleton().SkeletonData[i].SkeletonPositions[(int)NuiSkeletonPositionIndex.ShoulderLeft];
//						rightShoulder = kinect.getSkeleton().SkeletonData[i].SkeletonPositions[(int)NuiSkeletonPositionIndex.ShoulderRight];
//						head = kinect.getSkeleton().SkeletonData[i].SkeletonPositions[(int)NuiSkeletonPositionIndex.Head];

						for(int j = 0; j < (int)NuiSkeletonPositionIndex.Count; j ++){
							joints[j]= kinect.getSkeleton().SkeletonData[i].SkeletonPositions[j];
						}

//						long unixTimeStamp = (long)(System.DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds;
						rhPos.Add(new MyMath.Vector3(rightHand.x , rightHand.y, rightHand.z));
						lhPos.Add(new MyMath.Vector3(leftHand.x , leftHand.y, leftHand.z));
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
		rhPos.Clear ();
		lhPos.Clear ();
		Debug.Log("start recording");
	}
	
	void StopRecord() {
			
		isRecording = false;
		string gestureName = nameField.text;
		string filePath = outputFile + gestureName;
		
		SaveGesture (filePath, gestureName);
		Debug.Log("stop recording");
	}

	public bool SaveGesture(string path, string gestureName){

		if (lhPos.Count <= 0 && rhPos.Count<= 0) {
			Debug.Log("No points.");
			return false;
		}
	
		string filePath = path + suffix;
		
		//do xml writing
		bool success = true;
		XmlTextWriter rWriter = null;
		
		try
		{
			Constrain[] c = GetConstrains();

			//save data
			rWriter = new XmlTextWriter(filePath, Encoding.UTF8);
			rWriter.Formatting = Formatting.Indented;
			rWriter.WriteStartDocument(true);
			
			rWriter.WriteStartElement("Gesture");
			rWriter.WriteAttributeString("GesName", gestureName);
			rWriter.WriteAttributeString("NumPts", XmlConvert.ToString(rhPos.Count + lhPos.Count));
			//rWriter.WriteAttributeString("Millseconds", XmlConvert.ToString(points[points.Count - 1].time - points[0].time));
			rWriter.WriteAttributeString("Date", System.DateTime.Now.ToLongDateString());
			rWriter.WriteAttributeString("TimeOfDay", System.DateTime.Now.ToLongTimeString());

			for(int i =0; i < c.GetLength(); i ++){
				rWriter.WriteStartElement("Constrains");
				rWriter.WriteAttributeString("Value", XmlConvert.ToString(c[i]));
				rWriter.WriteEndElement(); 
			}

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
			if (rWriter != null )
				rWriter.Close();
			LearningMachine.LoadGestureNew(filePath);
		}
		return success; // Xml file successfully written (or not)
	
	}

	private Constrain[] GetConstrains(){
		//calculate the constrains
		Constrain[] constrains = new Constrain[4];

		//right hands - right shoulders
		if(joints[NuiSkeletonPositionIndex.HandRight].y < joints[NuiSkeletonPositionIndex.ShoulderRight].y){
			if(joints[NuiSkeletonPositionIndex.HandRight].x < joints[NuiSkeletonPositionIndex.ShoulderRight].x){
				constrains[0] = Constrain.down_left;
			}else{
				constrains[0]  = Constrain.down_right;
			}
		}else{
			if(joints[NuiSkeletonPositionIndex.HandRight].x < joints[NuiSkeletonPositionIndex.ShoulderRight].x){
				constrains[0]  = Constrain.up_left;
			}else{
				constrains[0]  = Constrain.up_right;
			}
		}
		
		return constrains;
	}
}
