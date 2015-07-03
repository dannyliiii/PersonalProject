﻿using UnityEngine;
using System.Collections;

namespace Game{
	public class GameLogic : MonoBehaviour {

		public GameObject player;
		public GameObject plane;
		public GameObject monsterPrefab;
		GameObject monster;
		int level = 0;


		// Use this for initialization
		void Start () {
			SpawnMonster (level);

		}
		
		// Update is called once per frame
		void Update () {
			if (monster == null) {
				Debug.Log("a monster is destroied");
				SpawnMonster(level++);
			}
		}

		void SpawnMonster(int level){
			GameObject monster1 = monsterPrefab.transform.Find ("meshes").Find ("body").gameObject;
			float height = monster1.GetComponent<SkinnedMeshRenderer>().bounds.size.y;
			
//			GameObject robot2 = monsterPrefab.transform.Find("Robot2").gameObject;
//			float height = robot2.GetComponent<SkinnedMeshRenderer>().bounds.size.y;
//			float height = monsterPrefab.GetComponent<MeshRenderer>().bounds.size.y;
			monster =  Instantiate(monsterPrefab, 
	                              new Vector3(plane.transform.position.x, plane.transform.position.y, plane.transform.position.z),
		                          Quaternion.FromToRotation (-UnityEngine.Vector3.forward, -transform.forward)
		                          ) as GameObject;
			Monster monsterScript;

			monster.transform.Rotate (new Vector3 (0, 180, 0));
			monsterScript = monster.GetComponent<Monster>();
			monsterScript.ConfigMonster (level);
//			Debug.Log (monster.transform.localScale);
		}
	}
}
