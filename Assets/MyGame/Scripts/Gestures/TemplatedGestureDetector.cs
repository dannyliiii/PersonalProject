﻿using System.Collections;
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

		readonly List<Entry> entriesForRec = new List<Entry>();
		public bool record = false;

		public event Action<string> OnGestureDetected;
		
		DateTime lastGestureDate = DateTime.Now;
		
		readonly int windowSize; // Number of recorded positions
		
		public float Epsilon { get; set; }
		public float MinimalScore { get; set; }
		public float MinimalSize { get; set; }
		private readonly int frameCount = 30;

		public bool oneHanded;

		//change window size when change samplecount in learning machine
		public TemplatedGestureDetector(int windowSize = 100){
			this.windowSize = windowSize;
			MinimalPeriodBetweenGestures = 500;
			
			Epsilon = 0.035f;
			MinimalScore = 0.80f;
			MinimalSize = 0.1f;
			LearningMachine.Initialize ();
		}

		public List<Entry> EntriesForRec
		{
			get { return entriesForRec; }
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

			if (record) {
				entries.Clear();
				Entry recEntry = new Entry {PositionRight = posR, 
					PositionLeft = posL,
					Time = DateTime.Now,
					TpfPosLeft = pl,
					TpfPosRight = pr,
					ZY_TpfPosLeft = zy_pl,
					ZY_TpfPosRight = zy_pr,
					ZX_TpfPosLeft = zx_pl,
					ZX_TpfPosRight = zx_pr,
					constrain = c};
				EntriesForRec.Add (recEntry);

				// Remove too old positions
				if (EntriesForRec.Count > WindowSize)
				{
//					Entry entryToRemove = EntriesForRec[0];
//					EntriesForRec.Remove(entryToRemove);
					EntriesForRec.Clear();
					record = false;
				}


			} else {
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
				Entries.Add (newEntry);
				if(!oneHanded){
					if (IsFinished (Entries.Select (e => e.TpfPosLeft).ToList (), false) && IsFinished (Entries.Select (e => e.TpfPosRight).ToList (), false)) {
						record = true;
						Debug.Log("Start");
					}
				}
				else{
					if (IsFinished (Entries.Select (e => e.TpfPosRight).ToList (), false)) {
						record = true;
						Debug.Log("Start");
					}
				}
				// Remove too old positions
				if (Entries.Count > WindowSize)
				{
					Entry entryToRemove = Entries[0];
					Entries.Remove(entryToRemove);
				}
			}
//			Debug.Log (Entries.Count);
		}

		public void LookForGesture(int method)
		{
			if (EntriesForRec.Count <= 0)
				return;

			if (!oneHanded) {
				if (!IsFinished (EntriesForRec.Select (e => e.TpfPosLeft).ToList (), false) && !IsFinished (EntriesForRec.Select (e => e.TpfPosRight).ToList (), false)) {
					return;
				}
			} else {
				if (!IsFinished (EntriesForRec.Select (e => e.TpfPosRight).ToList (), false)) {
					return;
				}
			}

			entriesForRec.RemoveRange (entriesForRec.Count - frameCount - 1, frameCount);

			record = false;
			Debug.Log("Finish");
			ResultList resList = LearningMachine.Match (EntriesForRec.Select (e => e.TpfPosLeft).ToList(),
			                                            EntriesForRec.Select (e => e.TpfPosRight).ToList(),
			                                            EntriesForRec.Select (e => e.ZY_TpfPosLeft).ToList(),
			                                            EntriesForRec.Select (e => e.ZY_TpfPosRight).ToList(),
			                                            EntriesForRec.Select (e => e.ZX_TpfPosLeft).ToList(),
			                                            EntriesForRec.Select (e => e.ZX_TpfPosRight).ToList(),
			                                            EntriesForRec.Select (e => e.PositionLeft).ToList(),
			                                            EntriesForRec.Select (e => e.PositionRight).ToList(),		                           
			                                            EntriesForRec.Last().constrain,
			                                            Epsilon,
			                                            MinimalSize, oneHanded, method);

			int index = 0;
			if (method == 1) {
				index = resList.Index;

				bool flag = true;
				if (resList.GetScore (index) > LearningMachine.MinScore) {
					flag = false;
					RaiseGestureDetected (resList.GetName (index));
				} else {
					flag = false;
					RaiseGestureDetected ("No Gesture Recognized");
				}
				if (resList.GetName (index) == "s" && resList.GetScore (index) > LearningMachine.MinScoreOneHanded && flag) {
					RaiseGestureDetected (resList.GetName (index));
				}

//			} else if (method == 2 || method == 3) {
//				if(resList.GetScore (index) > LearningMachine.MinScoreDTW){
//					index = resList.IndexDTW;
//					RaiseGestureDetected (resList.GetName (index));
//				}
//				else {
//					RaiseGestureDetected ("No Gesture Recognized");
//				}
////				if (resList.GetScore (index) < 1.5f && resList.GetScore (index) > 0) {
////					RaiseGestureDetected (resList.GetName (index));
////				} else {
////					RaiseGestureDetected ("No Gesture Recognized");
////				}
////				UnityEngine.Debug.Log("%^%&^£*(&!£*)(!*£_!*£");
////				UnityEngine.Debug.Log(resList.GetScore(index));
//
			}else{
				index = resList.Index;

				if (resList.GetScore (index) > LearningMachine.MinScoreDTW) {
					RaiseGestureDetected (resList.GetName (index));
				} else {
					RaiseGestureDetected ("No Gesture Recognized");
				}
//				index = resList.Index;
			}

			LearningMachine.ResultList.ResetList ();
			//clear the point list
			EntriesForRec.Clear();
//			Entries.Clear ();
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
		public bool IsFinished(List<TimePointF> list, bool all = false){

			bool res = false;
			int count = 0;
			int listCount = 0;
			MyMath.Vector2 v1 = new MyMath.Vector2 (list [list.Count - 1].X, list [list.Count - 1].Y);
	
			if (list.Count >= frameCount) {

				for (int i = list.Count - 2; i > 1; i --) {
					MyMath.Vector2 v2 = new MyMath.Vector2 (list [i].X, list [i].Y);
					MyMath.Vector2 v3 = v1 - v2;
					//				Debug.Log(v3.LengthSqr);
					if (v3.LengthSqr < 0.002f) {
						count ++;
					}
					listCount ++;

					if (listCount >= frameCount) {
//						Debug.Log(123123123132);
						if (listCount - count < 3) {
							res = true;
//							UnityEngine.Debug.Log ("Gesture Finished!");
							break;
						} else {
							break;
						}
					}
				}
			}
			 if(all){
				for (int i = list.Count - 2; i > 1; i --) {
					MyMath.Vector2 v2 = new MyMath.Vector2 (list [i].X, list [i].Y);
					MyMath.Vector2 v3 = v1 - v2;
					//				Debug.Log(v3.LengthSqr);
					if (v3.LengthSqr < 0.0001f) {
						count ++;
					}
					listCount ++;
				}
				if (listCount - count < 5) {
					//						Debug.Log(123123123132);
					res = false;
				}
			}
//			Debug.Log (count);
			return res;
		}

		public void ClearData(){
			entries.Clear();
			entriesForRec.Clear ();
			record = false;
		}
	}
}
