using UnityEngine;
using System.Collections;

namespace MyMath{
	public class Matrix3  {
		public float[,] value = new float[3,3] ;
		
		public Matrix3(float a1, float a2, float a3,
		               float b1, float b2, float b3,
		               float c1, float c2, float c3){
			value[0,0] = a1;
			value[0,1] = a2;
			value[0,2] = a3;
			value[1,0] = b1;
			value[1,1] = b2;
			value[1,2] = b3;	
			value[2,0] = c1;
			value[2,1] = c2;
			value[2,2] = c3;
		}
		
	}
}