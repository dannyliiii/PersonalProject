using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour {

	int progress = 0;
	public GameObject text;
	public GameObject BackgroundTexture;
	public GameObject texture;
	public GameObject plane;
	// Use this for initialization
	void Start () {
//		gameObject.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
	
		if (Input.GetKeyDown (KeyCode.L)) {
			StartCoroutine(ShowLoadingScreen("KinectSample"));
		}

	}

	IEnumerator ShowLoadingScreen(string scene){
		text.SetActive (true);
		BackgroundTexture.SetActive (true);
		plane.SetActive (false);
		AsyncOperation ao = Application.LoadLevelAsync (scene);
		while (!ao.isDone) {
			progress = (int)ao.progress * 100;
//			Debug.Log(progress);
			text.GetComponent<Text>().text = "Loading...." + progress + " %";
			yield return null;
		}
	}
}
