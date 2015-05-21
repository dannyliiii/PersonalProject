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

public class Detector : MonoBehaviour {

	public GameObject dataImagePlane;
//	public RawImage dataImage;

	public int player = 0;
	TemplatedGestureDetector templateGestureDetector;
	public SkeletonWrapper sw;

	// for shooting projectile
	public GameObject projectile;
	private UnityEngine.Vector3 upForce;
	private readonly int speed = 50;
	private List<GameObject> projList;
	private List<RecordedPath> paths;
	private List<RecordedPath> rawData;
	private int currentData = 0;

	//for test
	private int[] testList;
	UnityEngine.Vector2 scrollPosition = UnityEngine.Vector2.zero;

	void Awake () {
		projList = new List<GameObject> ();
		upForce = new UnityEngine.Vector3 (0.0f, 150.0f, 0.0f);

		LoadTemplateGestureDetector ();

		paths = templateGestureDetector.LearningMachine.Paths;

		rawData = templateGestureDetector.LearningMachine.RawData;

		testList = new int[10];
		for(int i = 0 ; i < 10; i ++){
			testList[i] = i;
		}
		//DrawData (paths[0]);
	}

	void Update () {

//		if(rawData.Count > 0)
//			DrawDataPerFrame (rawData[0]);

		if(!KinectRecorder.IsRecording)
			ProcessFrame ();
	}

	void LoadTemplateGestureDetector()
	{
		templateGestureDetector = new TemplatedGestureDetector();
		templateGestureDetector.OnGestureDetected += OnGestureDetected;

	}

	void OnGestureDetected(string gesture)
	{
		//Shoot ();
	}

	void ProcessFrame()
	{
		if (sw.pollSkeleton ()) {
			for (int i = 0; i < (int)Kinect.NuiSkeletonPositionIndex.Count; i++) {

				if (i == (int)NuiSkeletonPositionIndex.HandRight) {
					MyMath.Vector3 pos = new MyMath.Vector3 (sw.bonePos[player, i].x
					                                         ,sw.bonePos[player, i].y
					                                         ,sw.bonePos[player, i].z);
					templateGestureDetector.Add (pos);
					
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

	void DrawDataPerFrame(RecordedPath path){

		if (currentData >= path.SampleCount - 1)
			currentData = 0;

		Material material = dataImagePlane.GetComponent<Renderer>().material;
		Texture2D texture = new Texture2D(512,512, TextureFormat.RGB24, false);
		texture.wrapMode = TextureWrapMode.Clamp;
		material.SetTexture(0, texture);
		

		for (int i = 0; i < currentData; i ++) {
//			MyMath.Vector2 start = MathHelper.NormalizeVector2D(path.Points[i]);
//			MyMath.Vector2 end = MathHelper.NormalizeVector2D(path.Points[i + 1]);

			MyMath.Vector2 start = path.Points[i];
			MyMath.Vector2 end = path.Points[i + 1];
			
			texture.DrawLine(new UnityEngine.Vector2((start.x + 1) * 256, (start.y + 1) * 256),
			                 new UnityEngine.Vector2((end.x + 1) * 256, (end.y + 1) * 256),
			                 Color.black);

		}

		texture.Apply();
		currentData ++;
	}

	void OnGUI() {

		scrollPosition = GUI.BeginScrollView(new Rect(20, 20, 100, 200), 
		                    scrollPosition , new Rect(25, 20, 80, 10 * 40),
		                    false, true);

		for (int i = 0; i < rawData.Count; i ++) {
			if (GUI.Button(new Rect(25, 20 + i * 40, 80, 30), rawData[i].gestureName)){
				currentData = 0;
				DrawDataPerFrame (rawData[i]);
			}
		}

		GUI.EndScrollView();
	
		for(int j = 0; j < templateGestureDetector.LearningMachine.ResultList.Size(); j ++){
			GUI.Label(new Rect(800, 20 + j * 40, 100, 20), templateGestureDetector.LearningMachine.ResultList.GetName(j));
			GUI.Label(new Rect(900, 20 + j * 40, 100, 20), templateGestureDetector.LearningMachine.ResultList.GetScore(j).ToString());
	
		}

		
	}
	
}

