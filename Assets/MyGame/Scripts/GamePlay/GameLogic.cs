using UnityEngine;
using System.Collections;

namespace Game{
	public class GameLogic : MonoBehaviour {

		public GameObject player;
		public GameObject plane;
		public GameObject monsterPrefab;
		GameObject monster;
		
		// Use this for initialization
		void Start () {
			SpawnMonster ();

		}
		
		// Update is called once per frame
		void Update () {
			if (monster == null) {
				Debug.Log("monster is destroied");
				SpawnMonster();
			}
		}

		void SpawnMonster(){

			float height = monsterPrefab.GetComponent<MeshRenderer>().bounds.size.y;
			monster =  Instantiate(monsterPrefab, 
	                              new Vector3(plane.transform.position.x, plane.transform.position.y + height * 0.5f, plane.transform.position.z),
		                          Quaternion.FromToRotation (UnityEngine.Vector3.forward, transform.forward)
		                          ) as GameObject;

		}
	}
}
