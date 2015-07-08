using UnityEngine;
using System.Collections;
using System;

namespace Game{
	public class Dimond : MonoBehaviour {
		Player player;
		Vector2 screenPosition;
		// Use this for initialization
		void Start () {
			player = GameObject.Find("Player").GetComponent<Player>() as Player;

			Vector2 screenPosition = Camera.main.WorldToScreenPoint(transform.position);
		}
		
		// Update is called once per frame
		void Update () {
			Vector2 screenPosition = Camera.main.WorldToScreenPoint(transform.position);
			Debug.Log (screenPosition);
		}

		void OnTriggerEnter(Collider other ){
	//		Debug.Log (other.gameObject.name);
			if (other.gameObject.name == "RightHand" || other.gameObject.name == "LeftHand") {
	//			Debug.Log("dimond collision if");
				player.dimond ++;
				Debug.Log("Dimond ++");
				Destroy(gameObject);
			}
		}
	}
}
