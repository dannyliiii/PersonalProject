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
		float animeTime = 0;

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

			RuntimeAnimatorController ac = animator.runtimeAnimatorController;    //Get Animator controller
			for(int i = 0; i< ac.animationClips.Length; i++)                 //For all animations
			{
				if(ac.animationClips[i].name == "SimpleDie")        //If it has the same name as your clip
				{
					animeTime = ac.animationClips[i].length;
				}
			}

		}
		
		// Update is called once per frame
		void Update () {

			
		}

		IEnumerator PlayAnimeAndWait(float time){
//			Debug.Log("in play and wait");
			animator.SetBool("die",true);

			yield return new WaitForSeconds(time);

			Destroy(gameObject);
		}

		void OnCollisionEnter(Collision collision) {

//			if (collision.gameObject.transform.name == "Projectile") {
			SpellBehavior spellBehavior  = collision.gameObject.GetComponent("SpellBehavior") as SpellBehavior;
			int hpTemp = hp;
			hp -= spellBehavior.spell.atk;
			Vector2 posTemp = imageTransGreen.anchoredPosition;

//			Debug.Log (hpLength * 0.1f);
//				imageTransGreen.position = new Vector3 (posTemp.x - hpLength * 0.1f , posTemp.y, posTemp.z);	
			imageTransGreen.anchoredPosition = new Vector2 (posTemp.x - hpLength * (1 - (float)hp / (float)hpTemp), posTemp.y);			
			
//				print (imageTransGreen.position.x);
//			}
			Destroy (collision.gameObject);
			if (hp <= 0) {
				//				Reset();
				
				StartCoroutine(PlayAnimeAndWait(animeTime));
				
			}

		}

		public void Reset(){
			hp = 100;
			imageTransGreen.position = imageTrans.position;
		}

	}
}