using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This script is used
/// </summary>
public class TransitionManager : MonoBehaviour
{
    #region Local Fields

    private bool isReady;

    #endregion

    #region Properties

    public bool Ready
    {
        get { return isReady; }
    }

    #endregion

    #region Public Methods

    public void Transition(int a_nextSceneIndex)
    {
        SceneManager.LoadScene(a_nextSceneIndex);
    }

    public void Init()
    {
        isReady = true;
    }

    public void SceneLoaded(Scene current, Scene next)
    {
        if (next.buildIndex > 0)
        {
            GameObject playerGo = GameObject.FindGameObjectWithTag("Player");

            Unit earthUnit = null;
            Unit LightningUnit = null;

            if (playerGo)
            {
                PlayerController player = playerGo.GetComponent<PlayerController>();

                earthUnit = player.SpawnEarthUnit();
                LightningUnit = player.SpawnLightningUnit();

                player.Init();

                playerGo.GetComponent<OverworldController>().Init();
            }
            else
            {
                Debug.LogError("No PlayerController found!");
            }

            GameObject AI_Controller = GameObject.FindGameObjectWithTag("AI_Controller");

            if (AI_Controller && LightningUnit && earthUnit)
            {
                AI_Controller.GetComponent<AI_Controller>().Init(earthUnit, LightningUnit);
            }
            else
            {
                Debug.LogError("No AI_Controller found!");
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

    #endregion

    #region Unity Life-cycle Methods

    private void Awake()
    {
        SceneManager.activeSceneChanged += SceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.activeSceneChanged -= SceneLoaded;
    }

    #endregion
}