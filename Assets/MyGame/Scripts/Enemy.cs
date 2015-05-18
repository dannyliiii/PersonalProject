using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

	private int hp;
	private int atk;
	private int def;

	// Use this for initialization
	void Start () {
		hp = 5;
	}
	
	// Update is called once per frame
	void Update () {
		Dead ();
	}

	virtual public void Dead(){
		if (hp <= 0) {
			Destroy(gameObject);
		}
	}

	virtual public void OnHit(){
		hp--;
	}

	void OnCollisionEnter(Collision collision) {
		if (collision.collider.gameObject.name != "Plane") {
			Destroy (collision.gameObject);
			OnHit ();
		}
	}
}
