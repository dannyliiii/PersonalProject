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
		//readonly LearningMachine learningMachine;
		private readonly float minScore = 0.7f;

//		public LearningMachine LearningMachine{
//			get{
//				return learningMachine;
//			}
//		}

		public TemplatedGestureDetector(int windowSize = 200){
			this.windowSize = windowSize;
			MinimalPeriodBetweenGestures = 0;
			
			Epsilon = 0.035f;
			MinimalScore = 0.80f;
			MinimalSize = 0.1f;
			LearningMachine.Initialize ();
			//learningMachine = new LearningMachine();
		}

		protected List<Entry> Entries
		{
			get { return entries; }
		}
		
		public int WindowSize
		{
			get { return windowSize; }
		}
		
		public void Add(MyMath.Vector3 position){

			Entry newEntry = new Entry {Position = position, Time = DateTime.Now};
			Entries.Add(newEntry);

			// Remove too old positions
			if (Entries.Count > WindowSize)
			{
				Entry entryToRemove = Entries[0];
				Entries.Remove(entryToRemove);
			}
			
			// Look for gestures
			LookForGesture();

//			if (path != null)
//			{
//				path.Points.Add(MyMath.Vector3.ToVector2(position));
//			}

			
		}

		private void LookForGesture()
		{
			ResultList resList = LearningMachine.Match (Entries.Select (e => new MyMath.Vector2 (e.Position.x, e.Position.y)).ToList (), Epsilon, MinimalScore, MinimalSize);
			//resList.SortDescending ();
			if (!resList.IsEmpty) {
				string gesName = resList.Name;
				double gesScore = resList.Score;
				if(gesName == "z"){
					// for tesing
				}
				if(gesScore > minScore){
					RaiseGestureDetected(gesName);
				}
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

			//clear the point list
			Entries.Clear();
			LearningMachine.ResultList.ClearList ();
			//Debug.Log (Entries.Count ());
		}
		
	}
}
