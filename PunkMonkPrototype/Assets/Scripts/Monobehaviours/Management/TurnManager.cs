using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Needs to be all caps to avoid conflict with Properties
public enum TEAM { player, ai, neutral }
public enum TurnState { start, end }

/// <summary>
/// This script is used to handle turn changes. Turns have two phases, start and end.
/// </summary>
public class TurnManager : MonoBehaviour
{
    #region Unity Inspector Fields

    [SerializeField] private TEAM startingTeam;

    [Header("Debug Info")]
    [SerializeField]
    private TEAM currentTeam;

    #endregion

    #region Local Fields

    private int turnCount;

    private bool isReady;

    public delegate void TurnStateChanged(TurnState newState, TEAM team, int turnNumber);
    public event TurnStateChanged TurnEvent;

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
        currentTeam = startingTeam;
        turnCount = 0;

        isReady = true;
    }

    public void EndTurn()
    {
        if (TurnEvent != null)
        {
            TurnEvent(TurnState.end, currentTeam, turnCount);
        }

        currentTeam = (currentTeam == TEAM.player) ? TEAM.ai : TEAM.player;

        StartTurn();
    }

    #endregion

    #region Unity Life-cycle Methods

    private void Start()
    {
        Manager.instance.StateController.OnGameStateChanged += GameStateChanged;
    }

    #endregion

    #region Local Methods

    private void GameStateChanged(GameState _oldstate, GameState _newstate)
    {
        // ensure this script knows it's in over-world state
        if (_newstate == GameState.battle)
        {
            StartTurn();
        }
        else
        {
            turnCount = 0;
        }
    }

    private void StartTurn()
    {
        turnCount++;

        if (TurnEvent != null)
        {
            TurnEvent(TurnState.start, currentTeam, turnCount);
        }
    }

    #endregion
}