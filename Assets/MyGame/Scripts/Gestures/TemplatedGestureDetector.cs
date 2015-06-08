using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using MyMath;
using UnityEngine;
using UnityEngine.UI;
using WobbrockLib;

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
		
		public void Add(MyMath.Vector3 posL, MyMath.Vector3 posR){

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

			Entry newEntry = new Entry {PositionRight = posR, 
										PositionLeft = posL,
										Time = DateTime.Now,
										TpfPosLeft = pl,
										TpfPosRight = pr,
										ZY_TpfPosLeft = zy_pl,
										ZY_TpfPosRight = zy_pr,
										ZX_TpfPosLeft = zx_pl,
										ZX_TpfPosRight = zx_pr,};
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

			ResultList resList = LearningMachine.Match (Entries.Select (e => e.TpfPosLeft).ToList(),
			                                            Entries.Select (e => e.TpfPosRight).ToList(),
			                                            Entries.Select (e => e.ZY_TpfPosLeft).ToList(),
			                                            Entries.Select (e => e.ZY_TpfPosRight).ToList(),
			                                            Entries.Select (e => e.ZX_TpfPosLeft).ToList(),
			                                            Entries.Select (e => e.ZX_TpfPosRight).ToList(),
			                                            Epsilon, MinimalSize);

			if (!resList.IsEmpty) {
				int index = resList.Index;

				if(resList.GetScore(index) > minScore){

					RaiseGestureDetected(resList.GetName(index));

					LearningMachine.ResultList.ResetList ();
					//clear the point list
					Entries.Clear();

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

		}
		
	}
}
