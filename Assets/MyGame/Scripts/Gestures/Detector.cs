﻿using UnityEngine;
using System.Collections;
//using System.IO;
using TemplateGesture;
using KinectWrapper;
using Kinect;
using MyMath;
using System.Collections.Generic;
using UnityEngine.UI;
using Drawing;
using System.Linq;

public class Detector : MonoBehaviour {

//	List<MyMath.Vector2> l;
//	List<MyMath.Vector2> r;
	Material material1; 
	Material material2; 
	Material material3; 
	Material material4;
	Texture2D texture1;
	Texture2D texture2;
	Texture2D texture3;
	Texture2D texture4;

	public DeviceOrEmulator devOrEmu;
	private KinectInterface kinect;

	public GameObject dataImagePlane;
	public GameObject dataImagePlane2;

	public GameObject dataImagePlaneRL;
	public GameObject dataImagePlaneRR;
//	public RawImage dataImage;

	public int player = 0;
	TemplatedGestureDetector templateGestureDetector;
	public SkeletonWrapper sw;

	private UnityEngine.Vector3 upForce;
	private readonly int speed = 50;
	private List<GameObject> projList;
//	private List<RecordedPath> paths;
//	private List<RecordedPath> rawData;
	private int currentData = 0;
	private string gesText = "";
	private int screenHeight = Screen.height;
	private int screenWidth = Screen.width;
	private GUIStyle gs;
	private int gesCount = 5;
	private int num = -1;
	// shooting projectile
	public GameObject projectile;

	//gui scroll view
	UnityEngine.Vector2 scrollPosition = UnityEngine.Vector2.zero;
	UnityEngine.Vector2 scrollPositionText = UnityEngine.Vector2.zero;

	void Awake () {

		kinect = devOrEmu.getKinect();

		projList = new List<GameObject> ();
		upForce = new UnityEngine.Vector3 (0.0f, 150.0f, 0.0f);

		LoadTemplateGestureDetector ();

		//paths = templateGestureDetector.LearningMachine.Paths;

		//rawData = templateGestureDetector.LearningMachine.RawData;

		gs = new GUIStyle ();
		gs.fontSize = 40;

		material1 = dataImagePlaneRL.GetComponent<Renderer>().material;
		material2 = dataImagePlaneRR.GetComponent<Renderer>().material;
		material3 = dataImagePlane.GetComponent<Renderer>().material;
		material4 = dataImagePlane2.GetComponent<Renderer>().material;
		texture1 = new Texture2D(512,512, TextureFormat.RGB24, false);
		texture2 = new Texture2D(512,512, TextureFormat.RGB24, false);
		texture3 = new Texture2D(512,512, TextureFormat.RGB24, false);
		texture4 = new Texture2D(512,512, TextureFormat.RGB24, false);
		texture1.wrapMode = TextureWrapMode.Clamp;
		texture2.wrapMode = TextureWrapMode.Clamp;
		texture3.wrapMode = TextureWrapMode.Clamp;
		texture4.wrapMode = TextureWrapMode.Clamp;
		material1.SetTexture(0, texture1);
		material2.SetTexture(0, texture2);
		material3.SetTexture(0, texture3);
		material4.SetTexture(0, texture4);
		

		//DrawData (paths[0]);
	}

	void Update () {

		if (num != -1)
			//DrawData (LearningMachine.RawData [num]);
			DrawDataPerFrame (num);

		if(!KinectRecorder.IsRecording)
			ProcessFrame ();

		DrawRealTimeHandsTracks ();
	}

	void LoadTemplateGestureDetector()
	{
		templateGestureDetector = new TemplatedGestureDetector();
		templateGestureDetector.OnGestureDetected += OnGestureDetected;

	}

	void OnGestureDetected(string gesture)
	{
		gesText = gesture;
		//Shoot ();
	}

