using UnityEngine;
using System.Collections;

namespace Game{
	public class Monster : MonoBehaviour {

		string monsterName;
		int hp;
		int atk;
		int def;
		Attribute attribute;
		public RectTransform imageTran;

		// Use this for initialization
		void Start () {
			Vector3 position = gameObject.transform.position;
			imageTran.position = new Vector3 (position.x, position.y + 1.0f * gameObject.transform.localScale.y , -1.0f);
//			imageTran.position = new Vector3 (position.x, position.y + Screen.height / 800 , -1);
		}
		
		// Update is called once per frame
		void Update () {
		
		}
	}
}