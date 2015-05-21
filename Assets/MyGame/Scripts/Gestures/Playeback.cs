using UnityEngine;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace Kinect{
	public class Playeback : MonoBehaviour{

		private NuiSkeletonFrame[] skeletonFrame;
		private string folderPath = "Assets/MyGame/Recordings/";
		private string playbackSuffix = ".data";
//		private float playbackSpeed = 0.00333f;
		private float timer = 0.0f;
		//for temporary use
		string path;

//		private bool newSkeleton = false;
//		private int curFrame = 0;

		// Use this for initialization
		void Start () {
			path = folderPath + "a" + playbackSuffix;
			LoadPlaybackData (path);

		}
		
		// Update is called once per frame
		void Update () {
			timer += Time.deltaTime;
			if(Input.GetKeyUp(KeyCode.F12)) {
				LoadPlaybackData(path);
			}

		}

		void LateUpdate(){
//			newSkeleton = false;
		}

//		bool KinectInterface.pollSkeleton(){
//			int frame = Mathf.FloorToInt(Time.realtimeSinceStartup / playbackSpeed);
//			if(frame > curFrame){
//				curFrame = frame;
//				newSkeleton = true;
//				return newSkeleton;
//			}
//			return newSkeleton;
//		}

//		NuiSkeletonFrame KinectInterface.getSkeleton() {
//			return skeletonFrame[curFrame % skeletonFrame.Length];
//		}

		public void LoadPlaybackData(string file){
			
			FileStream input = new FileStream (file, FileMode.Open);
			BinaryFormatter bf = new BinaryFormatter ();
			SerialSkeletonFrame[] serialSkeleton = (SerialSkeletonFrame[])bf.Deserialize (input);
			skeletonFrame = new NuiSkeletonFrame[serialSkeleton.Length];
			for (int i = 0; i < serialSkeleton.Length; i ++) {
				skeletonFrame[i] = serialSkeleton[i].deserialize();
			}
			input.Close ();
			
		}

	}
}
