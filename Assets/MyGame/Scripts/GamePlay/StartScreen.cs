using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using Drawing;

public class StartScreen : MonoBehaviour {

	readonly int maxNumber = 100;
	Queue<Vector3> mousePositions;	
	Texture2D texture;
	RawImage[] img;
	// Use this for initialization
	void Start () {
		texture = new Texture2D(512,512, TextureFormat.RGB24, false);
		texture.wrapMode = TextureWrapMode.Clamp;

		mousePositions = new Queue<Vector3>(50);
		img = transform.Find("Canvas").gameObject.GetComponentsInChildren<RawImage>(); 
		img [0].texture = texture;

	}
	
	// Update is called once per frame
	void Update () {
		Debug.Log (Input.mousePosition);
		AddPosition2List (Input.mousePosition);
		DrawMousePositions ();
	}

	void LoadScene(string scene){
		Application.LoadLevel (scene);
	}

	void DrawMousePositions(){

		texture = new Texture2D (512, 512, TextureFormat.RGB24, false);
		foreach(var p in mousePositions){
	
			float x = p.x / Screen.width * 512;
			float y = 512 - p.y / Screen.height * 512;
				
			texture.DrawFilledCircle( (int)x,  (int)y, 3, Color.black);
		}

		texture.Apply ();
		img [0].texture = texture;

	}

	void AddPosition2List(Vector3 pos){

		if (mousePositions.Count >= maxNumber) {
			mousePositions.Dequeue();
		}
		mousePositions.Enqueue (pos);
	}
	
}
