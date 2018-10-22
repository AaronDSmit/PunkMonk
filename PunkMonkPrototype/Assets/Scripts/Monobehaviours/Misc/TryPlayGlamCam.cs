using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TryPlayGlamCam : MonoBehaviour 
{
	private float camChance;
	private CameraController cam;


	private void Awake()
	{
		cam = GameObject.FindGameObjectWithTag ("CameraRig").GetComponent<CameraController> ();
		camChance = cam.GlamCamChance;
	}

	public void PlayGlamCam()
	{
		if (Random.Range (0, 100) <= camChance) 
		{
			cam.PlayGlamCam(gameObject.transform.parent.GetComponent<Unit>());
		}
	}

}
