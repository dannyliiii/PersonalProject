using UnityEngine;
using System.Collections;

namespace Game{
	public class Monster : MonoBehaviour {

		string monsterName;
		int hp;
		public int atk { get; set;}
		int def;
		Attribute attribute;
		public RectTransform imageTrans;
		public RectTransform imageTransGreen;
		float hpLength;
		int hpTotal = 100;
		float height; 
		int count = 0;
		public Animator animator;

		// Use this for initialization
		void Awake () {
			Vector3 position = gameObject.transform.position;
			height = GetComponent<MeshRenderer>().bounds.size.y;
			imageTrans.position = new Vector3 (position.x, position.y + height * 0.6f , -1.0f);
//			imageTransGreen.position = new Vector3 (position.x, position.y + 1.0f * gameObject.transform.localScale.y , -1.0f);
//			imageTran.position = new Vector3 (position.x, position.y + Screen.height / 800 , -1);
			hp = hpTotal;
			hpLength = imageTrans.rect.width;
			animator.SetBool("die",false);
		
		}
		
		// Update is called once per frame
		void Update () {
			if (hp <= 0) {
//				Reset();
				animator.SetBool("die",true);
			}

		}

		void OnCollisionEnter(Collision collision) {
//			if (collision.gameObject.transform.name == "Projectile") {
			Destroy (collision.gameObject);
			hp -= 10;
			Vector2 posTemp = imageTransGreen.anchoredPosition;

//			Debug.Log (hpLength * 0.1f);
//				imageTransGreen.position = new Vector3 (posTemp.x - hpLength * 0.1f , posTemp.y, posTemp.z);	
			imageTransGreen.anchoredPosition = new Vector2 (posTemp.x - hpLength * 0.1f, posTemp.y);			
			
//				print (imageTransGreen.position.x);
//			}
		}

		public void Reset(){
			hp = 100;
			imageTransGreen.position = imageTrans.position;
		}

	}
}