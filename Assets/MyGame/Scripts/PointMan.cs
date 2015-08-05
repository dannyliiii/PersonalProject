using UnityEngine;
using System.Collections;

public class PointMan : MonoBehaviour {

//	public Animation anime;
	public AnimationClip anime;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
//		anime.Play("HDEL1");
		gameObject.GetComponent<Animation>().Play();
	}
}
