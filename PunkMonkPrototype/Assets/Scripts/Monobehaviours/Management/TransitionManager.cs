using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour
{
    #region Inspector Variables

    [SerializeField] private Unit earthUnitPrefab;

    [SerializeField] private Unit LightningUnitPrefab;

    #endregion

    private bool isReady;

    public void Init()
    {
        isReady = true;
    }

    private void Awake()
    {
        SceneManager.activeSceneChanged += SceneLoaded;
    }

    public void SceneLoaded(Scene current, Scene next)
    {
        if (next.buildIndex > 0)
        {
            GameObject earthGO = GameObject.FindGameObjectWithTag("EarthUnitSpawn");
            Hex spawnHexEarth = null;
            Unit earth = null;

            if (earthGO)
            {
                spawnHexEarth = earthGO.transform.parent.GetComponent<Hex>();

                Vector3 spawnPosEarth = spawnHexEarth.transform.position;
                spawnPosEarth.y = 0.1f;

                earth = Instantiate(earthUnitPrefab, spawnPosEarth, Quaternion.identity);
                earth.Spawn(spawnHexEarth);
            }
            else
            {
                Debug.LogError("No Earth unit found!");
            }


            GameObject lightningGO = GameObject.FindGameObjectWithTag("LightningUnitSpawn");
            Hex spawnHexLightning = null;
            Unit lightning = null;

            if (lightningGO)
            {
                spawnHexLightning = lightningGO.transform.parent.GetComponent<Hex>();

                Vector3 spawnPosLightning = spawnHexLightning.transform.position;
                spawnPosLightning.y = 0.1f;

                lightning = Instantiate(LightningUnitPrefab, spawnPosLightning, Quaternion.identity);
                lightning.Spawn(spawnHexLightning);

                lightning.GetComponent<OverworldFollower>().Init();
            }
            else
            {
                Debug.LogError("No lightning unit found!");
            }


            GameObject playerGo = GameObject.FindGameObjectWithTag("Player");

            if (playerGo && lightningGO && earthGO)
            {
                playerGo.GetComponent<PlayerController>().Init();
                playerGo.GetComponent<OverworldController>().Init();
            }
            else
            {
                Debug.LogError("No PlayerController found!");
            }

            GameObject AI_Controller = GameObject.FindGameObjectWithTag("AI_Controller");

            if (AI_Controller && lightningGO && earthGO)
            {
                playerGo.GetComponent<AI_Controller>().Init(earth, lightning);
            }
            else
            {
                Debug.LogError("No PlayerController found!");
            }

            GameObject cameraGO = GameObject.FindGameObjectWithTag("CameraRig");

            if (cameraGO)
            {
                cameraGO.GetComponent<CameraController>().Init();
            }
            else
            {
                Debug.LogError("No CameraController found!");
            }
        }
    }

    public static void Transition(int a_nextSceneIndex)
    {
        Debug.Log("Loading Scene " + a_nextSceneIndex);

        SceneManager.LoadScene(a_nextSceneIndex);
    }

    public bool Ready
    {
        get { return isReady; }
    }


    private void OnDisable()
    {
        SceneManager.activeSceneChanged -= SceneLoaded;
    }
}