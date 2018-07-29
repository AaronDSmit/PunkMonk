using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Game_state { mainmenu, overworld, battle, loading, cinematic, pausemenu }

public class StateManager : MonoBehaviour
{
    [SerializeField] private Game_state currentState = Game_state.mainmenu;

    [SerializeField] private Game_state previousState = Game_state.pausemenu;

    [SerializeField] private List<Game_state> stateHistory;

    private bool isReady;

    // events
    public delegate void Game_stateChanged(Game_state _oldState, Game_state _newState);
    public static event Game_stateChanged OnGameStateChanged;

    #region public Functions

    public void Init()
    {
        stateHistory = new List<Game_state>();

        ChangeGame_state(Game_state.loading);

        isReady = true;
    }

    public void StartGame()
    {
        ChangeGame_state(Game_state.overworld);
    }

    // This function changes the current game state to the target game state and calls the event on any script that is listening
    public void ChangeGame_state(Game_state a_targetState)
    {
        // Don't change to target state if that is already the current state
        if (currentState != a_targetState)
        {
            previousState = currentState;
            currentState = a_targetState;

            stateHistory.Add(currentState);

            if (currentState == Game_state.pausemenu)
            {
                Time.timeScale = 0.0f;
            }

            if (previousState == Game_state.pausemenu)
            {
                Time.timeScale = 1.0f;
            }

            if (OnGameStateChanged != null)
            {
                OnGameStateChanged(previousState, currentState);
            }

            Debug.Log(string.Format("Changed from {0} to {1}", StateToString(previousState), StateToString(a_targetState)));
        }
        else
        {
            Debug.Log(string.Format("Tried to change to {0} but that's already the current state", StateToString(a_targetState)));
        }
    }

    // Returns the string version of the state
    public static string StateToString(Game_state a_state)
    {
        switch (a_state)
        {
            case Game_state.pausemenu:
                return "pause menu";
            case Game_state.battle:
                return "battle";
            case Game_state.cinematic:
                return "cinematic";
            case Game_state.loading:
                return "loading";
            case Game_state.mainmenu:
                return "main menu";
            case Game_state.overworld:
                return "over world";
            default:
                return "unknown state";
        }
    }

    public bool Ready
    {
        get { return isReady; }
    }

    // Returns previous game state
    public Game_state PreviousState
    {
        get { return previousState; }
    }

    // Returns current game state
    public Game_state CurrentGameState
    {
        get { return currentState; }
    }

    #endregion
}