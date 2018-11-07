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
        GameObject cam = GameObject.FindGameObjectWithTag("MainCamera");
        Vector3 ogCamPos = cam.transform.position;

        var CASettings = cam.GetComponents<PostProcessingBehaviour>()[0].profile.chromaticAberration.settings;

        float startCA = CASettings.intensity;

        float elapsed = 0.0f;

        float currentLerpPercent = 0.0f;

        while(elapsed < 0.7f)
        {
            float x = Random.Range(-1f, 1f) * a_magintude;
            float y = Random.Range(-1f, 1f) * a_magintude;

            currentLerpPercent = elapsed / (0.7f / 2);

            Mathf.Lerp(startCA, 1, Mathf.PingPong(currentLerpPercent, 0.5f));

            cam.transform.localPosition = new Vector3(x, y, ogCamPos.z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        cam.transform.localPosition = ogCamPos;

    }
}