	void ProcessFrame()
	{
		if (kinect == null)
			return;

		if (kinect.pollSkeleton()){

			
			// to record the first tracked player's right hand positon;
			for (int ii = 0; ii < Kinect.Constants.NuiSkeletonCount; ii++)
			{
				if (kinect.getSkeleton().SkeletonData[ii].eTrackingState == Kinect.NuiSkeletonTrackingState.SkeletonTracked)
				{
					
					Vector4 rightHand = kinect.getSkeleton().SkeletonData[ii].SkeletonPositions[(int)NuiSkeletonPositionIndex.HandRight];
					Vector4 leftHand = kinect.getSkeleton().SkeletonData[ii].SkeletonPositions[(int)NuiSkeletonPositionIndex.HandLeft];

					templateGestureDetector.Add ( new MyMath.Vector3(leftHand.x, leftHand.y, leftHand.z),
					                             new MyMath.Vector3(rightHand.x, rightHand.y, rightHand.z)
					                            );
					templateGestureDetector.LookForGesture();
					break;
					
				}
			}
			
		}


//		if (sw.pollSkeleton ()) {
//
//			for (int i = 0; i < (int)Kinect.NuiSkeletonPositionIndex.Count; i++) {
//				bool flag = false;
//				MyMath.Vector3 posL = MyMath.Vector3.Zero;
//				MyMath.Vector3 posR = MyMath.Vector3.Zero;
//				if (i == (int)NuiSkeletonPositionIndex.HandRight) {
//
//					posR = new MyMath.Vector3 (sw.bonePos[player, i].x
//					                                         ,sw.bonePos[player, i].y
//					                                         ,sw.bonePos[player, i].z);
////					templateGestureDetector.Add (pos);
//					flag = true;
////					templateGestureDetector.LookForGesture();
//
////					if(KinectSensor.Instance.getIsRightHandGrip()){
////						//Debug.Log("Right hand grips.");
////						MyMath.Vector3 pos = new MyMath.Vector3 (sw.bonePos[player, i].x
////						                                         ,sw.bonePos[player, i].y
////						                                         ,sw.bonePos[player, i].z);
////						templateGestureDetector.Add (pos);
////					}else{
////						//Debug.Log("Right hand releases.");
////						templateGestureDetector.LookForGesture();
////					}
//				}
//				if (i == (int)NuiSkeletonPositionIndex.HandLeft) {
//					
//					posL = new MyMath.Vector3 (sw.bonePos[player, i].x
//					                                         ,sw.bonePos[player, i].y
//					                                         ,sw.bonePos[player, i].z);
////					templateGestureDetector.Add (pos);
//					flag = true;
//
//				}
//				if(flag){
//					templateGestureDetector.Add (posL, posR);
//					templateGestureDetector.LookForGesture();
//				}
//			}
//		}
	}

	void Shoot(){
		
		GameObject proj =  Instantiate(projectile, transform.position, Quaternion.FromToRotation (UnityEngine.Vector3.forward, transform.forward)) as GameObject;
		Rigidbody rb = proj.GetComponent<Rigidbody> ();
		rb.velocity = transform.forward * speed;
		rb.AddForce (upForce);
		ParticleSystem ps = proj.GetComponent<ParticleSystem>();
		ps.Play ();
		projList.Add (proj);
	}

