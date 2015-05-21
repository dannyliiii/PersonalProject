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


		private ArrayList arrList;

		public ResultList(){
			arrList = new ArrayList ();
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
					BestResult r = (BestResult) arrList[0];
					return r.Name;
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
					BestResult r = (BestResult) arrList[0];
					return r.Score;
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
