using UnityEngine;
using System.Collections;

namespace Game{
	public enum Attribute : int{
		fire,
		water,
		wind,
		lightening,
		dirt,
		ice,
		dark,
		lighting
	}

	public class Spell /*: MonoBehaviour*/ {
		
		public string spellName;
		public Attribute attribute;
		public int atk;
		public int lvl;
		public string gesture;
		public bool IsLocked;

		private readonly float maxTime = 4.0f;
		private float liveTime;

		public Spell(){

		}
		public Spell(string name, int attr, int a, int l, string ges, bool locked = true){
			spellName = name;
			attribute = (Attribute)attr;
			atk = a;
			lvl = l;
			gesture = ges;
			IsLocked = locked;
		}

//		void Start () {
//			liveTime = 0.0f;
//		}
//		// Update is called once per frame
//		void Update () {
//			liveTime += Time.deltaTime;
//			if (liveTime > maxTime) {
////				Destroy(gameObject);
//			}
//		}

	}
}