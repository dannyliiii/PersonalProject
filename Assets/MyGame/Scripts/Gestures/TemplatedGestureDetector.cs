using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using MyMath;
using UnityEngine;
using UnityEngine.UI;
using WobbrockLib;
using Kinect;

namespace TemplateGesture{
	public class TemplatedGestureDetector {

		public int MinimalPeriodBetweenGestures { get; set; }

		readonly List<Entry> entries = new List<Entry>();
		
		public event Action<string> OnGestureDetected;
		
		DateTime lastGestureDate = DateTime.Now;
		
		readonly int windowSize; // Number of recorded positions
		
		public float Epsilon { get; set; }
		public float MinimalScore { get; set; }
		public float MinimalSize { get; set; }
		private readonly float minScore = 0.85f;
		private readonly int frameCount = 40;
		

		//change window size when change samplecount in learning machine
		public TemplatedGestureDetector(int windowSize = 100){
			this.windowSize = windowSize;
			MinimalPeriodBetweenGestures = 500;
			
			Epsilon = 0.035f;
			MinimalScore = 0.80f;
			MinimalSize = 0.1f;
			LearningMachine.Initialize ();
		}
		public List<Entry> Entries
		{
			get { return entries; }
		}
		
		public int WindowSize
		{
			get { return windowSize; }
		}
		
		public void Add(List<Vector4> joints){

			MyMath.Vector3 posR = new MyMath.Vector3 (joints[(int)NuiSkeletonPositionIndex.HandRight].x, joints[(int)NuiSkeletonPositionIndex.HandRight].y, joints[(int)NuiSkeletonPositionIndex.HandRight].z);
			MyMath.Vector3 posL = new MyMath.Vector3 (joints[(int)NuiSkeletonPositionIndex.HandLeft].x, joints[(int)NuiSkeletonPositionIndex.HandLeft].y, joints[(int)NuiSkeletonPositionIndex.HandLeft].z);
			

			TimePointF pr = TimePointF.Empty;
			pr.X = posR.x;
			pr.Y = posR.y;

			TimePointF pl = TimePointF.Empty;
			pl.X = posL.x;
			pl.Y = posL.y;

			TimePointF zy_pr = TimePointF.Empty;
			zy_pr.X = posR.z;
			zy_pr.Y = posR.y;
			
			TimePointF zy_pl = TimePointF.Empty;
			zy_pl.X = posL.z;
			zy_pl.Y = posL.y;

			TimePointF zx_pr = TimePointF.Empty;
			zx_pr.X = posR.x;
			zx_pr.Y = posR.z;
			
			TimePointF zx_pl = TimePointF.Empty;
			zx_pl.X = posL.x;
			zx_pl.Y = posL.z;

			Constrain[] c = GestureConstrains.GetConstrains (joints);
//			foreach (var v in c) {
//				Debug.Log(v);
//			}
//			Debug.Log ("=========");
			Entry newEntry = new Entry {PositionRight = posR, 
										PositionLeft = posL,
										Time = DateTime.Now,
										TpfPosLeft = pl,
										TpfPosRight = pr,
										ZY_TpfPosLeft = zy_pl,
										ZY_TpfPosRight = zy_pr,
										ZX_TpfPosLeft = zx_pl,
										ZX_TpfPosRight = zx_pr,
										constrain = c};
			Entries.Add(newEntry);

			// Remove too old positions
			if (Entries.Count > WindowSize)
			{
				Entry entryToRemove = Entries[0];
				Entries.Remove(entryToRemove);
			}
		}

		public void LookForGesture()
		{
			if (Entries.Count <= 0)
				return;

			if (!IsFinished (Entries.Select (e => e.TpfPosLeft).ToList ()) && !IsFinished (Entries.Select (e => e.TpfPosRight).ToList ()))
				return;

			ResultList resList = LearningMachine.Match (Entries.Select (e => e.TpfPosLeft).ToList(),
			                                            Entries.Select (e => e.TpfPosRight).ToList(),
			                                            Entries.Select (e => e.ZY_TpfPosLeft).ToList(),
			                                            Entries.Select (e => e.ZY_TpfPosRight).ToList(),
			                                            Entries.Select (e => e.ZX_TpfPosLeft).ToList(),
			                                            Entries.Select (e => e.ZX_TpfPosRight).ToList(),
			                                            entries.Last().constrain,
			                                            Epsilon,
			                                            MinimalSize);


//			Debug.Log(resList.GetScore("hdel3"));
//			if (!resList.IsEmpty) {
				int index = resList.Index;

				if(resList.GetScore(index) > minScore){

					RaiseGestureDetected(resList.GetName(index));
				}
				LearningMachine.ResultList.ResetList ();
				//clear the point list
				Entries.Clear();
//			}
//			Debug.Log("Entries Clear.");			

		}

		private void RaiseGestureDetected(string gesture)
		{

			if (DateTime.Now.Subtract(lastGestureDate).TotalMilliseconds > MinimalPeriodBetweenGestures)
			{
				if (OnGestureDetected != null)
					OnGestureDetected(gesture);
				
				lastGestureDate = DateTime.Now;
			}

		}

		public bool MouseClicked(List<Vector4> joints, List<Vector4>rh){
			bool res = false;

			bool p1 = true;
			if (rh.Count < frameCount)
				return res;


			MyMath.Vector2 v1 = new MyMath.Vector2(rh [rh.Count - 1].x, rh [rh.Count - 1].y);
			for (int i = rh.Count-2; i > rh.Count - frameCount; i --) {
//				Debug.Log(Entries[i].PositionRight.z );
				MyMath.Vector2 v2 = new MyMath.Vector2 (rh[i].x, rh[i].y);
				MyMath.Vector2 v3 = v1 - v2;
				if(v3.LengthSqr > 0.001f){
					p1 = false;
					break;
				}
			}

			if (p1) {
				if(joints[(int)NuiSkeletonPositionIndex.HandLeft].y > joints[(int)NuiSkeletonPositionIndex.Head].y){
					res = true;
				}
			}

			if(res)
				Debug.Log ("click!");
			
			return res;
		}

		// to check if the gesture is finished
		public bool IsFinished(List<TimePointF> list){

			bool res = false;
			int count = 0;
			int listCount = 0;
		
			if (list.Count > frameCount) {
				MyMath.Vector2 v1 = new MyMath.Vector2 (list [list.Count - 1].X, list [list.Count - 1].Y);
				for (int i = list.Count - 2; i > 1; i --) {
					MyMath.Vector2 v2 = new MyMath.Vector2 (list [i].X, list [i].Y);
					MyMath.Vector2 v3 = v1 - v2;
	//				Debug.Log(v3.LengthSqr);
					if (v3.LengthSqr < 0.001f) {
						count ++;
					}
					listCount ++;

					if (count >= listCount) {
						if (count > frameCount) {
							res = true;
//							UnityEngine.Debug.Log ("Gesture Finished!");
							break;
						}
					} else {
						break;
					}
				}
			}
//			Debug.Log (count);
			return res;
		}
		
	}
}
