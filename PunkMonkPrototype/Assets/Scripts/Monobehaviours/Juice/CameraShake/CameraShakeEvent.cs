using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShakeEvent : MonoBehaviour
{
    CameraShake cam;

    private void OnEnable()
    {
        cam = Camera.main.transform.parent.GetComponent<CameraShake>();
    }

    public void StartCameraShake(float a_trauma)
    {
        cam.StartCameraShake(a_trauma);
    }

}
