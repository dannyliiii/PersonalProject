using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TemplateGesture{
	public static class DynamicTimeWraping{
		public static bool boundryConstrainStart = true;
		public static bool boundryConstrainEnd = true;
		public static DTWVariables[] variables;


		public static void Initialize(DTWVariables[] vars, bool start, bool end){
			boundryConstrainEnd = end;
			boundryConstrainStart = start;
			variables = vars;
		}
		
	}
}