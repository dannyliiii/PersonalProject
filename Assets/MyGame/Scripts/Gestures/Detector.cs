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
using System.Linq;
using Game;
using UnityEditor;
using System.Xml;

public class Detector : MonoBehaviour {

	public Player playerClass;

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

	public int player = 0;
	TemplatedGestureDetector templateGestureDetector;
	public SkeletonWrapper sw;

	private int currentData = 0;
	private string gesText = "";
	private int screenHeight = Screen.height;
	private int screenWidth = Screen.width;
	private GUIStyle gs;
	private int gesCount = 5;
	private int num = -1;
	private List<Vector4> joints;

	public bool controlMouse;
	public GUITexture handCursor;
	List<Vector4> rightHands;
	int rhpMax = 100;
	int timer = 40;

	public GameObject rightHand;
	public GameObject leftHand;
	public GameObject rhsp;
	public GUITexture GUIRH;

	Material planeMatTemp;
	Texture2D planeTemp;

	bool startScreen = true;

	Texture2D texture;
	RawImage[] img;

	public bool oneHanded;
	//gui scroll view
	public bool gesScroll;
	UnityEngine.Vector2 scrollPosition = UnityEngine.Vector2.zero;
//	UnityEngine.Vector2 scrollPositionText = UnityEngine.Vector2.zero;

	enum TexArrEnum : int{
		t1 = 0,
		t2 = 1,
		t3 = 2,
		t4 = 3,
	}

	void Awake () {

		Object.DontDestroyOnLoad (gameObject);

//		Debug.Log("detector awake");
		kinect = devOrEmu.getKinect();

		LoadTemplateGestureDetector ();

		gs = new GUIStyle ();
		gs.fontSize = 40;

//		UnityEngine.Debug.Log (EditorApplication.currentScene);
		if (EditorApplication.currentScene == "Assets/MyGame/Scenes/KinectSample.unity") {
			startScreen = false;

			textureArr = new Texture2D[arrCount];
			
			for (int i = 0; i < arrCount; i ++) {
				textureArr [i] = new Texture2D (512, 512, TextureFormat.RGB24, false);
				textureArr [i].wrapMode = TextureWrapMode.Clamp;
			}


			material1 = dataImagePlaneRL.GetComponent<Renderer> ().material;
			material2 = dataImagePlaneRR.GetComponent<Renderer> ().material;
			material3 = dataImagePlane.GetComponent<Renderer> ().material;
			material4 = dataImagePlane2.GetComponent<Renderer> ().material;

//			if (!controlMouse) {
//				//			leftHand.SetActive (false);
//				//			rightHand.SetActive (false);
//				handCursor.gameObject.SetActive (false);
//			}

		}
//		Debug.Log(EditorApplication.currentScene);
		if (EditorApplication.currentScene == "Assets/MyGame/Scenes/StartScreen.unity" /**&& img != null**/) {
			Debug.Log("In start screen");

			planeMatTemp = GameObject.Find("Plane").GetComponent<MeshRenderer>().material;
			planeTemp = new Texture2D(512,512, TextureFormat.RGB24, false);
			planeMatTemp.SetTexture(0, planeTemp);

			texture = new Texture2D(512,512, TextureFormat.RGB24, false);
			texture.wrapMode = TextureWrapMode.Clamp;
			
//			img = Camera.main.transform.Find("Canvas").gameObject.GetComponentsInChildren<RawImage>(); 
//			img [0].texture = texture;
//			img[0].color = Color.black;
//			texture = img[0].texture as Texture2D;
		}

		//to detect the position of joints in the latest frame
		joints = new List<Vector4> ();
		for (int i = 0; i < (int)NuiSkeletonPositionIndex.Count; i ++) {
			joints.Add(Vector4.zero);
		}



		rightHands = new List<Vector4> ();
		
//		var playerScript: OtherScript = GetComponent(OtherScript); 

	}

	void Update () {

		if (timer < 40)
			timer++;
	
		templateGestureDetector.oneHanded = oneHanded;
		DrawDataPerFrame (num);

		if(!KinectRecorder.IsRecording)
			ProcessFrame ();

//		UnityEngine.Debug.Log (startScreen);
		if (!startScreen) {
			DrawRealTimeHandsTracks ();
		} else {
			DrawRightHandTracksStartScreen();
		}

//		if (Input.GetKeyDown (KeyCode.L) && startScreen) {
//			Application.LoadLevel ("KinectSample");
//		}

	}

