using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private Button move;

    private Button attack;

    private Button specialAttack;

    private GameObject gameplayUI;

    private Image fadePlane;

    private Text loadingText;

    private PlayerController player;

    private Unit selectedUnit;

    private Button[] buttons;

    private bool[] buttonInitialState;

    private bool isLocked;

    private bool isReady;

    public void Init()
    {
        isReady = true;

        //isLocked = false;

        //GameObject playerGO = GameObject.FindGameObjectWithTag("Player");

        //if (playerGO)
        //{
        //    player = playerGO.GetComponent<PlayerController>();
        //}
        //else
        //{
        //    Debug.LogError("No player controller found!");
        //}

        //buttons = GetComponentsInChildren<Button>();

        //buttonInitialState = new bool[buttons.Length];

        //for (int i = 0; i < buttons.Length; i++)
        //{
        //    buttonInitialState[i] = buttons[i].interactable;
        //}

        //
    }

    private void Awake()
    {
        Transform canvas = GameObject.FindGameObjectWithTag("UI").transform;

        if (canvas)
        {
            gameplayUI = canvas.GetChild(0).gameObject;

            move = gameplayUI.transform.GetChild(0).GetComponent<Button>();
            attack = gameplayUI.transform.GetChild(1).GetComponent<Button>();
            specialAttack = gameplayUI.transform.GetChild(2).GetComponent<Button>();

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

            gameplayUI.SetActive(false);
        }

        TurnManager.TurnEvent += TurnEvent;

        // Subscribe to game state
        StateManager.OnGameStateChanged += GameStateChanged;
    }

    private void GameStateChanged(Game_state a_oldstate, Game_state a_newstate)
    {
        // Leaving the loading state
        if (a_oldstate == Game_state.loading)
        {
            StartCoroutine(FadeOutLoading());
        }

        if (a_newstate == Game_state.battle)
        {
            StartCoroutine(FadeIntoBattle());
        }
        else
        {
            gameplayUI.SetActive(false);
        }
    }

    private IEnumerator FadeIntoBattle()
    {
        StartCoroutine(FadeImage(0, 1, 1.0f));

        yield return new WaitForSeconds(1.0f);

        gameplayUI.SetActive(true);

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

    public void SelectAction(int actionIndex)
    {
        // player.SelectAction(actionIndex);
    }

    private void TurnEvent(Turn_state a_newState, TEAM a_team, int a_turnNumber)
    {
        if (a_team == TEAM.player)
        {
            if (a_newState == Turn_state.start)
            {
                //ToggleHUD();
                //endTurnButton.Toggle();
            }
            else if (a_newState == Turn_state.end)
            {
                //ToggleHUD();
                //endTurnButton.Toggle();
            }
        }
    }

    public bool Ready
    {
        get { return isReady; }
    }
}