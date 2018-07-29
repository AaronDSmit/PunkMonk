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
    [SerializeField] private List<SceneTransitionPoint> transitionPoints;

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
            Tile spawnPosEarth = GameObject.FindGameObjectWithTag("EarthUnitSpawn").transform.parent.GetComponent<Tile>();
            Tile spawnPosLightning = GameObject.FindGameObjectWithTag("LightningUnitSpawn").transform.parent.GetComponent<Tile>();

            Unit earth = Instantiate(earthUnitPrefab, spawnPosEarth.transform.position, Quaternion.identity);
            earth.Spawn(spawnPosEarth);

            Unit lightning = Instantiate(LightningUnitPrefab, spawnPosLightning.transform.position, Quaternion.identity);
            lightning.Spawn(spawnPosLightning);

            GameObject playerGo = GameObject.FindGameObjectWithTag("Player");

            playerGo.GetComponent<PlayerController>().Init();
            playerGo.GetComponent<OverworldController>().Init();
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