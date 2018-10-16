using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetDissolveToCamera : MonoBehaviour
{
    private Camera mainCamera;
    private Material material;

	void Awake()
    {
        mainCamera = Camera.main;
        material = GetComponent<Renderer>().material;
	}
	
	void Update ()
    {
        material.SetVector("_RayDirection", mainCamera.transform.forward);
        material.SetVector("_RayOrigin", mainCamera.transform.position);
    }
}
