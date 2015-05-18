using UnityEngine;
using System.Collections;

public class MouseEvent : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void StartGame(){
		Application.LoadLevel ("MainScene");
		
	}
	
	public void GameSetting(){
		
	}
	
	public void QuitGame(){
		Application.Quit ();
		
	}
}
