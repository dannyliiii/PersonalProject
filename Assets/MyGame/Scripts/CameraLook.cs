using UnityEngine;
using System.Collections;
using System;

public class CameraLook  {
	
	private Quaternion m_CameraTargetRot;
	private bool clampVerticalRotation = true;
	public bool smooth = false;
	public float smoothTime = 5f;
	public float MinimumX = -90F;
	public float MaximumX = 90F;

	public void Init(Transform camera){
		m_CameraTargetRot = camera.localRotation;
	}

	public void LookRotation(Transform camera, float xRot)
	{
		m_CameraTargetRot *= Quaternion.Euler (0f, xRot, 0f);
		
		if(clampVerticalRotation)
			m_CameraTargetRot = ClampRotationAroundXAxis (m_CameraTargetRot);
		
		if(smooth)
		{
			Camera.main.transform.localRotation = Quaternion.Slerp (camera.localRotation, m_CameraTargetRot,
			                                                        smoothTime * Time.deltaTime);
//			camera.localRotation = Quaternion.Slerp (camera.localRotation, m_CameraTargetRot,
//			                                         smoothTime * Time.deltaTime);
		}
		else
		{
			Camera.main.transform.localRotation = m_CameraTargetRot;
//			camera.localRotation = m_CameraTargetRot;
		}
	}
	
	
	Quaternion ClampRotationAroundXAxis(Quaternion q)
	{
		q.x /= q.w;
		q.y /= q.w;
		q.z /= q.w;
		q.w = 1.0f;
		
		float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan (q.x);
		
		angleX = Mathf.Clamp (angleX, MinimumX, MaximumX);
		
		q.x = Mathf.Tan (0.5f * Mathf.Deg2Rad * angleX);
		
		return q;
	}
}
