using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoRenderOnTrigger : MonoBehaviour
{
    private new Renderer renderer = null;

    private void Awake()
    {
        renderer = GetComponent<Renderer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "MainCamera")
        {
            renderer.enabled = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "MainCamera")
        {
            renderer.enabled = true;
        }
    }
}
