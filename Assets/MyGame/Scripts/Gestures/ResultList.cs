using System;
using System.Collections;
using System.Collections.Generic;

namespace TemplateGesture{
	public class ResultList {

		public class BestResult : IComparable
		{
			public static BestResult Empty = new BestResult(String.Empty, -1);
			
			private string name;
			private double score;

			public void SetScore(double s){
				score = s;
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

		public void UpdateResult(string name, double score){
			for (int i = 0; i < arrList.Count; i ++) {
				if(arrList[i].Name == name){
//					if(arrList[i].Score < score){
						arrList[i].SetScore(score);
//					}
				}
			}
		}
		
		public void SortDescending()
		{
			arrList.Sort();
		}

		public string Name
		{
			get
			{
				if (arrList.Count > 0)
				{
//					BestResult r;
					double score = 0;
					string name = "";
					for(int i = 0; i < arrList.Count; i++){
						if(arrList[i].Score > score){
							score = arrList[i].Score;
							name = arrList[i].Name;
						}
					}
					return name;
				}
				return String.Empty;
			}
		}
		public double Score
		{
			get
			{
				if (arrList.Count > 0)
				{
//					BestResult r;
					double score = 0;
					for(int i = 0; i < arrList.Count; i++){
						if(arrList[i].Score > score){
							score = arrList[i].Score;
						}
					}
					return score;
				}
				return -1.0;
			}
		}

		public double GetScore(int pos){
			if (arrList.Count > pos)
			{
				BestResult r = (BestResult) arrList[pos];
				return r.Score;
			}
			return -1.0;
		}

		public string GetName(int pos){
			if (arrList.Count > pos)
			{
				BestResult r = (BestResult) arrList[pos];
				return r.Name;
			}
			return "";
		}

		public int Size(){
			return arrList.Count;
		}

		public void ClearList(){
			arrList.Clear ();
		}

	}
}
