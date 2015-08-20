using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Game{
	public class Monster : MonoBehaviour {

		string monsterName;
		int hp;
		public int atk { get; set;}
		int def;
		public int xp;
		Attribute attribute;
		public RectTransform imageTrans;
		public RectTransform imageTransGreen;
		float hpLength;
		int hpTotal = 100;
		float height; 
//		int count = 0;
		public Animator animator;
		float animeTime = 0;
		Animation anime;
		float respawnDelay = 0.5f;
		public GameObject dimond;
		float speed = 8.0f;
		public Text healthText;
		Vector2 oldPos;
		Vector2 newPos;
		int oldHp;
		int newHp;
		bool flag = true;

		public GameObject spell;
		public float timer = 0;
		// Use this for initialization
		void Awake () {

//			RuntimeAnimatorController ac = animator.runtimeAnimatorController;    //Get Animator controller
//			for(int i = 0; i< ac.animationClips.Length; i++)                 //For all animations
//			{
//				if(ac.animationClips[i].name == "SimpleDie")        //If it has the same name as your clip
//				{
//					animeTime = ac.animationClips[i].length;
//				}
//			}
			oldHp = newHp = hp = hpTotal = 100;
			healthText.text = hp.ToString() + "/" + hpTotal;
			hpLength = imageTrans.rect.width;
			anime = GetComponent<Animation> ();
			anime ["monster1Idle"].wrapMode = WrapMode.Loop;
			anime.Play("monster1Idle");
			
			oldPos = imageTransGreen.anchoredPosition;
			animeTime = anime.GetClip("monster1Die").length;
	
		}
		
		// Update is called once per frame
		void Update () {

			imageTransGreen.anchoredPosition = Vector2.Lerp (oldPos, newPos, 0.1f);
			oldPos = imageTransGreen.anchoredPosition;
			oldHp = (int)Mathf.Lerp (oldHp, newHp, 0.1f);
			healthText.text = oldHp.ToString () + "/" + hpTotal;
//			Debug.Log("==========");
//			Debug.Log (oldHp);
//			Debug.Log (newHp);

			if (oldHp <= 0 && flag) {
				flag = false;
				Destroy (gameObject.GetComponent<CapsuleCollider> ());
				
				GameObject dim = Instantiate (dimond, transform.position, Quaternion.FromToRotation (UnityEngine.Vector3.forward, transform.forward)) as GameObject;
				Rigidbody rb = dim.GetComponent<Rigidbody> ();
				Vector3 front = Camera.main.transform.forward;
//				rb.velocity = (transform.up + new Vector3 (Random.Range (-1.0f, 1.0f), 0, -0.3f)) * speed;
//				rb.velocity = (transform.up + front *(Random.Range (-1.0f, 1.0f)) + new Vector3(0,0,-0.3f)) * speed;
				rb.velocity = (transform.up + new Vector3 (Random.Range (-1.0f, 1.0f), 0, Random.Range (-1.0f, 1.0f))) * speed;		
				
				anime.Play ("monster1Die");
				StartCoroutine (PlayAnimeAndWait (animeTime));
			}

			if (!Camera.main.GetComponent<GameLogic> ().UIOn) {
				timer += Time.deltaTime;
			}

			if (timer > 5 && newHp > 0) {
				Attack ();
				timer = 0;
			} else {
				timer = 0;
			}

		}

		void LateUpdate(){

		}

		public void ConfigMonster(int level){
			Vector3 position = gameObject.transform.position;

			GameObject monster1 = transform.Find ("meshes").Find ("body").gameObject;
			float height = monster1.GetComponent<SkinnedMeshRenderer>().bounds.size.y;

//			GameObject robot2 = transform.Find("Robot2").gameObject;
//			height = robot2.GetComponent<SkinnedMeshRenderer>().bounds.size.y;

//			height = GetComponent<MeshRenderer>().bounds.size.y;
			imageTrans.position = new Vector3 (position.x, position.y + height , position.z);
			//			imageTransGreen.position = new Vector3 (position.x, position.y + 1.0f * gameObject.transform.localScale.y , -1.0f);
			//			imageTran.position = new Vector3 (position.x, position.y + Screen.height / 800 , -1);
			hpTotal = 100 + level * 50;
			newHp = oldHp = hp = hpTotal;
			hpLength = imageTrans.rect.width;
			oldPos = imageTransGreen.anchoredPosition;
			xp = hp;
//			Debug.Log (level);
//			RectTransform rt = gameObject.GetComponent<RectTransform> ();
//			Debug.Log (rt.localScale);
//			Vector3 scale = rt.localScale + new Vector3((float)level, (float)level, (float)level );
//			rt.localScale = scale;
//			Debug.Log (rt.localScale);

//			Color randColor = new Color(Random.Range(0.0f,1.0f), Random.Range(0.0f,1.0f), Random.Range(0.0f,1.0f), 1.0f);
//			gameObject.GetComponent<MeshRenderer> ().material.color = randColor;
//			Debug.Log (gameObject.GetComponent<MeshRenderer> ().material.color);
			
		}

		IEnumerator PlayAnimeAndWait(float time){
//			Debug.Log("in play and wait");
//			animator.SetBool("die",true);

			yield return new WaitForSeconds(time + respawnDelay);

			Destroy(gameObject);

		}

		void OnTriggerEnter(Collider collision) {

			SpellBehavior spellBehavior = null;
			spellBehavior = collision.gameObject.GetComponent("SpellBehavior") as SpellBehavior;
			if (spellBehavior == null)
				return;

			if (!spellBehavior.attack) {
				newHp -= spellBehavior.spell.atk;

				if (newHp < 0)
					newHp = 0;

				oldPos = imageTransGreen.anchoredPosition;
				newPos = new Vector2 (oldPos.x - hpLength * ((float)spellBehavior.spell.atk / (float)hpTotal), oldPos.y);	

				anime.Play ("monster1Hit2");

				Destroy (collision.gameObject);

			} else {
			
			}
				
		}

		public void Reset(){
			hp = 100;
			imageTransGreen.position = imageTrans.position;
		}

		public void Attack(){

			if (anime.isPlaying) {
				anime.Stop();
			}
			anime.Play ("monster1Attack1");

			GameObject monster1 = transform.Find ("meshes").Find ("body").gameObject;
			float height = monster1.GetComponent<SkinnedMeshRenderer>().bounds.size.y;

			Vector3 position = transform.position + new Vector3(0, height, 0);
			Vector3 toPosition = Camera.main.transform.position;
			GameObject proj =  Instantiate(/*projectile*/ spell, position, Quaternion.FromToRotation (UnityEngine.Vector3.forward, transform.forward)) as GameObject;
			SpellBehavior spellBehavior = proj.GetComponent("SpellBehavior") as SpellBehavior;
			spellBehavior.attack = true;

			Rigidbody rb = proj.GetComponent<Rigidbody> ();
			Vector3 velDir = (toPosition - position);
			rb.velocity = velDir;
//			rb.AddForce (new Vector3(0, -100f, 0));
			ParticleSystem ps = proj.GetComponent<ParticleSystem>();
			ps.Play ();


		}

		void OnDestroy(){
			if (Camera.main != null) {
//				Camera.main.GetComponent<GameLogic> ().MoveCamera ();
				Camera.main.GetComponent<GameLogic> ().MonsterKilled ();
			}
		}

	}
}