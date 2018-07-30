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

    [HideInInspector]
    [SerializeField]
    private List<SceneTransitionPoint> transitionPoints;

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
            Tile spawnTileEarth = GameObject.FindGameObjectWithTag("EarthUnitSpawn").transform.parent.GetComponent<Tile>();
            Tile spawnTileLightning = GameObject.FindGameObjectWithTag("LightningUnitSpawn").transform.parent.GetComponent<Tile>();

            Vector3 spawnPosEarth = spawnTileEarth.transform.position;
            spawnPosEarth.y = 0.1f;

            Vector3 spawnPosLightning = spawnTileLightning.transform.position;
            spawnPosLightning.y = 0.1f;

            Unit earth = Instantiate(earthUnitPrefab, spawnPosEarth, Quaternion.identity);
            earth.Spawn(spawnTileEarth);

            Unit lightning = Instantiate(LightningUnitPrefab, spawnPosLightning, Quaternion.identity);
            lightning.Spawn(spawnTileLightning);

            GameObject playerGo = GameObject.FindGameObjectWithTag("Player");

            playerGo.GetComponent<PlayerController>().Init();
            playerGo.GetComponent<OverworldController>().Init();

            CameraController cameraController = GameObject.FindGameObjectWithTag("CameraRig").GetComponent<CameraController>();
            cameraController.Init();
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