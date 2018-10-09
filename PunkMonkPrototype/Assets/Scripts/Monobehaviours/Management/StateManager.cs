using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { mainmenu, overworld, battle, loading, cinematic, transition }

/// <summary>
/// This script is used to handle state changes, the state can be changed instantly, after a delay and during a blackout of the screen.
/// </summary>
public class StateManager : MonoBehaviour
{
    #region Unity Inspector Fields

    [Header("Debug Info")]
    [SerializeField]
    private GameState currentState = GameState.mainmenu;

    [SerializeField]
    private GameState previousState = GameState.mainmenu;

    [SerializeField]
    private GameState stateBeforeTransition = GameState.mainmenu;

    [SerializeField]
    private GameState stateAfterTransition = GameState.mainmenu;

    [SerializeField]
    private List<GameState> stateHistory;


    #endregion

    #region Local Fields

    private bool midLoad = false;

    private bool isReady;

    private bool isPaused;

    public delegate void GameStateChanged(GameState _oldState, GameState _newState);

    // Called right after changing the game state
    public event GameStateChanged OnGameStateChanged;

    public static float stateTransitionTime = 1.0f;

    #endregion

    #region Properties

    public bool MidLoad
    {
        get { return midLoad; }
        set { midLoad = value; }
    }

    public bool Ready
    {
        get { return isReady; }
    }

    public GameState StateBeforeTransition
    {
        get { return stateBeforeTransition; }
    }

    // Returns previous game state
    public GameState PreviousState
    {
        get { return previousState; }
    }

    // Returns current game state
    public GameState CurrentGameState
    {
        get { return currentState; }
    }

    public GameState StateAfterTransition
    {
        get { return stateAfterTransition; }
    }

    #endregion

    #region Public Methods

    public void Init()
    {
        stateHistory = new List<GameState>();

        ChangeGameState(GameState.loading);

        isReady = true;
    }

    public void StartGame()
    {
        ChangeGameState(GameState.overworld);
    }

    public void PauseGame()
    {
        isPaused = !isPaused;

        Manager.instance.UIController.PauseMenu(isPaused);

        if (isPaused)
        {
            Time.timeScale = 0.0f;
        }
        else
        {
            Time.timeScale = 1.0f;
        }
    }

    // Change the current game state to the target game state and calls the event on any script that is listening
    public void ChangeGameState(GameState a_targetState)
    {
        // Don't change to target state if that is already the current state
        if (currentState != a_targetState)
        {
            previousState = currentState;
            currentState = a_targetState;

            stateHistory.Add(currentState);

            if (OnGameStateChanged != null)
            {
                OnGameStateChanged(previousState, currentState);
            }

            //Debug.Log(string.Format("Changed from {0} to {1}", StateToString(previousState), StateToString(a_targetState)));
        }
        else
        {
            // Debug.Log(string.Format("Tried to change to {0} but that's already the current state", StateToString(a_targetState)));
        }
    }

    // Changes game state to transition and then change the game state after a delay in seconds
    public void ChangeGameStateAfterDelay(GameState a_targetState, float a_delay)
    {
        StartCoroutine(DelayedStateChange(a_targetState, a_delay));
    }

    // Change the game state during a blackout of the screen
    public void ChangeStateAfterFade(GameState a_targetState)
    {
        // UI will fade to black when the state is changed to loading and will fade back once the new state is set
        ChangeGameState(GameState.loading);

        StartCoroutine(ChangeMidLoad(a_targetState));
    }

    // Returns the string version of the state
    public static string StateToString(GameState a_state)
    {
        switch (a_state)
        {
            case GameState.battle:
                return "Battle";
            case GameState.cinematic:
                return "Cinematic";
            case GameState.loading:
                return "Loading";
            case GameState.mainmenu:
                return "Main menu";
            case GameState.overworld:
                return "Over world";
            default:
                return "unknown state";
        }
    }

    #endregion

    #region Local Methods

    private IEnumerator ChangeMidLoad(GameState a_targetState)
    {
        // The UI is fading to black and will set midLoad to true once the screen is black
        yield return new WaitUntil(() => midLoad);

        // the reason it waits is that other scripts could also be waiting for midLoad to be true
        yield return new WaitForSeconds(0.1f);

        midLoad = false;

        // can now change state since the screen is black, the UI will fade from black at this point as well

        ChangeGameState(a_targetState);
    }

    private IEnumerator DelayedStateChange(GameState a_targetState, float a_delay)
    {
        // next state is needed by scripts looking for the state after a transition
        stateBeforeTransition = currentState;
        stateAfterTransition = a_targetState;
        ChangeGameState(GameState.transition);

        yield return new WaitForSeconds(a_delay);

        ChangeGameState(a_targetState);
    }

    #endregion
}