using System.Collections;
using System;

namespace TemplateGesture{
	public struct JointPosition {
		public float x;
		public float y;
		public float z;
		public long time;
		
		public JointPosition(float a, float b, float c, long t){
			x = a;
			y = b;
			z = c;
			time = t;
		}
		
	}
}
