using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// This script is used.
/// </summary>
public class TransitionManager : MonoBehaviour
{

    #region Unity Inspector Fields

    [SerializeField]
    private Image fadePlane = null;

    #endregion

    #region Local Fields

    private bool isReady;

    private AsyncOperation async;

    public float loadProgress = 0f;

    private bool finishedLoading = false;

    private bool loadingScene = false;

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
        //SceneManager.LoadScene(a_nextSceneIndex);
        StartCoroutine(ChangeScenes(a_nextSceneIndex, 2));
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

            Manager.instance.UIController.LinkToCamera();
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

    private IEnumerator LoadScene(int a_sceneIndex)
    {
        async = SceneManager.LoadSceneAsync(a_sceneIndex, LoadSceneMode.Single);
        async.allowSceneActivation = false;
        finishedLoading = false;
        loadingScene = true;

        while (async.isDone == false)
        {
            loadProgress = async.progress;
            yield return null;
        }

        finishedLoading = true;
        loadingScene = false;
    }

    private IEnumerator ChangeScenes(int a_sceneIndex, float a_fadeTime)
    {
        if (fadePlane != null)
            StartCoroutine(Fade(Color.clear, Color.black, a_fadeTime, fadePlane));

        yield return new WaitForSeconds(a_fadeTime);

        if (loadingScene == false)
            StartCoroutine(LoadScene(a_sceneIndex));

        async.allowSceneActivation = true;
    }

    // Fades the image from a colour to another over x seconds.
    private IEnumerator Fade(Color a_from, Color a_to, float a_time, Image a_image)
    {
        float speed = 1 / a_time;
        float percent = 0;

        while (percent < 1)
        {
            percent += Time.deltaTime * speed;
            a_image.color = Color.Lerp(a_from, a_to, percent);

            yield return null;
        }
    }
}