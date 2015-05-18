using UnityEngine;
using System.Collections;

namespace MyMath{
	public class Vector3 {
		public float x;
		public float y;
		public float z;
		
		public Vector3(float a, float b, float c)
		{
			x = a;
			y = b;
			z = c;
		}
		
		public float Length
		{
			get
			{
				return Mathf.Sqrt(x * x + y * y + z * z);
			}
		}
		
		public static float Dot(Vector3 v1, Vector3 v2)
		{
			return v1.x * v2.x + v1.y * v2.y + v1.z * v2.z;
		}
		
		public static Vector3 operator -(Vector3 v1, Vector3 v2)
		{
			return new Vector3(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z);
		}
		
		public static Vector3 operator +(Vector3 v1, Vector3 v2)
		{
			return new Vector3(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);
		}
		
		public static Vector3 operator *(Vector3 v1, float value)
		{
			return new Vector3(v1.x * value, v1.y * value, v1.z * value);
		}
		
		public static Vector3 operator /(Vector3 v1, float value)
		{
			return new Vector3(v1.x / value, v1.y / value, v1.z / value);
		}

		public static Vector3 Zero{
			get{
				return new Vector3(0,0,0);
			}
		}

		public static Vector2 ToVector2(Vector3 v){

			return new Vector2 (v.x, v.y);
		}

	}
}
