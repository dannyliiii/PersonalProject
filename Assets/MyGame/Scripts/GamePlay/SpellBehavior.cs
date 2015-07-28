using UnityEngine;
using System.Collections;

namespace Game{
	public class SpellBehavior : MonoBehaviour {

		public Spell spell;
		private readonly float maxTime = 4.0f;
		private float liveTime;
		public bool attack = true;

		void Start () {
			liveTime = 0.0f;
		}
		// Update is called once per frame
		void Update () {

			liveTime += Time.deltaTime;
			if (liveTime > maxTime) {
				//				Destroy(gameObject);
			}
		}
	}
}