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
		int lSamplesCount;
		int rSamplesCount;

		public int LSampleCount{
			get{
				return lSamplesCount;
			}
			set{
				lSamplesCount = value;
			}
		}

		public int RSampleCount{
			get{
				return rSamplesCount;
			}
			set{
				rSamplesCount = value;
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
		
		public RecordedData(string name)
		{
//			this.lSamplesCount = lsc;
//			this.rSamplesCount = rsc;
			
			this.gestureName = name;
			lPoints = new List<MyMath.Vector2> ();
			rPoints = new List<MyMath.Vector2> ();
		}
		
		public void CloseAndPrepare()
		{
			lPoints = GoldenSection.Pack(lPoints, lPoints.Count);
			rPoints = GoldenSection.Pack(rPoints, rPoints.Count);
		}
		
		public float Match(List<MyMath.Vector2> lPositions, List<MyMath.Vector2> rPositions, RecordedData p, float threshold, float minimalScore, float minSize)
		{
			//			if (positions.Count < samplesCount)
			//				return -1;
			
			if (!GoldenSectionExtension.IsLargeEnough(lPositions, minSize)|| !GoldenSectionExtension.IsLargeEnough(rPositions, minSize))
				return -2;
			
			List<MyMath.Vector2> lLocals = GoldenSection.Pack(lPositions, lPositions.Count);
			List<MyMath.Vector2> rLocals = GoldenSection.Pack(rPositions, rPositions.Count);
			
			
			float lScore = GoldenSection.Search(lLocals, p.LPoints, -MathHelper.PiOver4, MathHelper.PiOver4, threshold);

			float rScore = GoldenSection.Search(rLocals, p.rPoints, -MathHelper.PiOver4, MathHelper.PiOver4, threshold);

			//UnityEngine.Debug.Log(score);
			
			return Mathf.Max(lScore, rScore);
		}
		
	}
}