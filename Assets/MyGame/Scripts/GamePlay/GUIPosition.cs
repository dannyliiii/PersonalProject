using UnityEngine;
using System.Collections;

public class GUIPosition : MonoBehaviour {

	public GUIText text;
	public GUITexture texture;
	public GUIText plusOne;
	// Use this for initialization
	void Start () {
		texture.transform.position = new Vector2 (0.1f, 0.1f);

		text.transform.position = (Vector2)texture.transform.position + new Vector2 (0.02f, 0.02f);
//		text.transform.position = texture.transform.position  + new Vector2 (texture.s, Screen.height / 10.0f);
		plusOne.transform.position = (Vector2)text.transform.position + new Vector2 (0, 0.05f);
		plusOne.color = new Color (0.0f, 0.0f, 0.0f, 0);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
