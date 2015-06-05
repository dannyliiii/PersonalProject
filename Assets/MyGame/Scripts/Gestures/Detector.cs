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

//	List<Entry> el;
	int arrCount = 8;
	Material material1; 
	Material material2; 
	Material material3; 
	Material material4;
	Texture2D[] textureArr;

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

	enum TexArrEnum : int{
		t1 = 0,
		t2 = 1,
		t3 = 2,
		t4 = 3,
	}


	void Awake () {

		kinect = devOrEmu.getKinect();

		projList = new List<GameObject> ();
		upForce = new UnityEngine.Vector3 (0.0f, 150.0f, 0.0f);

		LoadTemplateGestureDetector ();

		//paths = templateGestureDetector.LearningMachine.Paths;

		//rawData = templateGestureDetector.LearningMachine.RawData;

		gs = new GUIStyle ();
		gs.fontSize = 40;
		
		textureArr = new Texture2D[arrCount];
		
		for (int i = 0; i < arrCount; i ++) {
			textureArr[i] = new Texture2D(512,512, TextureFormat.RGB24, false);
			textureArr[i].wrapMode = TextureWrapMode.Clamp;
		}


		material1 = dataImagePlaneRL.GetComponent<Renderer>().material;
		material2 = dataImagePlaneRR.GetComponent<Renderer>().material;
		material3 = dataImagePlane.GetComponent<Renderer>().material;
		material4 = dataImagePlane2.GetComponent<Renderer>().material;
		
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

		material1.SetTexture (0, new Texture2D(512,512, TextureFormat.RGB24, false));
		DestroyImmediate (material1.mainTexture);
		DestroyImmediate (textureArr[(int)TexArrEnum.t1]);
		textureArr[(int)TexArrEnum.t1] = new Texture2D (512, 512, TextureFormat.RGB24, false);
		material1.SetTexture (0, textureArr[(int)TexArrEnum.t1]);

		material2.SetTexture (0, new Texture2D(512,512, TextureFormat.RGB24, false));
		DestroyImmediate (material2.mainTexture);
		DestroyImmediate (textureArr[(int)TexArrEnum.t2]);
		textureArr[(int)TexArrEnum.t2] = new Texture2D (512, 512, TextureFormat.RGB24, false);
		material2.SetTexture (0, textureArr[(int)TexArrEnum.t2]);

		for (int i = 0; i < templateGestureDetector.Entries.Count ; i ++) {


			float x = Mathf.Round((-templateGestureDetector.Entries[i].PositionLeft.x + 1) * 256);
			float y = Mathf.Round((templateGestureDetector.Entries[i].PositionLeft.y  + 1) * 256);
			
			textureArr[(int)TexArrEnum.t1].DrawFilledCircle( (int)x, (int)y, 3, Color.green);
			
			
			float xe = Mathf.Round((-templateGestureDetector.Entries[i].PositionRight.x + 1) * 256);
			float ye = Mathf.Round((templateGestureDetector.Entries[i].PositionRight.y  + 1) * 256);
			
			textureArr[(int)TexArrEnum.t2].DrawFilledCircle( (int)xe, (int)ye, 3, Color.green);

		}

		textureArr[(int)TexArrEnum.t1].Apply ();
		textureArr[(int)TexArrEnum.t2].Apply ();

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

		if (LearningMachine.RawData.Count <= 0)
			return;
		RecordedData data = LearningMachine.RawPos [num];
		

		if (currentData >= data.Size)
			currentData = 0;


		material3.SetTexture (0, new Texture2D(512,512, TextureFormat.RGB24, false));
		DestroyImmediate (material3.mainTexture);
		DestroyImmediate (textureArr[(int)TexArrEnum.t3]);
		textureArr[(int)TexArrEnum.t3] = new Texture2D (512, 512, TextureFormat.RGB24, false);
		material3.SetTexture (0, textureArr[(int)TexArrEnum.t3]);

		material4.SetTexture (0, new Texture2D(512,512, TextureFormat.RGB24, false));
		DestroyImmediate (material4.mainTexture);
		DestroyImmediate (textureArr[(int)TexArrEnum.t4]);
		textureArr[(int)TexArrEnum.t4] = new Texture2D (512, 512, TextureFormat.RGB24, false);
		material4.SetTexture (0, textureArr[(int)TexArrEnum.t4]);

		for (int i = 0; i < currentData; i ++) {

			MyMath.Vector2 lStart = data.LPoints[i];
			MyMath.Vector2 lEnd = data.LPoints[i+1]; 
			MyMath.Vector2 rStart = data.RPoints[i];
			MyMath.Vector2 rEnd = data.RPoints[i+1];
	
//			textureArr[(int)TexArrEnum.t4].DrawFilledCircle( (rStart.x + 1) * 256, (rStart.y + 1) * 256, 3, Color.green);
//
//			textureArr[(int)TexArrEnum.t3].DrawFilledCircle( (lStart.x + 1) * 256, (lStart.y + 1) * 256, 3, Color.green);


			textureArr[(int)TexArrEnum.t4].DrawLine(new UnityEngine.Vector2(((rStart.x  + 1)) * 256, (rStart.y + 1) * 256),
			                                		new UnityEngine.Vector2(((rEnd.x + 1)) * 256, (rEnd.y + 1) * 256),
			                                        Color.red);

			textureArr[(int)TexArrEnum.t3].DrawLine(new UnityEngine.Vector2((lStart.x + 1) * 256, (lStart.y  + 1) * 256),
			                           				new UnityEngine.Vector2((lEnd.x + 1) * 256, (lEnd.y  + 1) * 256),
			                           				Color.blue);
		}

		textureArr [(int)TexArrEnum.t3].Apply ();
		textureArr [(int)TexArrEnum.t4].Apply ();


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

