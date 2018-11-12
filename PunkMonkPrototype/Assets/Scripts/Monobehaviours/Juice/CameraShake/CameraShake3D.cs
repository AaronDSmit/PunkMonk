using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Implemented based off the GDC talk "Math for Game Programmers: Juicing Your Cameras With Math" https://www.youtube.com/watch?v=tu-Qe66AvtY

/// <summary>
/// 
/// </summary>
public class CameraShake3D : MonoBehaviour
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

    #region Public Methods

    public void AddTrauma(float a_trauma)
    {
        trauma += a_trauma;
        trauma = Mathf.Clamp01(trauma);
    }

    #endregion

    #region Unity Methods

    private void Start()
    {
        seed = Random.Range(0, 2048);
    }

    private void LateUpdate()
    {
        shake = trauma * trauma;

        yaw = Mathf.Lerp(-maxYaw, maxYaw, Mathf.PerlinNoise(seed, Time.time * strength)) * shake;
        pitch = Mathf.Lerp(-maxPitch, maxPitch, Mathf.PerlinNoise(seed * 2, Time.time * strength)) * shake;
        roll = Mathf.Lerp(-maxRoll, maxRoll, Mathf.PerlinNoise(seed * 3, Time.time * strength)) * shake;

        transform.rotation = Quaternion.Euler(pitch, yaw, roll);

        // Linearly decrease trauma
        if (trauma > 0)
            trauma -= Time.deltaTime / traumaDecay;
        else
            trauma = 0;
    }

    #endregion

    #region Private Methods

    #endregion
}
