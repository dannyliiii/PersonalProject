using System.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using MyMath;
using UnityEngine;
using WobbrockLib;
using WobbrockLib.Extensions;
using System.Drawing;

namespace TemplateGesture{
	public enum Direction{
		up_left,
		up_right,
		down_left,
		down_right
	}


	public class RecordedData {
		public string gestureName;
		List<MyMath.Vector2> lPoints;
		List<MyMath.Vector2> rPoints;
		List<PointF> lpf;
		List<PointF> rpf;

		int sampleCount;

		Direction dl;
		Direction dr;

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
		public Direction DL{
			set{dl = value;}
			get{return dl;}
		}

		public Direction DR{
			set{dr = value;}
			get{return dr;}
		}
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

		public List<PointF> LP{
			get{return lpf;}
			set{lpf = value;}
		}

		public List<PointF> RP{
			get{return rpf;}
			set{rpf = value;}
		}
		
		public RecordedData(string name,int sc)
		{
//			this.lSamplesCount = lsc;
//			this.rSamplesCount = rsc;

			this.sampleCount = sc;
			this.gestureName = name;
			lPoints = new List<MyMath.Vector2> ();
			rPoints = new List<MyMath.Vector2> ();
			rpf = new List<PointF> ();
			lpf = new List<PointF> ();
		}
		
//		public void CloseAndPrepare()
//		{
//			lPoints = GoldenSection.Pack(lPoints, sampleCount);
//			rPoints = GoldenSection.Pack(rPoints, sampleCount);
//		}
		
		public double Match(List<TimePointF> tpfll, List<TimePointF> tpflr, List<MyMath.Vector2> lPositions, List<MyMath.Vector2> rPositions, float threshold, float minSize)
		{
			if (lPositions.Count < 70)
				return -1;

//			if (!GoldenSectionExtension.IsLargeEnough(lPositions, minSize)|| !GoldenSectionExtension.IsLargeEnough(rPositions, minSize))
//				return -2;
			List<PointF> pfl = GoldenSection.DollarOnePack (tpfll, sampleCount);
			List<PointF> pfr = GoldenSection.DollarOnePack (tpflr, sampleCount); 


			double[] bestl = GoldenSection.GoldenSectionSearch(
				pfl,                             // to rotate
				lpf,                           // to match
				GeotrigEx.Degrees2Radians(-45.0),   // lbound
				GeotrigEx.Degrees2Radians(+45.0),   // ubound
				GeotrigEx.Degrees2Radians(2.0)      // threshold
				);
			
			double scorel = 1.0 - bestl[0] / GoldenSection.HalfDiagonal;

			double[] bestr = GoldenSection.GoldenSectionSearch(
				pfr,                             // to rotate
				rpf,                           // to match
				GeotrigEx.Degrees2Radians(-45.0),   // lbound
				GeotrigEx.Degrees2Radians(+45.0),   // ubound
				GeotrigEx.Degrees2Radians(2.0)      // threshold
				);
			
			double scorer = 1.0 - bestr[0] / GoldenSection.HalfDiagonal;


//			List<MyMath.Vector2> lLocals = GoldenSection.Pack(lPositions, sampleCount);
//			List<MyMath.Vector2> rLocals = GoldenSection.Pack(rPositions, sampleCount);
			
			
//			float lScore = GoldenSection.Search(lLocals, lPoints, -MathHelper.PiOver4, MathHelper.PiOver4, threshold);
//
//			float rScore = GoldenSection.Search(rLocals, rPoints, -MathHelper.PiOver4, MathHelper.PiOver4, threshold);

			//UnityEngine.Debug.Log(score);
//			if (lScore < 0.6 || rScore < 0.6)
//				return (lScore + rScore) * 0.5f;	
//			else

			if (scorel > scorer)
				return scorel;
			else
				return scorer;
		}
		
	}
}