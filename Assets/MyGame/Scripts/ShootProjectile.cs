using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShootProjectile : MonoBehaviour {

	public GameObject projectile;
	private Vector3 upForce;
	private readonly int speed = 50;
	private List<GameObject> projList;
	//private GestureListener gestureListener;

	// Use this for initialization
	void Awake () {
		projList = new List<GameObject> ();
		upForce = new Vector3 (0.0f, 150.0f, 0.0f);
		//gestureListener = Camera.main.GetComponent<GestureListener>();
	}
	
	// Update is called once per frame
	void Update () {
//		if (gestureListener.IsSwipeLeft()) {
//			Debug.Log("Swip left.");
//			Shoot();		
//		}
//		if (gestureListener.IsTrapezeEasyLevel2 ()) {
//			Debug.Log("trapezeEasyLevel2");
//			Shoot();
//		}
	}

	public void Shoot(){

		GameObject proj =  Instantiate(projectile, transform.position, Quaternion.FromToRotation (Vector3.forward, transform.forward)) as GameObject;
		Rigidbody rb = proj.GetComponent<Rigidbody> ();
		rb.velocity = transform.forward * speed;
		rb.AddForce (upForce);
		ParticleSystem ps = proj.GetComponent<ParticleSystem>();
		ps.Play ();
		projList.Add (proj);
	}
}
