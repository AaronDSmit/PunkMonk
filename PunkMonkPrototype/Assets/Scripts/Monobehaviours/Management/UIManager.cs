﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This script is used to manage UI transitions
/// </summary>

public class UIManager : MonoBehaviour
{
    #region Unity Inspector Fields

    [SerializeField]
    private Button move;

    [SerializeField]
    private Button attack;

    [SerializeField]
    private Button specialAttack;

    #endregion

    #region Reference Fields

    private FadingUI battleUI;

    private Image fadePlane;

    private Text loadingText;

    private ProfileSwitcher profiles;

    private PlayerController player;

    private Unit selectedUnit;

    private Button[] buttons;

    #endregion

    #region Local Fields

    private bool[] buttonInitialState;

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
        UpdateUnitInfo();
    }

    public void LockUI()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            // save previous state
            buttonInitialState[i] = buttons[i].interactable;

            buttons[i].interactable = false;
        }
    }

    public void UnlockUI()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].interactable = buttonInitialState[i];
        }

        UpdateUnitInfo();
    }

    public void EndPlayersTurn()
    {
        Manager.instance.TurnController.EndTurn(TEAM.player);
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

            fadePlane = canvas.Find("FadePlane").GetComponent<Image>();
            loadingText = fadePlane.transform.GetComponentInChildren<Text>();

            if (fadePlane)
            {
                fadePlane.color = new Color(fadePlane.color.r, fadePlane.color.g, fadePlane.color.b, 1.0f);
            }

            if (loadingText)
            {
                loadingText.color = new Color(loadingText.color.r, loadingText.color.g, loadingText.color.b, 1.0f);
            }
        }

        Manager.instance.TurnController.PlayerTurnEvent += TurnEvent;
        Manager.instance.StateController.OnGameStateChanged += GameStateChanged;

        buttons = GetComponentsInChildren<Button>();

        buttonInitialState = new bool[buttons.Length];

        for (int i = 0; i < buttons.Length; i++)
        {
            buttonInitialState[i] = buttons[i].interactable;
        }

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }


    private void OnDestroy()
    {
        Manager.instance.TurnController.PlayerTurnEvent -= TurnEvent;
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
        }
    }

    private void TurnEvent(TurnManager.TurnState a_newState, int a_turnNumber)
    {
        if (a_newState == TurnManager.TurnState.start)
        {
            battleUI.FadeIn();
        }
        else if (a_newState == TurnManager.TurnState.end)
        {
            battleUI.FadeOut();
        }
    }

    private void UpdateUnitInfo()
    {
        if (selectedUnit)
        {
            move.interactable = selectedUnit.CanMove;
            attack.interactable = selectedUnit.CanAttack;
            specialAttack.interactable = selectedUnit.CanSpecialAttack;
        }
    }

    #region Coroutines

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

    #endregion

    #endregion
}