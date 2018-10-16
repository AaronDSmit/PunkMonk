using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetDissolveToCamera : MonoBehaviour
{
    private Camera camera;
    private Material material;

	void Awake()
    {
        camera = Camera.main;
        material = GetComponent<Renderer>().material;
	}
	
	void Update ()
    {
        material.SetVector("_RayDirection", camera.transform.forward);
        material.SetVector("_RayOrigin", camera.transform.position);
    }
}
