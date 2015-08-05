using UnityEngine;
using System.Collections;
using System;

namespace Game{
	public class CametaClass : MonoBehaviour {

		public bool moveForward = false;
		Vector3 targetPos;
		bool flag = false;
		// Use this for initialization
		int section;

		public bool rotate = false;
		bool flag2 = false;
		Vector3 targetRot;
		float rotateAngle = 0;
		Vector3 temp;
		public GameObject terrain;

		public bool moving = false;

		public GameObject depthPlane;

		void Start () {
		

		}
		
		// Update is called once per frame
		void Update () {

			GetSection();

			if(Input.GetKeyDown(KeyCode.RightArrow)){
				rotate = true;
				moveForward = true;
				moving = true;
			}
			if(Input.GetKeyDown(KeyCode.LeftArrow)){
				Camera.main.transform.eulerAngles -= new Vector3(0, 90, 0);
			}

			if(rotate){ 
				if(!flag2){
					
					if(section == 1 || section == 4){
						targetRot = Camera.main.transform.eulerAngles + new Vector3(0, 90, 0);	
					}
					if(section == 2 || section == 3){
						targetRot = Camera.main.transform.eulerAngles - new Vector3(0, 90, 0);	
					}
					temp = Camera.main.transform.eulerAngles;
					flag2 = true;
					//				Debug.Log(temp);
					//				Debug.Log(targetRot);
				}
				
				temp = Vector3.Lerp(temp, targetRot, 0.1f);
				//			Camera.main.transform.eulerAngles = Vector3.Lerp(Camera.main.transform.eulerAngles, targetRot, 0.1f);
				Camera.main.transform.eulerAngles = temp;
				//			Debug.Log(11111111111);
				//			Debug.Log(Camera.main.transform.eulerAngles);
				//			Debug.Log(targetRot);
				float value = temp.y - targetRot.y;
				
				if((Math.Abs(value) < 0.5)){
					rotate = false;
					Camera.main.transform.eulerAngles = targetRot;
					flag2 = false;
				}
			}

			if(moveForward && !flag2){
				if(!flag){
					targetPos = Camera.main.transform.position + Camera.main.transform.forward * 250;
					flag = true;
	//				Debug.Log("moving forward");
				}
				Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, targetPos, 0.05f);

				if(Vector3.Distance(Camera.main.transform.position,  targetPos) < 0.5f){
					Camera.main.transform.position = targetPos;
					moveForward = false;
					flag = false;
					moving = false;
				}
			}
		}

		void GetSection(){

			if(Camera.main.transform.position.x < terrain.transform.position.x + 250){
				if(Camera.main.transform.position.z < terrain.transform.position.z + 250){
					section = 1;
				}else{
					section = 4;
				}
			}else{
				if(Camera.main.transform.position.z < terrain.transform.position.z){
					section = 2;
				}else{
					section = 3;
				}
			}
		}

	}
}