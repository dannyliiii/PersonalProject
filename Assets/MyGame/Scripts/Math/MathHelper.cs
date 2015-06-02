using System.Collections;
using UnityEngine;

namespace MyMath{
	public static class MathHelper
	{
		public const float PiOver4 = (float)Mathf.PI / 4.0f;
		public const float PiOver2 = (float)Mathf.PI / 2.0f;

		public static Vector2 NormalizeVector2D(Vector2 v){
			float l = Length (v);
			return new Vector2 (v.x / l, v.y / l);
		}

		public static float Length(Vector2 v){
			return Mathf.Sqrt (v.x * v.x + v.y * v.y);
		}

		public static float Sqrt(float f){
			return Mathf.Sqrt (f);
		}
}
}