	void DrawRealTimeHandsTracks(){

//		List<MyMath.Vector2> l= GoldenSection.Pack(templateGestureDetector.Entries.Select (e => new MyMath.Vector2 (e.PositionLeft.x, e.PositionLeft.y)).ToList (),100);
//		List<MyMath.Vector2> r= GoldenSection.Pack(templateGestureDetector.Entries.Select (e => new MyMath.Vector2 (e.PositionRight.x, e.PositionRight.y)).ToList (),100);


//		l= templateGestureDetector.Entries.Select (e => new MyMath.Vector2 (e.PositionLeft.x, e.PositionLeft.y)).ToList ();
//		r= templateGestureDetector.Entries.Select (e => new MyMath.Vector2 (e.PositionRight.x, e.PositionRight.y)).ToList ();

//		List<Entry> entryList = templateGestureDetector.Entries;



		for (int i = 0; i < templateGestureDetector.Entries.Count ; i ++) {


			float x = Mathf.Round((-templateGestureDetector.Entries[i].PositionLeft.x + 1) * 256);
			float y = Mathf.Round((templateGestureDetector.Entries[i].PositionLeft.y  + 1) * 256);
			
			texture1.DrawFilledCircle( (int)x, (int)y, 3, Color.green);
			
			
			float xe = Mathf.Round((-templateGestureDetector.Entries[i].PositionRight.x + 1) * 256);
			float ye = Mathf.Round((templateGestureDetector.Entries[i].PositionRight.y  + 1) * 256);
			
			texture2.DrawFilledCircle( (int)xe, (int)ye, 3, Color.green);

		}


//		for (int i = 0; i < entryList.Count - 1; i ++) {
//
//			float x = Mathf.Round((-entryList[i].PositionLeft.x + 1) * 256);
//			float y = Mathf.Round((entryList[i].PositionLeft.y  + 1) * 256);
//			
//			texture2.DrawFilledCircle( (int)x, (int)y, 3, Color.green);
//
//
//			float xe = Mathf.Round((-entryList[i].PositionRight.x + 1) * 256);
//			float ye = Mathf.Round((entryList[i].PositionRight.y  + 1) * 256);
//
//			texture.DrawFilledCircle( (int)xe, (int)ye, 3, Color.green);
//
////			texture2.DrawLine(new UnityEngine.Vector2(( -entryList[i].PositionLeft.x + 1) * 256, (entryList[i].PositionLeft.y  + 1) * 256),
////			                 new UnityEngine.Vector2(( -entryList[i+1].PositionLeft.x + 1) * 256, (entryList[i+1].PositionLeft.y  + 1) * 256),
////			                 Color.red);
//
////			texture.DrawLine(new UnityEngine.Vector2(( -entryList[i].PositionRight.x + 1) * 256, (entryList[i].PositionRight.y  + 1) * 256),
////			                  new UnityEngine.Vector2(( -entryList[i+1].PositionRight.x + 1) * 256, (entryList[i+1].PositionRight.y  + 1) * 256),
////			                 Color.blue);
//		}
		texture1.Apply ();
		texture2.Apply ();
//		l.Clear ();
//		r.Clear ();

	} 


	void DrawData(RecordedPath path){
		
		Material material = dataImagePlane.GetComponent<Renderer>().material;
		Texture2D texture = new Texture2D(512,512, TextureFormat.RGB24, false);
		texture.wrapMode = TextureWrapMode.Clamp;
		material.SetTexture(0, texture);
	
		//texture.DrawLine(new UnityEngine.Vector2(0, 0), new UnityEngine.Vector2(512, 256), Color.black);

		for (int i = 0; i < path.SampleCount - 1; i ++) {

//			MyMath.Vector2 start = MathHelper.NormalizeVector2D(path.Points[i]);
//			MyMath.Vector2 end = MathHelper.NormalizeVector2D(path.Points[i + 1]);

			MyMath.Vector2 start = path.Points[i];
			MyMath.Vector2 end = path.Points[i + 1];

			texture.DrawLine(new UnityEngine.Vector2((start.x + 1) * 256, (start.y + 1) * 256),
			                 new UnityEngine.Vector2((end.x + 1) * 256, (end.y + 1) * 256),
			                 Color.black);

		}
		
		texture.Apply();

	}