	void LoadTemplateGestureDetector()
	{
		templateGestureDetector = new TemplatedGestureDetector();
		templateGestureDetector.OnGestureDetected += OnGestureDetected;

	}

	void OnGestureDetected(string gesture)
	{
		gesText = gesture;


		switch (gesture){
		case "s":
			if(startScreen)
				Application.LoadLevel ("KinectSample");
			break;
		default:
			playerClass.CastSpell(gesture);
			break;
		}
	}

	void ProcessFrame()
	{
		if (kinect == null) {
			Debug.Log("kinect is null");
			return;
		}


		if (kinect.pollSkeleton()){

//			Debug.Log(Kinect.Constants.NuiSkeletonCount);
			
			// to record the first tracked player's right hand positon;
			for (int i = 0; i < Kinect.Constants.NuiSkeletonCount; i++)
			{
				if (kinect.getSkeleton().SkeletonData[i].eTrackingState == Kinect.NuiSkeletonTrackingState.SkeletonTracked)
				{
					for(int j = 0; j < (int)NuiSkeletonPositionIndex.Count; j ++){
						joints[j]= kinect.getSkeleton().SkeletonData[i].SkeletonPositions[j];
					}

//					if(Mathf.Abs(joints[(int)NuiSkeletonPositionIndex.HandRight].x - rightHands[rightHands.Count - 1].x) > 0.001 
//					   &&Mathf.Abs(joints[(int)NuiSkeletonPositionIndex.HandRight].x - rightHands[rightHands.Count - 1].x) > 0.001){
//						rightHands.Add(joints[(int)NuiSkeletonPositionIndex.HandRight]);
//					}else{
//						rightHands.Add(rightHands[rightHands.Count - 1]);
//					}

					if (rightHands.Count > rhpMax)
					{
						Vector4 entryToRemove = rightHands[0];
						rightHands.Remove(entryToRemove);
					}


					//control the mouse with right hand.
					if(controlMouse && GUIRH != null/**&& handCursor!= null && rightHand != null && leftHand != null**/ )
					{
						UnityEngine.Vector3 vCursorPos = new UnityEngine.Vector3 ((joints[(int)NuiSkeletonPositionIndex.HandRight].x + 1) * 0.5f,
						                                                          (joints[(int)NuiSkeletonPositionIndex.HandRight].y + 1) * 0.5f,
						                                                          joints[(int)NuiSkeletonPositionIndex.HandRight].z);

//						float x = (joints[(int)NuiSkeletonPositionIndex.HandRight].x) * 10.0f;
//						float y = ((joints[(int)NuiSkeletonPositionIndex.HandRight].y) * 2) * 5.0f;

//						float xL = (joints[(int)NuiSkeletonPositionIndex.HandLeft].x) * 10.0f;
//						float yL = ((joints[(int)NuiSkeletonPositionIndex.HandLeft].y) * 2) * 5.0f;
					

						float xGUI = (joints[(int)NuiSkeletonPositionIndex.HandRight].x + 1) * 0.5f;
						float yGUI = ((joints[(int)NuiSkeletonPositionIndex.HandRight].y) + 1) * 0.5f;
//						if(handCursor.GetComponent<GUITexture>() == null)
//						{
//							float zDist = handCursor.transform.position.z - Camera.main.transform.position.z;
//							vCursorPos.z = zDist;
//							
//							vCursorPos = Camera.main.ViewportToWorldPoint(vCursorPos);
//						}

						//interpolate the cursor position
//						handCursor.transform.position = UnityEngine.Vector3.Lerp(handCursor.transform.position, vCursorPos, 4 * Time.deltaTime);
//						rightHand.transform.position = UnityEngine.Vector3.Lerp(rightHand.transform.position, new UnityEngine.Vector3(x,y,rightHand.transform.position.z), 4 * Time.deltaTime);
//						leftHand.transform.position = UnityEngine.Vector3.Lerp(leftHand.transform.position, new UnityEngine.Vector3(xL,yL,leftHand.transform.position.z), 4 * Time.deltaTime);

						UnityEngine.Vector2 oldPos = GUIRH.transform.position;
						GUIRH.transform.position = UnityEngine.Vector2.Lerp(oldPos, new UnityEngine.Vector2(xGUI, yGUI), 4 * Time.deltaTime);

						//move the mouse
//						MouseControl.MouseMove(vCursorPos);

//						if(templateGestureDetector.MouseClicked(joints, rightHands) && timer == 40){
//							MouseControl.MouseClick();
//							timer = 0;
//						}
					}
					
//					Debug.Log("adding joints to entry");
					templateGestureDetector.Add(joints);
					templateGestureDetector.LookForGesture();
					break;
					
				}
			}
			
		}
	}

