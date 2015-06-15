using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TemplateGesture{
	public class ResultList {

		public class BestResult : IComparable
		{
			public static BestResult Empty = new BestResult(String.Empty, 0);
			
			private string name;
			private double score;

			public void SetScore(double s){
				score = s;
			}
			public void SetName(string n){
				name = n;
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

		public void UpdateResult(int num, string name, double score){
			arrList [num].SetName (name);
			arrList [num].SetScore(score);

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
				int res = -1;
				if (arrList.Count > 0)
				{
					double score = -1;
					for(int i = 0; i < arrList.Count; i++){
						if(arrList[i].Score > score){
							res = i;
							score = arrList[i].Score;
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
