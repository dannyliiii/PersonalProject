using System.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using MyMath;
using UnityEngine;

namespace TemplateGesture{
	[Serializable]
	public class RecordedData {
		public string gestureName;
		List<MyMath.Vector2> lPoints;
		List<MyMath.Vector2> rPoints;
		int sampleCount;
//		int lSamplesCount;
//		int rSamplesCount;

//		public int LSampleCount{
//			get{
//				return lSamplesCount;
//			}
//			set{
//				lSamplesCount = value;
//			}
//		}
//
//		public int RSampleCount{
//			get{
//				return rSamplesCount;
//			}
//			set{
//				rSamplesCount = value;
//			}
//		}

		public int Size{
			get{
				return lPoints.Count;
			}
		}
		public int SampleCount{
			get{
				return sampleCount;
			}
			set{
				sampleCount = value;
			}
		}

		public List<MyMath.Vector2> LPoints
		{
			get { return lPoints; }
			set { lPoints = value; }
		}

		public List<MyMath.Vector2> RPoints
		{
			get { return rPoints; }
			set { rPoints = value; }
		}
		
		public RecordedData(string name,int sc)
		{
//			this.lSamplesCount = lsc;
//			this.rSamplesCount = rsc;

			this.sampleCount = sc;
			this.gestureName = name;
			lPoints = new List<MyMath.Vector2> ();
			rPoints = new List<MyMath.Vector2> ();
		}
		
//		public void CloseAndPrepare()
//		{
//			lPoints = GoldenSection.Pack(lPoints, sampleCount);
//			rPoints = GoldenSection.Pack(rPoints, sampleCount);
//		}
		
		public float Match(List<MyMath.Vector2> lPositions, List<MyMath.Vector2> rPositions, float threshold, float minSize)
		{
			if (lPositions.Count < 70)
				return -1;

//			if (!GoldenSectionExtension.IsLargeEnough(lPositions, minSize)|| !GoldenSectionExtension.IsLargeEnough(rPositions, minSize))
//				return -2;
			
			List<MyMath.Vector2> lLocals = GoldenSection.Pack(lPositions, sampleCount);
			List<MyMath.Vector2> rLocals = GoldenSection.Pack(rPositions, sampleCount);
			
			
			float lScore = GoldenSection.Search(lLocals, lPoints, -MathHelper.PiOver4, MathHelper.PiOver4, threshold);

			float rScore = GoldenSection.Search(rLocals, rPoints, -MathHelper.PiOver4, MathHelper.PiOver4, threshold);

			//UnityEngine.Debug.Log(score);
//			if (lScore < 0.6 || rScore < 0.6)
//				return (lScore + rScore) * 0.5f;	
//			else
				return Mathf.Max(lScore, rScore);
		}
		
	}
}