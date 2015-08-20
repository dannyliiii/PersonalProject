using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using System;
using UnityEngine.UI;

namespace Game{
	public class GameLogic : MonoBehaviour {

		public GameObject PointMan;
		public Canvas UICanvas;
		public Text lvUp;
		Color lvUpColorO;
		Color lvUpColorT;
		public GameObject player;
		Player playerScript;
		public GameObject plane;
		public GameObject monsterPrefab;
		public GameObject cursor;
		public GameObject monster;
		int level = 0;
		public GameObject rightHand;
		public GameObject rightHandScreenPos;
		public GUITexture handGUI;
		readonly string filePath = "Assets/MyGame/Configs/Save.data";
		readonly string spellFilePath = "Assets/MyGame/Configs/Spells.data";
		Vector2 lastWindowSize;
		public GameObject buttonPrefab;
		bool initilized = false;

		Transform content;
		Transform panel;
		Transform header;
		Transform contentPanel;
		RectTransform rtPanel;
		RectTransform rtHeader;
		RectTransform rtContent;
		RectTransform rtContentPanel;
		Vector3 panelOnPosition;
		Vector3 panelOffPosition;
		Vector3 contentOnPosition;
		Vector3 contentOffPosition;
		List<GameObject> buttons;
		int currentButton;

		bool UIMoveLeft = false;
		bool UIMoveRight = false;
		public bool UIOn = false;

		public GameObject canvasGO;

		public GameObject[] terrains;
		int terrain = 0;
		int terrainOld = 0;

		int monsterKillCount = 0;

		public CametaClass cc;
		// Use this for initialization
		void Start () {
			currentButton = 0;
			PointMan.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width * 0.7f, Screen.height * 0.8f, 20));
			PointMan.SetActive(false);

			playerScript = player.GetComponent<Player>() as Player;
			LoadSaveData ();
			SpawnMonster (level);
			lastWindowSize = new Vector2 (Screen.width, Screen.height);

			content = UICanvas.transform.FindChild("Content");
			panel = UICanvas.transform.FindChild("Panel");
			header = panel.FindChild("HeaderText");
			contentPanel = content.FindChild("ContentPanel");
			buttons = new List<GameObject>();

			lvUpColorO = lvUp.color;
			lvUpColorT = new Color (lvUpColorO.r, lvUpColorO.g, lvUpColorO.b, 0);

			InitializeUI();
//			UnityEngine.Debug.Log(buttons[currentButton].transform.position);
//			UnityEngine.Debug.Log (buttons [currentButton].GetComponent<Button> ().transform.position); //.Select ();
			buttons [currentButton].GetComponent<Button> ().Select();

