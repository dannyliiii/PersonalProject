using UnityEngine;
using System.Collections;

namespace MyMath{
	public class Vector2 {

		public float x, y;

		public Vector2(float a, float b){
			x = a;
			y = b;
		}
		public float LengthSqr{
			get{
				return x * x + y * y;
			}
		}
		public static Vector2 operator+(Vector2 v1, Vector2 v2){
			return new Vector2(v1.x + v2.x, v1.y + v2.y);
		}
		public static Vector2 operator-(Vector2 v1, Vector2 v2){
			return new Vector2(v1.x - v2.x, v1.y - v2.y);
		}
		public static Vector2 operator*(Vector2 v, float value){
			return new Vector2(v.x * value, v.y * value);
		}
		public static Vector2 operator/(Vector2 v,float value){
			return new Vector2(v.x / value, v.y / value);
		}

		public static Vector2 Zero{
			get{
				return new Vector2(0 , 0);
			}
		}

	}
}