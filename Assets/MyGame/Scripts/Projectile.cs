using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

	private float time;
	// Use this for initialization
	void Start () {
		time = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
		time += Time.deltaTime;
		//Debug.Log (time);
		if (time > 4.0f) {
			Destroy(gameObject);
		}
	}
}