			initilized = true;
		}
		// Update is called once per frame
		void Update () {
		
			MoveTerrain ();

			Vector2 thisWindowSize = new Vector2 (Screen.width, Screen.height);
			if (lastWindowSize != thisWindowSize) {
				lastWindowSize = thisWindowSize;
				InitializeUI();
			}

			if (monster == null && !cc.moving) {
//				UnityEngine.Debug.Log("a monster is destroied");
				SpawnMonster(++level);
//				UnityEngine.Debug.Log("xp gained");
//				UnityEngine.Debug.Log( monster.GetComponent<Monster>().xp);
				playerScript.xp += monster.GetComponent<Monster>().xp;
			}

			HitTest ();


			if(Input.GetKeyDown(KeyCode.M)){
				MoveUI(0);
			}
			if(Input.GetKeyDown(KeyCode.N)){
				MoveUI(1);
			}
			if(Input.GetKeyDown(KeyCode.UpArrow)){
				MoveFocus(0);
//				if(currentButton > 0){
//					buttons[--currentButton].GetComponent<Button>().Select();
//				}
			}
			if(Input.GetKeyDown(KeyCode.DownArrow)){
				MoveFocus(1);
				
//				if(currentButton < buttons.Count - 1){
//					buttons[++currentButton].GetComponent<Button>().Select();
//				}
//				UnityEngine.Debug.Log(currentButton);
			}

			if(Input.GetKeyDown(KeyCode.L)){

				buttons[currentButton].GetComponent<Button>().onClick.Invoke();

			}

			if (UIMoveLeft && !UIOn) {
				rtPanel.position = Vector3.Lerp(rtPanel.position, panelOnPosition, 0.1f);
				rtContent.position = Vector3.Lerp(rtContent.position, contentOnPosition, 0.1f);
				if(Mathf.Abs(rtPanel.position.x - panelOnPosition.x) < 5.0f && Mathf.Abs(rtContent.position.x - contentOnPosition.x) < 5.0f ){
					rtPanel.position = panelOnPosition;
					rtContent.position = contentOnPosition;
					UIOn = true;
					UIMoveLeft = false;
					PointMan.gameObject.SetActive(true);
					PlayAnime(playerScript.spell[currentButton].spellName);
				}
//				UnityEngine.Debug.Log("moving left");
			}
			if (UIMoveRight && UIOn) {
				rtPanel.position = Vector3.Lerp(rtPanel.position, panelOffPosition, 0.1f);
				rtContent.position = Vector3.Lerp(rtContent.position, contentOffPosition, 0.1f);
				PointMan.SetActive(false);
				if(Mathf.Abs(rtPanel.position.x - panelOffPosition.x) < 5.0f && Mathf.Abs(rtContent.position.x - contentOffPosition.x) < 5.0f ){
					rtPanel.position = panelOffPosition;
					rtContent.position = contentOffPosition;
					UIOn = false;
					UIMoveRight = false;
					currentButton = 0;
				}
//				UnityEngine.Debug.Log("moving right");	
			}

			if (lvUp.IsActive()) {
				lvUp.color = Color.Lerp (lvUp.color, lvUpColorT, 0.05f);
				if(lvUp.color.a < 0.1f){
					lvUp.gameObject.SetActive(false);
					lvUp.color = lvUpColorO;
				}
			}

			if (Input.GetKeyDown (KeyCode.Q)) {
				monster.GetComponent<Monster>().Attack();
			}

			//monsters attack
			if (monster != null) {
				if (monster.GetComponent<Monster> ().timer > 10) {
					if (!UIOn)
						monster.GetComponent<Monster> ().Attack ();

					monster.GetComponent<Monster> ().timer = 0;
				}
			}

			if (playerScript.hp <= 0) {
				GameOver();
			}

			if (Input.GetKeyDown (KeyCode.K)) {
				PlayAnime("HDEL1");
			}

		}
		
		void SpawnMonster(int level){
//			GameObject monster1 = monsterPrefab.transform.Find ("meshes").Find ("body").gameObject;
//			float height = monster1.GetComponent<SkinnedMeshRenderer>().bounds.size.y;

//			float height = monsterPrefab.GetComponent<MeshRenderer>().bounds.size.y;

//			Vector3 position = new Vector3(Camera.main.transform.position.x - Camera.main.transform.forward.x,
//			                               plane.transform.position.y,
//			                               Camera.main.transform.position.z - Camera.main.transform.position.z);

			Vector3 position = Camera.main.transform.position + Camera.main.transform.forward * 20;
//			monster =  Instantiate(monsterPrefab, 
//	                              new Vector3(plane.transform.position.x, plane.transform.position.y, plane.transform.position.z),
//		                          Quaternion.FromToRotation (-UnityEngine.Vector3.forward, -transform.forward)
//		                          ) as GameObject;
			position.y = plane.transform.position.y;
			Quaternion lookDirection = Camera.main.transform.rotation;

			monster =  Instantiate(monsterPrefab, 
			                       position,
			                       lookDirection) as GameObject;
			Monster monsterScript;

			monster.transform.Rotate (new Vector3 (0, 180, 0));
			monsterScript = monster.GetComponent<Monster>();
			monsterScript.ConfigMonster (level);
		}

		void HitTest() {

			GameObject[] go = GameObject.FindGameObjectsWithTag("Dimond");
			List<int> removeList = new List<int> ();
			List<GameObject> gol = new List<GameObject> ();
			int i = 0;
			foreach(var d in go){
				gol.Add(d);
				Vector3 pos = Camera.main.WorldToScreenPoint( d.transform.position );
				if(handGUI.HitTest (pos)){
					removeList.Add(i);
				}
				i++;
			}
			if (i > 0) {
				foreach (int r in removeList) {
					playerScript.diamond++;
					Destroy(go[r]);
				}
			}
		}

		bool LoadSaveData(){

			XmlTextReader spellReader = null;
			
			spellReader = new XmlTextReader(spellFilePath);
			spellReader.WhitespaceHandling = WhitespaceHandling.None;
			spellReader.MoveToContent();
			
			if (spellReader != null) {
				while (spellReader.Read()) {
					if(spellReader.LocalName == "Spell"){
						int level = XmlConvert.ToInt32(spellReader.GetAttribute("Level"));
						int damage = XmlConvert.ToInt32(spellReader.GetAttribute("Damage"));
						int attribute = XmlConvert.ToInt32(spellReader.GetAttribute("Attribute"));
						string gesture = spellReader.GetAttribute("Gesture");
						string name = spellReader.GetAttribute("Name");
						bool Islock = XmlConvert.ToBoolean(spellReader.GetAttribute("Lock"));
						int num = XmlConvert.ToInt32(spellReader.GetAttribute("Number"));
						playerScript.spell.Add(new Spell(name, attribute, damage, level, gesture, num, Islock));
					}
				}
			}
			spellReader.Close();

			XmlReader reader = new XmlTextReader(filePath);
//			reader.WhitespaceHandling = WhitespaceHandling.None;
			reader.MoveToContent();
			bool success = true;
			System.Diagnostics.Debug.Assert(reader.LocalName == "Save");
			try{
				while(reader.Read()){
					if(reader.LocalName == "SceneLevel"){
						string s = reader.ReadInnerXml();
						bool res = int.TryParse(s.Trim(), out level);
						if(!res){
							UnityEngine.Debug.Log("SceneLevel load failed");	
						}
					}
					if(reader.LocalName == "PlayerLevel"){
						string s = reader.ReadInnerXml();
						bool res = int.TryParse(s.Trim(), out playerScript.lv);
						if(!res){
							UnityEngine.Debug.Log("PlayerLevel load failed");	
						}
					}
					if(reader.LocalName == "Diamond"){
						string s = reader.ReadInnerXml();
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
			//wrtie general data
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

			//write spell data
			XmlDocument xmlDocSpell = new XmlDocument ();
			xmlDocSpell.Load(spellFilePath);
			XmlNode rootSpell = xmlDocSpell.DocumentElement;
			XmlNodeList xnl = rootSpell.ChildNodes;

			for(int i = 0; i < xnl.Count; i ++){
				XmlAttributeCollection xac = xnl.Item(i).Attributes;
				xac.GetNamedItem("Level").Value = playerScript.spell[i].lvl.ToString();
				xac.GetNamedItem("Damage").Value = playerScript.spell[i].atk.ToString();
			}
			xmlDocSpell.Save(spellFilePath);
		}

		void InitializeUI(){

			rtPanel =  panel.gameObject.GetComponent<RectTransform>();
			rtPanel.sizeDelta = new Vector2(Screen.width * 0.2f, Screen.height);

			rtHeader = header.GetComponent<RectTransform> ();
			rtHeader.sizeDelta = new Vector2 (rtPanel.sizeDelta.x, rtPanel.sizeDelta.y * 0.1f);
			Text headerText = header.gameObject.GetComponent<Text> ();
			headerText.fontSize = (int)(Screen.width * 0.02f);

			rtContent = content.gameObject.GetComponent<RectTransform> ();
			rtContent.sizeDelta = new Vector2 (rtPanel.sizeDelta.x, rtPanel.sizeDelta.y * 0.9f);
			rtContent.position = new Vector3(rtPanel.position.x, rtPanel.position.y - rtHeader.sizeDelta.y * 0.5f, 1);

//			UnityEngine.Debug.Log (rtContent.sizeDelta.x * 0.95f);
			rtContentPanel = contentPanel.gameObject.GetComponent<RectTransform> ();
			rtContentPanel.sizeDelta = new Vector2 (rtContent.sizeDelta.x * 0.95f, rtContent.sizeDelta.y);
//			UnityEngine.Debug.Log (rtContentPanel.sizeDelta.x);
//			rtContentPanel.position = new Vector3(rtPanel.position.x, rtPanel.position.y - rtHeader.sizeDelta.y * 0.5f, 1);

			if (!initilized) {
//				UnityEngine.Debug.Log(playerScript.spell.Count);
//				UnityEngine.Debug.Log(playerScript.diamond);
//				UnityEngine.Debug.Log(playerScript.lv);
//				UICanvas.gameObject.SetActive(true);
				foreach (var s in playerScript.spell){
					GameObject button = Instantiate (buttonPrefab) as GameObject;
					button.transform.SetParent (contentPanel);
					Text[] btnText = button.GetComponentsInChildren<Text>();
					btnText[0].text = s.spellName;
					btnText[1].text = s.lvl.ToString();
					btnText[2].text = s.atk.ToString();
					button.GetComponent<Button>().onClick.AddListener(delegate() { UpgradeSpell(currentButton); });
//					LayoutElement leButton = button.GetComponent<LayoutElement> ();
//					leButton.minWidth = rtContent.sizeDelta.x;
//					leButton.minHeight = rtContent.sizeDelta.y * 0.1f;
					buttons.Add(button);
				}
			}
			panelOnPosition = rtPanel.position;
			contentOnPosition = rtContent.position;
			rtPanel.position += new Vector3 (rtPanel.sizeDelta.x, 0, 0);
			rtContent.position += new Vector3 (rtContent.sizeDelta.x, 0, 0);
			panelOffPosition = rtPanel.position;
			contentOffPosition = rtContent.position;

		}

		public void MoveUI(int direction){

//			UnityEngine.Debug.Log("moving UI");
			//direction: 0.left, 1.right
			if (direction == 0 && !UIOn) {
				UIMoveLeft = true;
			}
			if (direction == 1 && UIOn) {
				UIMoveRight = true;
			}
		}

		public void UpgradeSpell(int currentButton){
//			UnityEngine.Debug.Log(currentButton);
			playerScript.diamond -= playerScript.spell[currentButton].lvl;

			playerScript.UpgradeSpell(currentButton);
			Text[] btnText = buttons[currentButton].GetComponentsInChildren<Text>();
			btnText[1].text = playerScript.spell[currentButton].lvl.ToString();
			btnText[2].text = playerScript.spell[currentButton].atk.ToString();

			lvUp.rectTransform.position = buttons [currentButton].transform.position - new Vector3 (rtContent.sizeDelta.x * 0.5f + lvUp.rectTransform.sizeDelta.x * 0.5f, 0, 0 );
			lvUp.gameObject.SetActive(true);
		}

		public void ButtonClick(){
//			UnityEngine.Debug.Log("button clicked");
//			UnityEngine.Debug.Log(currentButton);
			if (currentButton >= 0 && currentButton < buttons.Count) {
				buttons [currentButton].GetComponent<Button> ().onClick.Invoke ();
//				UnityEngine.Debug.Log("button does been clicked");
			}
		}

		public void MoveFocus(int direction){
			//direction: 0.up, 1.down
			if(direction == 0){
				if(currentButton > 0){
					buttons[--currentButton].GetComponent<Button>().Select();
				}
			}
			else if(direction == 1){
				if(currentButton < buttons.Count - 1){
					buttons[++currentButton].GetComponent<Button>().Select();
				}
			}
			else{
				UnityEngine.Debug.Log("Unknown diereciton");
			}

			PlayAnime (playerScript.spell [currentButton].spellName);
		}

		public void GameOver(){
			player.transform.Find ("HealthBar").gameObject.SetActive (false);
			canvasGO.SetActive (true);
			Time.timeScale =0;

		}

		public void Restart(){
			SpawnMonster (1);
			player.transform.Find ("HealthBar").gameObject.SetActive (true);
			playerScript.hp = 100;
			canvasGO.SetActive (false);
			Time.timeScale =1;
		}

		public void Quit(){
			Application.Quit();
		}

		void MoveTerrain(){

			if (player.transform.position.z >= terrains [terrain].transform.position.z + 500) {
				if(terrain == 0)
					terrain = 1;
				else
					terrain = 0;
			} else {
				return;
			}

			if (terrain != terrainOld) {
				int t = terrain;
				if (t == 0) {
					terrains [t + 1].transform.position = terrains [t].transform.position + new Vector3 (0, 0, 500);
				} else {
					terrains [t - 1].transform.position = terrains [t].transform.position + new Vector3 (0, 0, 500);
				}
				terrainOld = terrain;
			}else{
				return;
			}
		}

		public void MoveCamera(){
			cc.rotate = true;
			cc.moveForward = true;
			cc.moving = true;
			if (GameObject.Find ("KinectPrefab").GetComponent<Detector> () != null) {
				GameObject.Find ("KinectPrefab").GetComponent<Detector> ().templateGestureDetector.ClearData ();
			}
		}

		public void MonsterKilled(){
			monsterKillCount ++;
			if (monsterKillCount >= 5) {
				MoveCamera();
				monsterKillCount = 0;
			}
		}

		public void PlayAnime(string ges){

			GameObject.Find ("ShoulderRight").transform.rotation = Quaternion.identity;
			GameObject.Find ("ShoulderLeft").transform.rotation = Quaternion.identity;
			GameObject.Find ("ElbowRight").transform.rotation = Quaternion.identity;
			GameObject.Find ("ElbowLeft").transform.rotation = Quaternion.identity;
			PointMan.GetComponent<PointMan> ().Play (ges);
		}
	}
}
