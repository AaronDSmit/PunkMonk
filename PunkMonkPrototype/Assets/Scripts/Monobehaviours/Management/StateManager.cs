using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { mainmenu, overworld, battle, loading, cinematic, pausemenu }

/// <summary>
/// This script is used to handle state changes
/// </summary>
public class StateManager : MonoBehaviour
{
    #region Unity Inspector Fields

    [Header("Debug Info")]
    [SerializeField]
    private GameState currentState = GameState.mainmenu;

    [SerializeField]
    private GameState previousState = GameState.pausemenu;

    [SerializeField]
    private List<GameState> stateHistory;

    #endregion

    #region Local Fields

    private bool midLoad = false;

    private bool isReady;

    public delegate void Game_stateChanged(GameState _oldState, GameState _newState);
    public event Game_stateChanged OnGameStateChanged;

    #endregion

    #region Properties

    public bool MidLoad
    {
        set { midLoad = value; }
    }

    public bool Ready
    {
        get { return isReady; }
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

    // This function changes the current game state to the target game state and calls the event on any script that is listening
    public void ChangeGameState(GameState a_targetState)
    {
        // Don't change to target state if that is already the current state
        if (currentState != a_targetState)
        {
            previousState = currentState;
            currentState = a_targetState;

            stateHistory.Add(currentState);

            if (currentState == GameState.pausemenu)
            {
                Time.timeScale = 0.0f;
            }

            if (previousState == GameState.pausemenu)
            {
                Time.timeScale = 1.0f;
            }

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

    public void ChangeStateAfterFade(GameState a_targetState)
    {
        ChangeGameState(GameState.loading);

        StartCoroutine(ChangeMidLoad(a_targetState));
    }

    // Returns the string version of the state
    public static string StateToString(GameState a_state)
    {
        switch (a_state)
        {
            case GameState.pausemenu:
                return "Pause menu";
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
        yield return new WaitUntil(() => midLoad);

        midLoad = false;

        ChangeGameState(a_targetState);
    }

    #endregion
}