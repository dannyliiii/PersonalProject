using UnityEngine;
using System.Collections;

public class PointMan : MonoBehaviour {

//	public Animation anime;
	Animation anime;
	// Use this for initialization
	void Start () {
		anime = gameObject.GetComponent<Animation> ();

	}
	
	// Update is called once per frame
	void Update () {

	}

	public void Play(string ges){
//		Debug.Log (ges);
		string s = ges.ToUpper ();
		AnimationClip ac;
		ac = anime.GetClip (s);
		if (ac != null) {
			anime.Play (s);
		}
//		anime.Play();
	}
}
