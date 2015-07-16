using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TemplateGesture{
	public class ResultList {

		private readonly double max = double.MaxValue;

		public class BestResult : IComparable
		{
			public static BestResult Empty = new BestResult(String.Empty, 0);
			
			private string name;
			private double score;
			private double radianceDiff = 0;
//			private bool xy, xz, yz;
			private int plane;

			public void SetScore(double s){
				score = s;
			}
			public void SetName(string n){
				name = n;
			}
			public void SetRadianceDiff(double diff){
				radianceDiff = diff;
			}
			// constructor
			public BestResult(string n, double s)
			{
				name = n;
				score = s;
			}

			public bool IsEmpty{
				get{
					return score == -1;
				}
			}
			public string Name{
				get{
					return name;
				}
			}
			public double Score{
				get{
					return score;
				}
			}
			public double Diff{
				get{
					return radianceDiff;
				}
			}
			public int Plane{
				get{
					return plane;
				}
				set{
					plane = value;
				}
			}

//			public bool XY{
//				get{
//					return xy;
//				}
//				set{
//					xy = value;
//				}
//			}
//			public bool XZ{
//				get{
//					return XZ;
//				}
//				set{
//					xz = value;
//				}
//			}
//			public bool YZ{
//				get{
//					return yz;
//				}
//				set{
//					yz = value;
//				}
//			}

			// sorts in descending order of Score
			public int CompareTo(object obj)
			{
				if (obj is BestResult) {
					BestResult r = (BestResult)obj;
					if (score < r.score)
						return 1;
					else if (score > r.score)
						return -1;
				} 
				return 0;
			}
		}


		private List<BestResult> arrList;

		public ResultList(){
			arrList = new List<BestResult> ();
		}

		public bool IsEmpty
		{
			get
			{
				return arrList.Count == 0;
			}
		}
		
		public void AddResult(string name, double score)
		{
			BestResult r = new BestResult(name, score);
			arrList.Add(r);
		}

		public void UpdateResult(int num, string name, double score, double diff = 0, int plane = 1){
			arrList [num].SetName (name);
			arrList [num].SetScore(score);
			arrList [num].SetRadianceDiff (diff);
			arrList [num].Plane = plane;

//			arrList [num].XY = xy;
//			arrList [num].XZ = xz;
//			arrList [num].YZ = yz;
//			for (int i = 0; i < arrList.Count; i ++) {
//				if(arrList[i].Name == name){
//					if(arrList[i].Score < score){
//						arrList[i].SetScore(score);
//					}
//				}
//			}
		}
		
		public void SortDescending()
		{
			arrList.Sort();
		}

//		public string Name
//		{
//			get
//			{
//				if (arrList.Count > 0)
//				{
////					BestResult r;
//					double score = 0;
//					string name = "";
//					for(int i = 0; i < arrList.Count; i++){
//						if(arrList[i].Score > score){
//							score = arrList[i].Score;
//							name = arrList[i].Name;
//						}
//					}
//					return name;
//				}
//				return String.Empty;
//			}
//		}
//		public double Score
//		{
//			get
//			{
//				if (arrList.Count > 0)
//				{
////					BestResult r;
//					double score = 0;
//					for(int i = 0; i < arrList.Count; i++){
//						if(arrList[i].Score > score){
//							score = arrList[i].Score;
//						}
//					}
//					return score;
//				}
//				return -1.0;
//			}
//		}

		public int Index{
			get{
//				int res = -1;
//				if (arrList.Count > 0)
//				{
//					double score = -1;
//					double score2 = -1;
//					bool flag = false;
//					for(int i = 0; i < arrList.Count; i++){
//						if(arrList[i].Plane > 1 && arrList[i].Score > LearningMachine.MinScore){
//							if(!flag)
//								flag = true;
//							if(arrList[i].Score > score2){
//								res = i;
//								score2 = arrList[i].Score;
//							}else{
//								//do nothing
//							}
//						}
//						if(!flag){
//							if(arrList[i].Score > score){
//								res = i;
//								score = arrList[i].Score;
//							}else if(arrList[i].Score == score){
//								if(arrList[res].Diff > arrList[i].Diff){
//									res = i;
//								}
//							}else{
//								//do nothing
//							}
//						}
//					}
//				}
//				return res;
				int res = -1;
				if (arrList.Count > 0)
				{
					double score = -1;
					for(int i = 0; i < arrList.Count; i++){
						if(arrList[i].Score > score){
							res = i;
							score = arrList[i].Score;
						}else if(arrList[i].Score == score){
							if(arrList[res].Diff > arrList[i].Diff){
								res = i;
							}
						}else{
							//do nothing
						}
					}
				}
				return res;
			}
		}
		public int IndexDTW{
			get{
				int res = -1;
				if (arrList.Count > 0)
				{
					double score = max;
					for(int i = 0; i < arrList.Count; i++){
						if(arrList[i].Score < score){
							res = i;
							score = arrList[i].Score;
						}else{
							//do nothing
						}
					}
				}
				return res;
			}
		}
		public double GetScore(string name){
			foreach (var v in arrList) {
				if(v.Name == name){
					return v.Score;
				}
			}
			return -1;
		}

		public double GetScore(int pos){
			if (arrList.Count > pos && pos >= 0)
			{
//				Debug.Log(arrList.Count);
//				Debug.Log(pos);
//				BestResult r = (BestResult) arrList[pos];
				return arrList[pos].Score;
			}
			return -1.0;
		}

		public string GetName(int pos){
			if (arrList.Count > pos && pos >= 0)
			{
//				BestResult r = (BestResult) arrList[pos];
				return arrList[pos].Name;
			}
			return "";
		}

		public int Size(){
			return arrList.Count;
		}

		public void ResetList(){
			for (int i = 0; i < arrList.Count; i ++) {
				arrList[i].SetScore (0);
			}
//			arrList.Clear ();
		}

	}
}
