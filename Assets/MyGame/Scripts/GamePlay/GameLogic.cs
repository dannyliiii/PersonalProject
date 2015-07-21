using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using System;

namespace Game{
	public class GameLogic : MonoBehaviour {
		public Canvas UICanvas;

		public GameObject player;
		Player playerScript;
		public GameObject plane;
		public GameObject monsterPrefab;
		public GameObject cursor;
		GameObject monster;
		int level = 0;
		public GameObject rightHand;
		public GameObject rightHandScreenPos;
		public GUITexture handGUI;
		string filePath = "Assets/MyGame/Configs/Save.data";
		// Use this for initialization
		void Start () {

			playerScript = player.GetComponent<Player>() as Player;
			LoadSaveData ();
			SpawnMonster (level);

			InitializeUI();


		}
		
		// Update is called once per frame
		void Update () {
			if (monster == null) {
				UnityEngine.Debug.Log("a monster is destroied");
				SpawnMonster(++level);
			}


			HitTest ();

//			Debug.Log ("Right hand");
//			UnityEngine.Vector2 screenPosition = Camera.main.WorldToScreenPoint(rightHand.transform.position);
//			rightHandScreenPos.transform.position = screenPosition;
//			Debug.Log (screenPosition);

		}
		
		void SpawnMonster(int level){
//			GameObject monster1 = monsterPrefab.transform.Find ("meshes").Find ("body").gameObject;
//			float height = monster1.GetComponent<SkinnedMeshRenderer>().bounds.size.y;
			
//			GameObject robot2 = monsterPrefab.transform.Find("Robot2").gameObject;
//			float height = robot2.GetComponent<SkinnedMeshRenderer>().bounds.size.y;
//			float height = monsterPrefab.GetComponent<MeshRenderer>().bounds.size.y;

			monster =  Instantiate(monsterPrefab, 
	                              new Vector3(plane.transform.position.x, plane.transform.position.y, plane.transform.position.z),
		                          Quaternion.FromToRotation (-UnityEngine.Vector3.forward, -transform.forward)
		                          ) as GameObject;
			Monster monsterScript;

			monster.transform.Rotate (new Vector3 (0, 180, 0));
			monsterScript = monster.GetComponent<Monster>();
			monsterScript.ConfigMonster (level);
//			Debug.Log (monster.transform.localScale);
		}

		void HitTest() {
//			bool res = false;

			GameObject[] go = GameObject.FindGameObjectsWithTag("Dimond");
			List<int> removeList = new List<int> ();
			List<GameObject> gol = new List<GameObject> ();
			int i = 0;
			foreach(var d in go){
				gol.Add(d);
				Vector3 pos = Camera.main.WorldToScreenPoint( d.transform.position );
				if(handGUI.HitTest (pos)){
					UnityEngine.Debug.Log("hit");
					removeList.Add(i);
				}
				i++;
			}
			if (i > 0) {
				foreach (int r in removeList) {
					UnityEngine.Debug.Log (r);
					playerScript.diamond++;
					Destroy(go[r]);
				}
			}
		}

		bool LoadSaveData(){
			XmlReader reader = new XmlTextReader(filePath);
//			reader.WhitespaceHandling = WhitespaceHandling.None;
			reader.MoveToContent();
			bool success = true;
			System.Diagnostics.Debug.Assert(reader.LocalName == "Save");
			try{
				while(reader.Read()){
					if(reader.LocalName == "SceneLevel"){
						string s = reader.ReadInnerXml();
//						UnityEngine.Debug.Log(s);
						bool res = int.TryParse(s.Trim(), out level);
						if(!res){
							UnityEngine.Debug.Log("SceneLevel load failed");	
						}
					}
					if(reader.LocalName == "PlayerLevel"){
						string s = reader.ReadInnerXml();
//						UnityEngine.Debug.Log(s);
						bool res = int.TryParse(s.Trim(), out playerScript.lv);
						if(!res){
							UnityEngine.Debug.Log("PlayerLevel load failed");	
						}
					}
					if(reader.LocalName == "Diamond"){
						string s = reader.ReadInnerXml();
//						UnityEngine.Debug.Log(s);
						bool res = int.TryParse(s.Trim(), out playerScript.diamond);
						playerScript.diamondOld = playerScript.diamond;
						if(!res){
							UnityEngine.Debug.Log("PlayerLevel load failed");	
						}
					}
				}
			}
			catch (XmlException xex)
			{
				UnityEngine.Debug.Log(xex.Message);
				success = false;
			}
			finally
			{
				if (reader != null)
					reader.Close();
			}
			return success;
		}

		void OnDestroy() {
			XmlDocument xmlDoc = new XmlDocument ();
			xmlDoc.Load(filePath);
			XmlNode root = xmlDoc.DocumentElement;
			
			XmlNode n1 = root.SelectSingleNode("SceneLevel");
			n1.InnerText = level.ToString();
			XmlNode n2 = root.SelectSingleNode("PlayerLevel");
			n2.InnerText = playerScript.lv.ToString();
			XmlNode n3 = root.SelectSingleNode("Diamond");
			n3.InnerText = playerScript.diamond.ToString();
			
			xmlDoc.Save(filePath);
		}

		void InitializeUI(){
			Transform content = UICanvas.transform.FindChild("Content");
			Transform panel = UICanvas.transform.FindChild("Panel");

			RectTransform rtp =  panel.gameObject.GetComponent<RectTransform>();
			rtp.sizeDelta = new Vector2(Screen.width * 0.1f, Screen.height);

		}

	}
}
