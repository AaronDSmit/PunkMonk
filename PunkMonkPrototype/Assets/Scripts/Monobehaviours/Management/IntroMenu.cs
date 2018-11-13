using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// This script is used for the main menu, it loads the next scene before switching to it
/// </summary>
public class IntroMenu : MonoBehaviour
{
    #region Unity Inspector Fields

    [SerializeField]
    private Image fadePlane = null;

    [SerializeField]
    private TextMeshProUGUI loadingText = null;

    [SerializeField]
    private Image loadingProgress = null;

    [SerializeField]
    private Image loadingImage = null;

    [SerializeField]
    private AK.Wwise.Event startMenuMusic = null;

    [SerializeField]
    private AK.Wwise.Event endMenuMusic = null;

    #endregion

    #region Local Fields

    private AsyncOperation async;

    public float loadProgress = 0f;

    private bool finishedLoading = false;

    private bool loadingScene = false;

    #endregion

    #region Public Methods

    public void StartGame()
    {
        Time.timeScale = 1.0f;

        fadePlane.raycastTarget = true;

        StopAllCoroutines();

        endMenuMusic.Post(gameObject);

        StartCoroutine(ChangeScenes(1));
    }

    #endregion

    #region Unity Life-cycle Methods

    private void Awake()
    {
        if (GameObject.FindGameObjectWithTag("Manager") != null)
        {
            Destroy(GameObject.FindGameObjectWithTag("Manager"));
        }
    }

    private void Start()
    {
        startMenuMusic.Post(gameObject);
        if (fadePlane != null)
        {
            StartCoroutine(Fade(Color.black, Color.clear, 1.0f, fadePlane));
            StartCoroutine(DoFadePlaneRaycast(1.0f));
        }
        //StartCoroutine(LoadScene());
    }

    #endregion

    #region Local Methods

    // Fades the fadePlane image from a colour to another over x seconds.
    private IEnumerator Fade(Color a_from, Color a_to, float a_time, Image a_plane)
    {
        float speed = 1 / a_time;
        float percent = 0;

        while (percent < 1)
        {
            percent += Time.deltaTime * speed;
            a_plane.color = Color.Lerp(a_from, a_to, percent);

            yield return null;
        }
    }

    // Fades the fadePlane image from a colour to another over x seconds.
    private IEnumerator Fade(Color a_from, Color a_to, float a_time, TextMeshProUGUI a_text)
    {
        float speed = 1 / a_time;
        float percent = 0;

        while (percent < 1)
        {
            percent += Time.deltaTime * speed;
            a_text.color = Color.Lerp(a_from, a_to, percent);

            yield return null;
        }
    }

    private IEnumerator LoadScene()
    {
        async = SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);
        async.allowSceneActivation = false;
        finishedLoading = false;
        loadingScene = true;

        while (async.isDone == false)
        {
            loadProgress = async.progress;
            loadingProgress.fillAmount = loadProgress + 0.05f;
            yield return null;
        }

        finishedLoading = true;
        loadingScene = false;
    }

    private IEnumerator ChangeScenes(float a_fadeTime)
    {
        if (fadePlane != null)
            StartCoroutine(Fade(Color.clear, Color.black, a_fadeTime, fadePlane));
        if (loadingText != null)
            StartCoroutine(Fade(Color.clear, Color.white, a_fadeTime, loadingText));
        if (loadingProgress != null)
            StartCoroutine(Fade(Color.clear, Color.white, a_fadeTime, loadingProgress));
        if (loadingImage != null)
            StartCoroutine(Fade(Color.clear, Color.white, a_fadeTime, loadingImage));

        yield return new WaitForSeconds(a_fadeTime);

        if (loadingScene == false)
            StartCoroutine(LoadScene());

        async.allowSceneActivation = true;
    }

    private IEnumerator DoFadePlaneRaycast(float a_time)
    {
        fadePlane.raycastTarget = true;

        yield return new WaitForSeconds(a_time);

        fadePlane.raycastTarget = false;

    }

    #endregion
}