	void DrawDataPerFrame(int num){
//		Debug.Log (currentData);

//		Material material3 = dataImagePlane.GetComponent<Renderer>().material;
//		Material material4 = dataImagePlane2.GetComponent<Renderer>().material;
//		Texture2D texture3 = new Texture2D(512,512, TextureFormat.RGB24, false);
//		Texture2D texture4 = new Texture2D(512,512, TextureFormat.RGB24, false);

//		Texture2D texture3 = material3.mainTexture as Texture2D;
//		texture3.wrapMode = TextureWrapMode.Clamp;
//		texture4.wrapMode = TextureWrapMode.Clamp;
//		material3.SetTexture(0, texture3);
//		material4.SetTexture(0, texture4);

		if (LearningMachine.RawData.Count <= 0)
			return;
//		RecordedPath path = LearningMachine.RawData [num];

		RecordedData data = LearningMachine.RawPos [num];

//		if (currentData >= path.SampleCount - 1)
//			currentData = 0;
		

		if (currentData >= data.Size)
			currentData = 0;

		for (int i = 0; i < currentData; i ++) {

//			MyMath.Vector2 start = path.Points[i];
//			MyMath.Vector2 end = path.Points[i + 1];
			MyMath.Vector2 lStart = data.LPoints[i];
			MyMath.Vector2 lEnd = data.LPoints[i+1]; 
			MyMath.Vector2 rStart = data.RPoints[i];
			MyMath.Vector2 rEnd = data.RPoints[i+1];
	

//			if(i < data.LSampleCount){
//				lStart = data.LPoints[i];
//				lEnd = data.LPoints[i+1];
//			}else{
//				lStart = data.LPoints[data.LSampleCount - 2];
//				lEnd = data.LPoints[data.LSampleCount - 1];
//			}
//
//			if(i < data.RSampleCount){
//				rStart = data.RPoints[i];
//				rEnd = data.RPoints[i+1];
//			}else{
//				rStart = data.RPoints[data.LSampleCount -2];
//				rEnd = data.RPoints[data.LSampleCount - 1];
//			}
		
//			if(lStart != MyMath.Vector2.Zero)
			texture3.DrawLine(new UnityEngine.Vector2((lStart.x + 1) * 256, (lStart.y  + 1) * 256),
				                 new UnityEngine.Vector2((lEnd.x + 1) * 256, (lEnd.y  + 1) * 256),
			                 Color.blue);

//			if(rStart != MyMath.Vector2.Zero)
			texture4.DrawLine(new UnityEngine.Vector2(((rStart.x  + 1)) * 256, (rStart.y + 1) * 256),
			                  new UnityEngine.Vector2(((rEnd.x + 1)) * 256, (rEnd.y + 1) * 256),
			                 Color.red);
		}
		
		texture3.Apply ();
		texture4.Apply();
	
//		Texture2D.Destroy (texture3);
//		Texture2D.Destroy (texture4);


		currentData ++;
	}

	void OnGUI() {

		if (LearningMachine.RawData.Count <= 0)
			return;

		if (gesCount < LearningMachine.RawData.Count) {
			gesCount = LearningMachine.RawData.Count;
		}

		scrollPosition = GUI.BeginScrollView(new Rect(screenWidth * 0.05f, screenHeight * 0.05f , 100, 200), 
		                                     scrollPosition , 
		                                     new Rect(screenWidth * 0.05f, screenHeight * 0.05f, 80, gesCount * 40),
		                    				 false, 
		                                     true);

		for (int i = 0; i < LearningMachine.RawData.Count; i ++) {
			if (GUI.Button(new Rect(screenWidth * 0.05f, screenHeight * 0.05f + i * 40, 80, 30), LearningMachine.RawData[i].gestureName)){
				currentData = 0;
				num = i;
			}
		}

		GUI.EndScrollView();
	

		scrollPositionText = GUI.BeginScrollView(new Rect(screenWidth * 0.7f, screenHeight * 0.05f  , screenWidth * 0.1f, 200), 
		                                         scrollPositionText , 
		                                         new Rect(screenWidth * 0.7f, screenHeight * 0.05f, screenWidth * 0.08f, gesCount * 40),
		                                         false, 
		                                         true);

		for(int j = 0; j < LearningMachine.ResultList.Size(); j ++){
//			GUI.Label(new Rect(screenWidth * 0.7f, screenHeight * 0.05f + j * 40, 100, 20), templateGestureDetector.LearningMachine.ResultList.GetName(j));
//			GUI.Label(new Rect(screenWidth * 0.75f, screenHeight * 0.05f + j * 40, 100, 20), templateGestureDetector.LearningMachine.ResultList.GetScore(j).ToString());
			GUI.Label(new Rect(screenWidth * 0.7f, screenHeight * 0.05f + j * 40, 100, 20), LearningMachine.ResultList.GetName(j));
			GUI.Label(new Rect(screenWidth * 0.75f, screenHeight * 0.05f + j * 40, 100, 20), LearningMachine.ResultList.GetScore(j).ToString());
	
		}

		GUI.EndScrollView ();

		string str = "Detected gesture shows here.";
		if(gesText != "")
			str = gesText + " detected";

		GUI.Label(new Rect(screenWidth * 0.35f , screenHeight * 0.05f , 200, 40), str, gs);

		
	}
	
}

