using UnityEngine;

/// <summary>
/// A simple script that makes the gameobject face towards the Main camera
/// </summary>
public class FaceCamera : MonoBehaviour
{
    private GameObject mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main.gameObject;
    }

    void LateUpdate ()
    {
        transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward, mainCamera.transform.rotation * Vector3.up);
        //transform.LookAt(camera.transform.position);
    }
}
