using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Kinect;

namespace TemplateGesture{
	
	public enum Constrain : int{
		up,
		down,
		left,
		right,
		count
	}
	public enum ConstrainPosition : int{
		rightHand_head,
		leftHand_head,
		count
	}

	public class GestureConstrains {

//		public static readonly int cSize = 2;
		
		public static Constrain[] GetConstrains(List<Vector4> list){

			Constrain[] res = new Constrain[(int)ConstrainPosition.count];

			//righthand - head
			if(list[(int)NuiSkeletonPositionIndex.HandRight].y < list[(int)NuiSkeletonPositionIndex.Head].y){
				res[0] = Constrain.down;
			}else{
				res[0] = Constrain.up;
			}

			//lefthand - head
			if(list[(int)NuiSkeletonPositionIndex.HandLeft].y < list[(int)NuiSkeletonPositionIndex.Head].y){
				res[1] = Constrain.down;
			}else{
				res[1] = Constrain.up;
			}

			return res;
		}

		public static bool MeetConstrains(Constrain[] c1, List<Constrain> c2){

			bool res = true;

//			Debug.Log("======c1========");
//			foreach (var c in c1) {
//				Debug.Log(c);
//			}
//			Debug.Log("======c2========");		
//			foreach (var c in c2) {
//				Debug.Log(c);
//			}

			for (int i = 0; i < (int)ConstrainPosition.count; i ++) {
				if(!((int)c1[i] == (int)c2[i])){
					res = false;
					break;
				}
			}

			return res;
		}

	}
}