	void DrawRightHandTracksStartScreen(){
//		Debug.Log ("drawing right hand");
//		texture = new Texture2D (512, 512, TextureFormat.RGB24, false);
//		planeTemp = new Texture2D (512, 512, TextureFormat.RGB24, false);
		
//		planeMatTemp.SetTexture (0, new Texture2D(512,512, TextureFormat.RGB24, false));
		DestroyImmediate (planeMatTemp.mainTexture);
		DestroyImmediate (planeTemp);

		planeTemp = new Texture2D (512, 512, TextureFormat.RGB24, false);
		planeMatTemp.SetTexture (0, planeTemp);

		for (int i = 0; i < templateGestureDetector.Entries.Count ; i ++) {
			
			// draw x,y
			float x = Mathf.Round((templateGestureDetector.Entries[i].PositionRight.x + 1) * 256);
			float y = Mathf.Round((-templateGestureDetector.Entries[i].PositionRight.y  + 1) * 256);
			
//			texture.DrawFilledCircle( (int)x,  (int)y, 3, Color.black);
			planeTemp.DrawFilledCircle( (int)x,  (int)y, 3, Color.black);
			
//			Debug.Log(new UnityEngine.Vector2(x,y));

//			Texture tex = (Texture) AssetDatabase.LoadAssetAtPath("Assets/MyGame/Textures/Diamond.png", typeof(Texture));
//			img [0].texture = tex;
		}
//		texture.Apply ();
		planeTemp.Apply ();
//		img [0].texture = texture;
	}
	void DrawRealTimeHandsTracks(){

//		UnityEngine.Debug.Log (templateGestureDetector.Entries.Count);

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

			// draw x,y
			float x = Mathf.Round((-templateGestureDetector.Entries[i].PositionLeft.x + 1) * 256);
			float y = Mathf.Round((templateGestureDetector.Entries[i].PositionLeft.y  + 1) * 256);		
			
			float xe = Mathf.Round((-templateGestureDetector.Entries[i].PositionRight.x + 1) * 256);
			float ye = Mathf.Round((templateGestureDetector.Entries[i].PositionRight.y  + 1) * 256);

//			//draw x,z
//			float x = Mathf.Round((-templateGestureDetector.Entries[i].PositionLeft.x + 1) * 256);
//			float y = Mathf.Round((templateGestureDetector.Entries[i].PositionLeft.z) * 256);
//
//			float xe = Mathf.Round((-templateGestureDetector.Entries[i].PositionRight.x + 1) * 256);
//			float ye = Mathf.Round((templateGestureDetector.Entries[i].PositionRight.z) * 256);
			
			textureArr[(int)TexArrEnum.t1].DrawFilledCircle( (int)x, (int)y, 3, Color.black);
			textureArr[(int)TexArrEnum.t2].DrawFilledCircle( (int)xe, (int)ye, 3, Color.black);

		}

