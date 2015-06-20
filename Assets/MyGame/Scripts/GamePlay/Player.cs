using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Game{
	public class Player : MonoBehaviour {

		int lv;
		int xp;
		int hp;
		int mp;
		int atk;
		int def;
		int money;
		List<Spell> spells;
	
		public GameObject projectile;
		private List<GameObject> projList;
		private UnityEngine.Vector3 upForce;
		private readonly int speed = 50;

		void Start(){
			lv = 1;
			xp = 0;
			hp = 100;
			mp = 100;
			atk = 1;
			def = 1;
			money = 0;
			spells = new List<Spell>();
			
			projList = new List<GameObject> ();
			upForce = new UnityEngine.Vector3 (0.0f, 100.0f, 0.0f);

			gameObject.transform.position = Camera.main.transform.position;
		}

		void Update(){
			//temp
			if (Input.GetKeyDown (KeyCode.A)) {
				CastSpell();
			}
		}

		public void CastSpell(){

			GameObject proj =  Instantiate(projectile, transform.position, Quaternion.FromToRotation (UnityEngine.Vector3.forward, transform.forward)) as GameObject;
			Rigidbody rb = proj.GetComponent<Rigidbody> ();
			rb.velocity = transform.forward * speed;
			rb.AddForce (upForce);
			ParticleSystem ps = proj.GetComponent<ParticleSystem>();
			ps.Play ();
			projList.Add (proj);

		}

		public void Dead(){

		}

	}
}