using System.Collections;
//using System.IO;
using System.Collections.Generic;
using System;
using System.Linq;
using MyMath;
using UnityEngine;
using UnityEngine.UI;

namespace TemplateGesture{
	public class TemplatedGestureDetector {

//		RecordedPath path;

		public int MinimalPeriodBetweenGestures { get; set; }
		
		readonly List<Entry> entries = new List<Entry>();
		
		public event Action<string> OnGestureDetected;
		
		DateTime lastGestureDate = DateTime.Now;
		
		readonly int windowSize; // Number of recorded positions
		
		public float Epsilon { get; set; }
		public float MinimalScore { get; set; }
		public float MinimalSize { get; set; }
		private readonly float minScore = 0.73f;


		//change window size when change samplecount in learning machine
		public TemplatedGestureDetector(int windowSize = 100){
			this.windowSize = windowSize;
			MinimalPeriodBetweenGestures = 0;
			
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
		
		public void Add(MyMath.Vector3 posL, MyMath.Vector3 posR){
			//1. left, 2. right

			Entry newEntry = new Entry {PositionRight = posR, PositionLeft = posL, Time = DateTime.Now};
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

			//ResultList resList = LearningMachine.Match (Entries.Select (e => new MyMath.Vector2 (e.Position.x, e.Position.y)).ToList (), Epsilon, MinimalScore, MinimalSize);

			ResultList resList = LearningMachine.Match (Entries.Select (e => new MyMath.Vector2 (e.PositionLeft.x, e.PositionLeft.y)).ToList (),
			                                            Entries.Select (e => new MyMath.Vector2 (e.PositionRight.x, e.PositionRight.y)).ToList(),
			                                            Epsilon, MinimalSize);
			
			//resList.SortDescending ();
			if (!resList.IsEmpty) {
//				string gesName = resList.Name;
//				double gesScore = resList.Score;
				int index = resList.Index;
		
				if(resList.GetScore(index) > minScore){
					RaiseGestureDetected(resList.GetName(index));
				}
				//Entries.Clear();
			}

		}

		private void RaiseGestureDetected(string gesture)
		{

			if (DateTime.Now.Subtract(lastGestureDate).TotalMilliseconds > MinimalPeriodBetweenGestures)
			{
				if (OnGestureDetected != null)
					OnGestureDetected(gesture);
				
				lastGestureDate = DateTime.Now;
			}

			LearningMachine.ResultList.ResetList ();
			//clear the point list
			Entries.Clear();

			//Debug.Log (Entries.Count ());
		}
		
	}
}
