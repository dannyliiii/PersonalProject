 using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Diagnostics;

namespace Game{
	public class Player : MonoBehaviour {

		public GUIText dimondNum;
		public int lv;
		public int xp;
		int hp;
		int mp;
		int atk;
		int def;
		public int diamond;
		List<Spell> spells;
	
		public GameObject plane;
		public GameObject projectile;
		public GameObject spell2;
		public GameObject[] spellsPrefabs;
		private List<GameObject> projList;
		private UnityEngine.Vector3 upForce;
		private readonly int speed = 30;
		List<Spell> spell = new List<Spell> ();
		readonly string filePath = "Assets/MyGame/Configs/Spells.data";

		void Start(){
			lv = 1;
			xp = 0;
			hp = 100;
			mp = 100;
			atk = 1;
			def = 1;
			diamond = 0;
			spells = new List<Spell>();
			
			projList = new List<GameObject> ();
			upForce = new UnityEngine.Vector3 (0.0f, 100.0f, 0.0f);

			gameObject.transform.position = Camera.main.transform.position;

			XmlTextReader reader = null;

			reader = new XmlTextReader(filePath);
			reader.WhitespaceHandling = WhitespaceHandling.None;
			reader.MoveToContent();

			if (reader != null) {
				while (reader.Read()) {
					if(reader.LocalName == "Spell"){
						int level = XmlConvert.ToInt32(reader.GetAttribute("Level"));
						int damage = XmlConvert.ToInt32(reader.GetAttribute("Damage"));
						int attribute = XmlConvert.ToInt32(reader.GetAttribute("Attribute"));
						string gesture = reader.GetAttribute("Gesture");
						string name = reader.GetAttribute("Name");
						bool Islock = XmlConvert.ToBoolean(reader.GetAttribute("Lock"));
						int num = XmlConvert.ToInt32(reader.GetAttribute("Number"));
						spell.Add(new Spell(name, attribute, damage, level, gesture, num, Islock));
					}
				}
			}
		}
		
		void Update(){
			//temp
			if (Input.GetKeyDown (KeyCode.A)) {
//				CastSpell();
				CastSpell("hdel1");
			}
			if (Input.GetKeyDown (KeyCode.B)) {
				//				CastSpell();
				CastSpell("HD Pool Entry Dive");
			}
			if (Input.GetKeyDown (KeyCode.C)) {
				//				CastSpell();
				CastSpell("High Dive Power-Up");
			}
			if (Input.GetKeyDown (KeyCode.D)) {
				//				CastSpell();
				CastSpell("hdel3");
			}

			dimondNum.text = diamond.ToString();
		}

		public void CastSpell(){

			GameObject proj =  Instantiate(projectile, transform.position, Quaternion.FromToRotation (UnityEngine.Vector3.forward, transform.forward)) as GameObject;
			SpellBehavior spellBehavior = proj.GetComponent("SpellBehavior") as SpellBehavior;
			spellBehavior.spell = spell[Random.Range(0,spell.Count - 1)];
//			UnityEngine.Debug.Log (spellBehavior.spell.spellName);
//			spellBehavior.spell = new Spell ("test", 1, 1, 1, "hdel1", false);
			Rigidbody rb = proj.GetComponent<Rigidbody> ();
			rb.velocity = transform.forward * speed;
			rb.AddForce (upForce);
			ParticleSystem ps = proj.GetComponent<ParticleSystem>();
			ps.Play ();
			projList.Add (proj);

		}

		public void CastSpell(string gesture){

			Spell temp = null;

//			int pos = 0;

			foreach(var s in spell){
				if(s.gesture == gesture){
					temp = s;
					break;
				} 
			}
			if (temp != null && temp.IsLocked) {
				UnityEngine.Debug.Log("Spell has not been unlocked.");
				return;
			}

			Vector3 position = transform.position + new Vector3 (2.5f, 0.0f, 0.0f);
			Vector3 toPosition = new Vector3 (plane.transform.position.x, plane.transform.position.y + 2.5f/*temp*/ * 0.5f, plane.transform.position.z);
			GameObject proj =  Instantiate(/*projectile*/ spellsPrefabs[temp.num], position, Quaternion.FromToRotation (UnityEngine.Vector3.forward, transform.forward)) as GameObject;
			SpellBehavior spellBehavior = proj.GetComponent("SpellBehavior") as SpellBehavior;
			if (temp == null) {
				temp = new Spell ("test", 1, 10, 1, "", 0, false);
			}
			spellBehavior.spell = temp;
//			UnityEngine.Debug.Log (spellBehavior.spell.spellName);
			Rigidbody rb = proj.GetComponent<Rigidbody> ();
			Vector3 velDir = (toPosition - position).normalized;
			rb.velocity = velDir * speed;
			rb.AddForce (upForce);
			ParticleSystem ps = proj.GetComponent<ParticleSystem>();
			ps.Play ();
			projList.Add (proj);
		}
		
		public void Dead(){

		}

		public void CollectDimond(){

		}

	}
}