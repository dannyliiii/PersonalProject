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
	
	
	public class Spell : MonoBehaviour {
		
		string spellName;
		Attribute attribute;
		int atk;
		int lvl;
		string gesture;

		private float time;

		void Start () {
			time = 0.0f;
		}
		// Update is called once per frame
		void Update () {
			time += Time.deltaTime;
			//Debug.Log (time);
			if (time > 4.0f) {
				Destroy(gameObject);
			}
		}

	}
}