		textureArr[(int)TexArrEnum.t1].Apply ();
		textureArr[(int)TexArrEnum.t2].Apply ();

	} 

	void DrawDataPerFrame(int num){
		if (num == -1)
			return;
		if (LearningMachine.Pos.Count <= 0)
			return;
		RecordedData data = LearningMachine.Pos [num];
		

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

			//xy
			MyMath.Vector2 lStart = data.LPoints[i];
//			MyMath.Vector2 lEnd = data.LPoints[i+1]; 
			MyMath.Vector2 rStart = data.RPoints[i];
//			MyMath.Vector2 rEnd = data.RPoints[i+1];

			float x = Mathf.Round((rStart.x + 1) * 256);
			float y = Mathf.Round((rStart.y  + 1) * 256);

			float x2 = Mathf.Round((lStart.x + 1) * 256);
			float y2 = Mathf.Round((lStart.y  + 1) * 256);

//			//xz
//			MyMath.Vector2 lStart = new MyMath.Vector2 (data.ZX_LPoints[i].x, data.ZX_LPoints[i].y);
//			MyMath.Vector2 rStart = new MyMath.Vector2 (data.ZX_RPoints[i].x, data.ZX_RPoints[i].y);
//			
//			float x = Mathf.Round((rStart.x + 1) * 256);
//			float y = Mathf.Round((rStart.y) * 256);
//
//			float x2 = Mathf.Round((lStart.x + 1) * 256);
//			float y2 = Mathf.Round((lStart.y) * 256);


			
			textureArr[(int)TexArrEnum.t4].DrawFilledCircle( (int)x, (int)y, 3, Color.red);
			textureArr[(int)TexArrEnum.t3].DrawFilledCircle( (int)x2, (int)y2, 3, Color.blue);
		}

		textureArr [(int)TexArrEnum.t3].Apply ();
		textureArr [(int)TexArrEnum.t4].Apply ();


		currentData ++;
	}

	void OnLevelWasLoaded(int level) {
		if (level == 1) {
			print ("level 1 is loaded");

			GUIRH = GameObject.Find("GUIHand").GetComponent<GUITexture>();
			dataImagePlaneRR = GameObject.Find ("DataImagePlaneRR") as GameObject;
			dataImagePlaneRL = GameObject.Find ("DataImagePlaneRL") as GameObject;

			startScreen = false;
		
			textureArr = new Texture2D[arrCount];
		
			for (int i = 0; i < arrCount; i ++) {
				textureArr [i] = new Texture2D (512, 512, TextureFormat.RGB24, false);
				textureArr [i].wrapMode = TextureWrapMode.Clamp;
			}
		
		
			material1 = dataImagePlaneRL.GetComponent<Renderer> ().material;
			material2 = dataImagePlaneRR.GetComponent<Renderer> ().material;
//			material3 = dataImagePlane.GetComponent<Renderer> ().material;
//			material4 = dataImagePlane2.GetComponent<Renderer> ().material;
		
			playerClass = GameObject.Find ("Player").GetComponent<Player>() as Player;
			controlMouse = true;
			if (!controlMouse) {
				//			leftHand.SetActive (false);
				//			rightHand.SetActive (false);
//				handCursor.gameObject.SetActive (false);
			}
		}
		
	}

	void OnGUI() {
		if (startScreen) {
			return;
		}
		if (LearningMachine.Pos.Count <= 0)
			return;

		if (gesCount < LearningMachine.Pos.Count) {
			gesCount = LearningMachine.Pos.Count;
		}

		if (gesScroll) {
			//show the list of gesture templates
			scrollPosition = GUI.BeginScrollView (new Rect (screenWidth * 0.05f, screenHeight * 0.65f, 100, 200), 
		                                     scrollPosition, 
		                                     new Rect (screenWidth * 0.05f, screenHeight * 0.05f, 80, gesCount * 40),
		                    				 false, 
		                                     true);

			for (int i = 0; i < LearningMachine.Pos.Count; i ++) {
				if (GUI.Button (new Rect (screenWidth * 0.05f, screenHeight * 0.05f + i * 40, 80, 30), LearningMachine.Pos [i].gestureName)) {
					currentData = 0;
					num = i;
				}
			}

			GUI.EndScrollView ();
		}
		//show the score
//		scrollPositionText = GUI.BeginScrollView(new Rect(screenWidth * 0.7f, screenHeight * 0.05f  , screenWidth * 0.1f, 200), 
//		                                         scrollPositionText , 
//		                                         new Rect(screenWidth * 0.7f, screenHeight * 0.05f, screenWidth * 0.08f, gesCount * 40),
//		                                         false, 
//		                                         true);
//
//		for(int j = 0; j < LearningMachine.ResultList.Size(); j ++){
//			GUI.Label(new Rect(screenWidth * 0.7f, screenHeight * 0.05f + j * 40, 100, 20), LearningMachine.ResultList.GetName(j));
//			GUI.Label(new Rect(screenWidth * 0.75f, screenHeight * 0.05f + j * 40, 100, 20), LearningMachine.ResultList.GetScore(j).ToString());
//		}
//
//		GUI.EndScrollView ();
		
		string str = "Detected gesture shows here.";
		if(gesText != "")
			str = gesText + " detected";

		GUI.Label(new Rect(screenWidth * 0.35f , screenHeight * 0.01f , 200, 40), str, gs);	

//		if (GUI.Button(new Rect(screenWidth * 0.5f, screenHeight * 0.5f,200,200),"Click"))
//			Debug.Log("Button Clicked!");
	}
}

