using UnityEngine;
using System.Collections;

namespace Game{
	public class Monster : MonoBehaviour {

		string monsterName;
		int hp;
		int atk;
		int def;
		Attribute attribute;
		public RectTransform imageTrans;
		public RectTransform imageTransGreen;
		float hpLength = 54;
		int hpTotal = 100;

		// Use this for initialization
		void Awake () {
			Vector3 position = gameObject.transform.position;
			imageTrans.position = new Vector3 (position.x, position.y + 1.0f * gameObject.transform.localScale.y , -1.0f);
			imageTransGreen.position = new Vector3 (position.x, position.y + 1.0f * gameObject.transform.localScale.y , -1.0f);
			
//			imageTran.position = new Vector3 (position.x, position.y + Screen.height / 800 , -1);
			hp = hpTotal;
		}
		
		// Update is called once per frame
		void Update () {
		
			if (hp <= 0) {
				Destroy(gameObject);
			}
		}

		void OnCollisionEnter(Collision collision) {
//			if (collision.gameObject.transform.name == "Projectile") {
				hp -= 10;
				Vector3 posTemp = imageTransGreen.position;

				imageTransGreen.position = new Vector3 (posTemp.x - hpLength * 0.1f , posTemp.y, posTemp.z);			
//				print (imageTransGreen.position.x);
//			}
		}

	}
}