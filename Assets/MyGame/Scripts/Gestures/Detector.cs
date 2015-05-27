using UnityEngine;
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
		projList = new List<GameObject> ();
		upForce = new UnityEngine.Vector3 (0.0f, 150.0f, 0.0f);

		LoadTemplateGestureDetector ();

		//paths = templateGestureDetector.LearningMachine.Paths;

		//rawData = templateGestureDetector.LearningMachine.RawData;

		gs = new GUIStyle ();
		gs.fontSize = 40;

		//DrawData (paths[0]);
	}

	void Update () {

		if (num != -1)
			//DrawData (LearningMachine.RawData [num]);
			DrawDataPerFrame (num);

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
		gesText = gesture;
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

					templateGestureDetector.LookForGesture();
//					if(KinectSensor.Instance.getIsRightHandGrip()){
//						//Debug.Log("Right hand grips.");
//						MyMath.Vector3 pos = new MyMath.Vector3 (sw.bonePos[player, i].x
//						                                         ,sw.bonePos[player, i].y
//						                                         ,sw.bonePos[player, i].z);
//						templateGestureDetector.Add (pos);
//					}else{
//						//Debug.Log("Right hand releases.");
//						templateGestureDetector.LookForGesture();
//					}
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

	void DrawDataPerFrame(int num){

		if (LearningMachine.RawData.Count <= 0)
			return;
		RecordedPath path = LearningMachine.RawData [num];

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
	

		scrollPositionText = GUI.BeginScrollView(new Rect(screenWidth * 0.7f, screenHeight * 0.2f  , screenWidth * 0.1f, 200), 
		                                         scrollPositionText , 
		                                         new Rect(screenWidth * 0.7f, screenHeight * 0.2f, screenWidth * 0.08f, gesCount * 40),
		                                         false, 
		                                         true);

		for(int j = 0; j < LearningMachine.ResultList.Size(); j ++){
//			GUI.Label(new Rect(screenWidth * 0.7f, screenHeight * 0.05f + j * 40, 100, 20), templateGestureDetector.LearningMachine.ResultList.GetName(j));
//			GUI.Label(new Rect(screenWidth * 0.75f, screenHeight * 0.05f + j * 40, 100, 20), templateGestureDetector.LearningMachine.ResultList.GetScore(j).ToString());
			GUI.Label(new Rect(screenWidth * 0.7f, screenHeight * 0.2f + j * 40, 100, 20), LearningMachine.ResultList.GetName(j));
			GUI.Label(new Rect(screenWidth * 0.75f, screenHeight * 0.2f + j * 40, 100, 20), LearningMachine.ResultList.GetScore(j).ToString());
	
		}

		GUI.EndScrollView ();

		string str = "Detected gesture shows here.";
		if(gesText != "")
			str = gesText + " detected";

		GUI.Label(new Rect(screenWidth * 0.5f , screenHeight * 0.05f , 200, 40), str, gs);

		
	}
	
}

