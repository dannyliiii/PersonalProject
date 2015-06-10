using System.Collections;
using System;
using MyMath;
using WobbrockLib;
using System.Drawing;
using TemplateGesture;

public class Entry {
	
	public DateTime Time { get; set; }
	public Vector3 PositionRight { get; set; }
	public Vector3 PositionLeft { get; set;}
	public TimePointF TpfPosLeft{ get; set;}
	public TimePointF TpfPosRight{ get; set;}
	public TimePointF ZY_TpfPosLeft{ get; set;}
	public TimePointF ZY_TpfPosRight{ get; set;}
	public TimePointF ZX_TpfPosLeft{ get; set;}
	public TimePointF ZX_TpfPosRight{ get; set;}
	public Constrain[] constrain = new Constrain[(int)ConstrainPosition.count];

}
