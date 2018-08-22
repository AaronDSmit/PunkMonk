using UnityEngine;

/// <summary>
/// A simple script that makes the gameobject face towards the Main camera
/// </summary>
public class FaceCamera : MonoBehaviour
{
    private GameObject camera;

    private void Awake()
    {
        camera = Camera.main.gameObject;
    }

    void LateUpdate ()
    {
        transform.LookAt(camera.transform.position);
	}
}
