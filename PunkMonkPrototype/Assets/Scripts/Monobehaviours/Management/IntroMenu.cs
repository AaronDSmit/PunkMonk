using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// This script is used for the main menu, it loads the next scene before switching to it
/// </summary>
public class IntroMenu : MonoBehaviour
{
    #region Unity Inspector Fields

    [SerializeField]
    private Image FadePlane;

    [SerializeField]
    private Text loading;

    [SerializeField]
    private GameObject Credits;

    #endregion

    #region Local Fields

    private AsyncOperation async;

    public float progress = 0f;

    public bool isDone = false;

    #endregion

    #region Public Methods

    public void StartGame()
    {
        StartCoroutine(ChangeScenes());
    }

    #endregion

    #region Unity Life-cycle Methods

    private void Start()
    {
        StartCoroutine(LoadScene());
    }

    #endregion

    #region Local Methods

    // Fades the fadePlane image from a colour to another over x seconds.
    private IEnumerator Fade(Color from, Color to, float time, Image _plane)
    {
        float speed = 1 / time;
        float percent = 0;

        while (percent < 1)
        {
            percent += Time.deltaTime * speed;
            _plane.color = Color.Lerp(from, to, percent);

            yield return null;
        }
    }

    // Fades the fadePlane image from a colour to another over x seconds.
    private IEnumerator Fade(Color from, Color to, float time, Text _text)
    {
        float speed = 1 / time;
        float percent = 0;

        while (percent < 1)
        {
            percent += Time.deltaTime * speed;
            _text.color = Color.Lerp(from, to, percent);

            yield return null;
        }
    }

    private IEnumerator LoadScene()
    {
        async = SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);
        async.allowSceneActivation = false;

        while (async.progress <= 0.89f)
        {
            progress = async.progress;
            yield return null;
        }
    }

    private IEnumerator ChangeScenes()
    {
        StartCoroutine(Fade(Color.clear, Color.black, 1, FadePlane));
        StartCoroutine(Fade(Color.clear, Color.white, 1, loading));

        yield return new WaitForSeconds(1f);

        async.allowSceneActivation = true;
    }

    #endregion
}