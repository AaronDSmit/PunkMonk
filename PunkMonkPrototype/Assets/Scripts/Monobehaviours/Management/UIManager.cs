using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This script is used to manage UI transitions
/// </summary>

public class UIManager : MonoBehaviour
{
    #region Unity Inspector Fields

    #endregion

    #region Reference Fields

    private Button move;

    private Button attack;

    private Button specialAttack;

    private FadingUI battleUI;

    private FadingUI endTurnButton;

    private Image fadePlane;

    private Text loadingText;

    private ProfileSwitcher profiles;

    private PlayerController player;

    private Unit selectedUnit;

    private Button[] buttons;

    #endregion

    #region Local Fields

    private bool[] buttonInitialState;

    private bool isLocked;

    private bool isReady;

    #endregion

    #region Properties

    public bool Ready
    {
        get { return isReady; }
    }

    #endregion

    #region Public Methods

    public void Init()
    {
        isReady = true;

        isLocked = false;
    }

    public void SelectAction(int actionIndex)
    {
        player.SelectAction(actionIndex);
    }

    public void UpdateSelectedUnit(Unit a_selectedUnit)
    {
        if (selectedUnit != null)
        {
            profiles.Switch(a_selectedUnit.CompareTag("EarthUnit"));
        }

        selectedUnit = a_selectedUnit;
    }

    #endregion

    #region Unity Life-cycle Methods

    private void Awake()
    {
        Transform canvas = GameObject.FindGameObjectWithTag("UI").transform;

        if (canvas)
        {
            battleUI = canvas.GetChild(0).GetComponent<FadingUI>();

            profiles = battleUI.transform.GetChild(0).GetComponent<ProfileSwitcher>();

            move = battleUI.transform.Find("Actions").GetChild(0).GetComponent<Button>();
            attack = battleUI.transform.Find("Actions").GetChild(1).GetComponent<Button>();
            specialAttack = battleUI.transform.Find("Actions").GetChild(2).GetComponent<Button>();

            fadePlane = canvas.Find("FadePlane").GetComponent<Image>();
            loadingText = fadePlane.transform.GetComponentInChildren<Text>();

            endTurnButton = canvas.transform.Find("EndTurnButton").GetComponent<FadingUI>();

            if (fadePlane)
            {
                fadePlane.color = new Color(fadePlane.color.r, fadePlane.color.g, fadePlane.color.b, 1.0f);
            }

            if (loadingText)
            {
                loadingText.color = new Color(loadingText.color.r, loadingText.color.g, loadingText.color.b, 1.0f);
            }
        }

        Manager.instance.TurnController.TurnEvent += TurnEvent;
        Manager.instance.StateController.OnGameStateChanged += GameStateChanged;

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    private void OnDisable()
    {
        Manager.instance.TurnController.TurnEvent -= TurnEvent;
        Manager.instance.StateController.OnGameStateChanged -= GameStateChanged;
    }

    #endregion

    #region Local Methods

    private void GameStateChanged(GameState a_oldstate, GameState a_newstate)
    {
        // Leaving the loading state
        if (a_oldstate == GameState.mainmenu)
        {
            StartCoroutine(FadeOutLoading());
        }
        else if (a_newstate == GameState.loading)
        {
            StartCoroutine(FadeIntoState());
        }

        if (a_oldstate == GameState.battle)
        {
            battleUI.FadeOut();
            endTurnButton.FadeOut();
        }
    }

    private IEnumerator FadeIntoState()
    {
        yield return StartCoroutine(FadeImage(0, 1, 1.0f));

        Manager.instance.StateController.MidLoad = true;

        yield return new WaitForSeconds(0.1f);

        StartCoroutine(FadeImage(1, 0, 1.0f));
    }

    private IEnumerator FadeOutLoading()
    {
        //yield return new WaitForSeconds(0.5f);

        // fade out from black
        StartCoroutine(FadeImage(1, 0, 0.5f));
        StartCoroutine(Fade(Color.white, Color.clear, 0.5f, loadingText));

        yield return null;
    }

    // Fades the fadePlane image from a colour to another over x seconds.
    private IEnumerator FadeImage(float from, float to, float time)
    {
        float currentLerpTime = 0;
        float t = 0;

        while (t < 1)
        {
            currentLerpTime += Time.deltaTime;
            t = currentLerpTime / time;

            fadePlane.color = new Color(fadePlane.color.r, fadePlane.color.g, fadePlane.color.b, Mathf.Lerp(from, to, t));

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

    private void TurnEvent(TurnState a_newState, TEAM a_team, int a_turnNumber)
    {
        if (a_team == TEAM.player)
        {
            if (a_newState == TurnState.start)
            {
                battleUI.FadeIn();
                endTurnButton.FadeIn();
            }
            else if (a_newState == TurnState.end)
            {
                battleUI.FadeOut();
                endTurnButton.FadeOut();
            }
        }
    }

    #endregion   
}