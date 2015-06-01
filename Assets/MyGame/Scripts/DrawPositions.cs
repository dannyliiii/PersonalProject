using UnityEngine;
using System.Collections;
using Drawing;
using MyMath;
using System.Collections.Generic;

public class DrawPositions : MonoBehaviour {


	public GameObject[] dips = new GameObject[2]; 

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	//added by danny
	//Draw list of vector
	public void DrawProcessedHandsPosistions(List<MyMath.Vector2> l, int p){

		Material materia = dips[p].GetComponent<Renderer>().material;
		Texture2D texture = new Texture2D(512,512, TextureFormat.RGB24, false);
		texture.wrapMode = TextureWrapMode.Clamp;
		materia.SetTexture(0, texture);

		int num = l.Count;

		for (int i = 0; i < num; i ++) {

			texture.DrawLine(new UnityEngine.Vector2((l[i].x + 1) * 256, (l[i].y  + 1) * 256),
			                 new UnityEngine.Vector2((l[i+1].x + 1) * 256, (l[i+1].y  + 1) * 256),
			                 Color.blue);

		}
		texture.Apply();
	}


}
