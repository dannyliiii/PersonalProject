﻿using System.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using MyMath;
using UnityEngine;
using WobbrockLib;
using WobbrockLib.Extensions;
using System.Drawing;

namespace TemplateGesture{

	public class RecordedData {
		public string gestureName;
		List<MyMath.Vector2> lPoints;
		List<MyMath.Vector2> rPoints;
		List<PointF> lpf;
		List<PointF> rpf;

		int sampleCount;

		//for the z-y plane
		List<PointF> zy_lpf;
		List<PointF> zy_rpf;
		public List<PointF> ZY_LP{
			get{return zy_lpf;}
			set{zy_lpf = value;}
		}
		
		public List<PointF> ZY_RP{
			get{return zy_rpf;}
			set{zy_rpf = value;}
		}

		// getter/setter
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
			this.sampleCount = sc;
			this.gestureName = name;
			lPoints = new List<MyMath.Vector2> ();
			rPoints = new List<MyMath.Vector2> ();
			rpf = new List<PointF> ();
			lpf = new List<PointF> ();
			zy_lpf = new List<PointF> ();
			zy_rpf = new List<PointF> ();
		}
		

		public double Match(List<TimePointF> tpfll, List<TimePointF> tpflr, float threshold, float minSize, int plane)
		{
//			if (lPositions.Count < 70)
//				return -1;

//			if (!GoldenSectionExtension.IsLargeEnough(lPositions, minSize)|| !GoldenSectionExtension.IsLargeEnough(rPositions, minSize))
//				return -2;

			List<PointF> pfl = GoldenSection.DollarOnePack (tpfll, sampleCount);
			List<PointF> pfr = GoldenSection.DollarOnePack (tpflr, sampleCount); 

			List<PointF> rdl, rdr;

			// 1: xy_plane 2: zy_plane
			if (plane == 1) {
				rdr = new List<PointF> (rpf);
				rdl = new List<PointF> (lpf);
			} else{
				rdr = new List<PointF> (zy_rpf);
				rdl = new List<PointF> (zy_lpf);
			}

			double[] bestl = GoldenSection.GoldenSectionSearch(
				pfl,                             // to rotate
				rdl,                           // to match
				GeotrigEx.Degrees2Radians(-45.0),   // lbound
				GeotrigEx.Degrees2Radians(+45.0),   // ubound
				GeotrigEx.Degrees2Radians(2.0)      // threshold
				);
			
			double scorel = 1.0 - bestl[0] / GoldenSection.HalfDiagonal;

			double[] bestr = GoldenSection.GoldenSectionSearch(
				pfr,                             // to rotate
				rdr,                           // to match
				GeotrigEx.Degrees2Radians(-45.0),   // lbound
				GeotrigEx.Degrees2Radians(+45.0),   // ubound
				GeotrigEx.Degrees2Radians(2.0)      // threshold
				);
			
			double scorer = 1.0 - bestr[0] / GoldenSection.HalfDiagonal;

			return (scorel + scorer) * 0.5f;
		}
		
	}
}