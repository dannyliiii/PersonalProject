using UnityEngine;
using System.Collections;
//using System.IO;
using TemplateGesture;
using KinectWrapper;
using Kinect;
using MyMath;
using System.Collections.Generic;

public class Detector : MonoBehaviour {

	public int player = 0;
	TemplatedGestureDetector templateGestureDetector;
	public SkeletonWrapper sw;

	// for shooting projectile
	public GameObject projectile;
	private UnityEngine.Vector3 upForce;
	private readonly int speed = 50;
	private List<GameObject> projList;


	void Awake () {
		projList = new List<GameObject> ();
		upForce = new UnityEngine.Vector3 (0.0f, 150.0f, 0.0f);

		LoadTemplateGestureDetector ();
	}

	void Update () {
		if(!KinectRecorder.IsRecording)
			ProcessFrame ();
	}

	void LoadTemplateGestureDetector()
	{
		templateGestureDetector = new TemplatedGestureDetector();
		templateGestureDetector.OnGestureDetected += OnGestureDetected;

	}

	void OnGestureDetected(string gesture)
	{
		Shoot ();
	}

	void ProcessFrame()
	{
		if (sw.pollSkeleton ()) {
			for (int i = 0; i < (int)Kinect.NuiSkeletonPositionIndex.Count; i++) {

				if (i == (int)NuiSkeletonPositionIndex.HandRight) {
					MyMath.Vector3 pos = new MyMath.Vector3 (sw.bonePos[player, i].x
					                                         ,sw.bonePos[player, i].y
					                                         ,sw.bonePos[player, i].z);
					templateGestureDetector.Add (pos);
					
				}
			}
		}
	}

	void Shoot(){
		
		GameObject proj =  Instantiate(projectile, transform.position, Quaternion.FromToRotation (UnityEngine.Vector3.forward, transform.forward)) as GameObject;
		Rigidbody rb = proj.GetComponent<Rigidbody> ();
		rb.velocity = transform.forward * speed;
		rb.AddForce (upForce);
		ParticleSystem ps = proj.GetComponent<ParticleSystem>();
		ps.Play ();
		projList.Add (proj);
	}

}

