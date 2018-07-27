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

    [HideInInspector]
    [SerializeField] private List<TransitionPoint> transitionPoints;

    // Global Manager ref
    public static TransitionManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            //stop the game from having more than one GameStateManager
            Destroy(gameObject);
            return;
        }

        //Don't destroy the GameStateManager in scene
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
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


            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().Init();
        }
    }

    public static void Transition(int a_nextSceneIndex)
    {
        Debug.Log("Loading Scene " + a_nextSceneIndex);

        SceneManager.LoadScene(a_nextSceneIndex);
    }
}