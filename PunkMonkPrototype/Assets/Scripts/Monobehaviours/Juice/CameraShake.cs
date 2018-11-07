using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class CameraShake : MonoBehaviour
{

    public void StartCameraShake(float magnutude)
    {
        StartCoroutine(Shake(magnutude));
    }
        
    IEnumerator Shake (float a_magintude)
    {
        GameObject cam = GameObject.FindGameObjectWithTag("CameraRig");

        CameraController camRig = cam.GetComponent<CameraController>();

        Vector3 ogCamPos = cam.transform.position;

        var CASettings = cam.transform.GetChild(0).GetComponents<PostProcessingBehaviour>()[0].profile.chromaticAberration.settings;

        float startCA = CASettings.intensity;

        float elapsed = 0.0f;

        float currentLerpPercent = 0.0f;

        camRig.Cinemachine = true;

        while (elapsed < 0.7f)
        {
            float x = Random.Range(-1f, 1f) * a_magintude;
            float z = Random.Range(-1f, 1f) * a_magintude;

            currentLerpPercent = elapsed / (0.7f);

            Mathf.Lerp(startCA, 1, Mathf.PingPong(currentLerpPercent, 1));

            cam.transform.localPosition = new Vector3(x, ogCamPos.y, z);

            elapsed += Time.deltaTime;

            yield return null;
        }
        camRig.Cinemachine = false;
        cam.transform.localPosition = ogCamPos;

    }
}
