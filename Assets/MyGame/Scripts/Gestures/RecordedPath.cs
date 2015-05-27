using System.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using MyMath;
using UnityEngine;

namespace TemplateGesture{
	[Serializable]
	public class RecordedPath {
		public string gestureName;
		List<MyMath.Vector2> points;
		readonly int samplesCount;
	
		public int SampleCount{
			get{
				return samplesCount;
			}
		}

		public List<MyMath.Vector2> Points
		{
			get { return points; }
			set { points = value; }
		}
		
		public RecordedPath(int samplesCount, string name)
		{
			this.samplesCount = samplesCount;
			this.gestureName = name;
			points = new List<MyMath.Vector2>();
		}
		
		public void CloseAndPrepare()
		{
			points = GoldenSection.Pack(points, samplesCount);
		}
		
		public float Match(List<MyMath.Vector2> positions, float threshold, float minimalScore, float minSize)
		{
//			if (positions.Count < samplesCount)
//				return -1;
			
			if (!GoldenSectionExtension.IsLargeEnough(positions, minSize))
				return -2;
			
			List<MyMath.Vector2> locals = GoldenSection.Pack(positions, samplesCount);
			
			float score = GoldenSection.Search(locals, points, -MathHelper.PiOver4, MathHelper.PiOver4, threshold);

			//UnityEngine.Debug.Log(score);

			return score;

			//return score > minimalScore;
		}
		
	}
}