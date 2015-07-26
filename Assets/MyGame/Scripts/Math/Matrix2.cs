using UnityEngine;
using System.Collections;

namespace MyMath{
	public class Matrix2  {
		public float[,] value = new float[2,2] ;

		public Matrix2(float a1, float a2, float b1, float b2){
			value[0,0] = a1;
			value[0,1] = a2;
			value[1,0] = b1;
			value[1,1] = b2;
		}

	}
}