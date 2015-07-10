using UnityEngine;
using System.Collections;

public class GUIPosition : MonoBehaviour {

	public GUIText text;
	public GUITexture texture;
	// Use this for initialization
	void Start () {
		texture.transform.position = new Vector2 (0.1f, 0.1f);

		text.transform.position = (Vector2)texture.transform.position + new Vector2 (0.05f, 0.01f);
//		text.transform.position = texture.transform.position  + new Vector2 (texture.s, Screen.height / 10.0f);
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
