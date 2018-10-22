using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObjectsOnTrigger : MonoBehaviour {

	[SerializeField] GameObject[] objects;

	private void OnTriggerEnter(Collider other)
	{
		foreach (GameObject go in objects) 
		{
			Destroy (go);
		}
	}

}
