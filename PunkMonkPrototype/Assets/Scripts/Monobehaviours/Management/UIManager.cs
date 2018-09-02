using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// This script is used to manage UI transitions
/// </summary>

public class UIManager : MonoBehaviour
{
    #region Unity Inspector Fields

    [SerializeField]
    private GameObject pauseMenu = null;

    [SerializeField]
    private FadingUI battleUI = null;

    [SerializeField]
    private Image fadePlane = null;

    [SerializeField]
    private Button move = null;

    [SerializeField]
    private Button attack = null;

    [SerializeField]
    private Button specialAttack = null;

    [SerializeField]
    private Sprite hexIconFilled;

    [SerializeField]
    private Sprite hexIconEmpty;

    #endregion

    #region Reference Fields

    private TextMeshProUGUI loadingText;

    private ProfileSwitcher profiles;

    private PlayerController player;

    private Unit selectedUnit;

    private Button[] buttons;

    private CameraController cameraController;




    #endregion

    #region Local Fields

    private bool[] buttonState;

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

    public void LinkToCamera()
    {
        cameraController = GameObject.FindGameObjectWithTag("CameraRig").GetComponent<CameraController>();

        cameraController.onGlamCamStart += HideUI;
        cameraController.onGlamCamEnd += ShowUI;
    }

    public void SelectAction(int actionIndex)
    {
        player.SelectAction(actionIndex);
    }

    public void Continue()
    {
        Manager.instance.StateController.PauseGame();
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void PauseMenu(bool a_show)
    {
        pauseMenu.SetActive(a_show);
    }

    public void UpdateSelectedUnit(Unit a_selectedUnit)
    {
        if (a_selectedUnit != selectedUnit)
        {
            if (selectedUnit != null)
            {
                profiles.Switch(a_selectedUnit.CompareTag("EarthUnit"));

                selectedUnit.OnCanMoveChanged -= CanMove;
                selectedUnit.OnCanAttackChanged -= CanAttack;
                selectedUnit.OnCanSpecialChanged -= CanSpecialAttack;
            }

            selectedUnit = a_selectedUnit;

            selectedUnit.OnCanMoveChanged += CanMove;
            selectedUnit.OnCanAttackChanged += CanAttack;
            selectedUnit.OnCanSpecialChanged += CanSpecialAttack;

            CanMove(selectedUnit.CanMove);
            CanAttack(selectedUnit.CanAttack);
            CanSpecialAttack(selectedUnit.CanSpecialAttack);
        }
    }

    public void CanMove(bool a_value)
    {
        if (battleUI.IsFading)
        {
            buttonState[0] = a_value;
        }
        else
        {
            move.interactable = buttonState[0] = a_value;

            profiles.CurrentProfile.GetChild(1).GetChild(0).GetComponent<Image>().sprite = (buttonState[0]) ? hexIconFilled : hexIconEmpty;
        }
    }

    public void CanAttack(bool a_value)
    {
        if (battleUI.IsFading)
        {
            buttonState[1] = a_value;
        }
        else
        {
            attack.interactable = buttonState[1] = a_value;

            if (buttonState[1])
            {
                profiles.CurrentProfile.GetChild(1).GetChild(1).GetComponent<Image>().sprite = hexIconFilled;
            }
            else
            {
                profiles.CurrentProfile.GetChild(1).GetChild(1).GetComponent<Image>().sprite = hexIconEmpty;
            }

        }
    }

    public void CanSpecialAttack(bool a_value)
    {
        if (battleUI.IsFading)
        {
            buttonState[2] = a_value;
        }
        else
        {
            specialAttack.interactable = buttonState[2] = a_value;

            if (buttonState[2])
            {
                profiles.CurrentProfile.GetChild(1).GetChild(2).GetComponent<Image>().sprite = hexIconFilled;
            }
            else
            {
                profiles.CurrentProfile.GetChild(1).GetChild(2).GetComponent<Image>().sprite = hexIconEmpty;
            }

        }
    }

    public void SwitchPlayerSelection()
    {
        player.SwitchSelection();
    }

    public void LockUI()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].interactable = false;
        }
    }

    public void UnlockUI()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].interactable = buttonState[i];
        }
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
            profiles = battleUI.transform.GetChild(0).GetComponent<ProfileSwitcher>();

            if (fadePlane)
            {
                fadePlane.color = new Color(fadePlane.color.r, fadePlane.color.g, fadePlane.color.b, 1.0f);
                loadingText = fadePlane.transform.GetComponentInChildren<TextMeshProUGUI>();

                if (loadingText)
                {
                    loadingText.color = new Color(loadingText.color.r, loadingText.color.g, loadingText.color.b, 1.0f);
                }
            }
        }

        Manager.instance.TurnController.PlayerTurnEvent += TurnEvent;
        Manager.instance.StateController.OnGameStateChanged += GameStateChanged;

        buttons = new Button[]
            {
                move,
                attack,
                specialAttack
            };


        buttonState = new bool[buttons.Length];

        for (int i = 0; i < buttons.Length; i++)
        {
            buttonState[i] = buttons[i].interactable;
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
        if (a_oldstate == GameState.loading)
        {
            StartCoroutine(FadeOutLoading());
        }
        else if (a_oldstate != GameState.mainmenu && a_newstate == GameState.loading)
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
            battleUI.FadeIn(UnlockUI);
        }
        else if (a_newState == TurnManager.TurnState.end)
        {
            battleUI.FadeOut();
        }
    }

    private void HideUI()
    {
        battleUI.HideInstant();
    }

    private void ShowUI()
    {
        if (Manager.instance.StateController.CurrentGameState == GameState.battle)
        {
            battleUI.ShowInstant();
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
    private IEnumerator Fade(Color from, Color to, float time, TextMeshProUGUI _text)
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