using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class CameraShake : MonoBehaviour
{
    #region Inspector Fields

    [Tooltip("How long it takes for the trauma level to hit 0 from the maximum 1 in seconds")]
    [Range(0.02f, 10.0f)]
    [SerializeField]
    private float traumaDecay = 2.0f;

    [Range(0.02f, 100.0f)]
    [SerializeField]
    private float strength = 5.0f;

    [SerializeField]
    private float maxYaw = 5.0f;
    [SerializeField]
    private float maxPitch = 5.0f;
    [SerializeField]
    private float maxRoll = 5.0f;

    #endregion

    #region References

    GameObject cam = null;

    #endregion

    #region Local Fields

    private float shake = 0.0f;

    private float trauma = 0.0f;

    private float yaw = 0.0f;
    private float pitch = 0.0f;
    private float roll = 0.0f;

    private int seed = 0;

    #endregion

    #region Properties

    #endregion

    public void StartCameraShake(float a_trauma)
    {
        StopAllCoroutines();
        StartCoroutine(Shake(a_trauma));
    }

    private void Start()
    {
        seed = Random.Range(0, 2048);

        cam = gameObject;
    }

    IEnumerator Shake(float a_trauma)
    {
        trauma += a_trauma;
        trauma = Mathf.Clamp01(trauma);

        CameraController camRig = cam.GetComponent<CameraController>();

        camRig.OverrideRot = true;

        Quaternion startRotation = cam.transform.rotation;

        while (trauma > 0)
        {
            shake = trauma * trauma;

            yaw = Mathf.Lerp(-maxYaw, maxYaw, Mathf.PerlinNoise(seed, Time.time * strength)) * shake;
            pitch = Mathf.Lerp(-maxPitch, maxPitch, Mathf.PerlinNoise(seed * 2, Time.time * strength)) * shake;
            roll = Mathf.Lerp(-maxRoll, maxRoll, Mathf.PerlinNoise(seed * 3, Time.time * strength)) * shake;

            cam.transform.rotation = startRotation * Quaternion.Euler(pitch, yaw, roll);

            // Linearly decrease trauma
            if (trauma > 0)
                trauma -= Time.deltaTime / traumaDecay;
            else
                trauma = 0;

            yield return null;
        }

        cam.transform.rotation = startRotation;

        camRig.OverrideRot = false;
    }
}
