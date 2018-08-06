using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TEAM { player, ai, neutral }
public enum Turn_state { start, end }

public class TurnManager : MonoBehaviour
{
    [SerializeField] private TEAM startingTeam;

    [SerializeField] private TEAM currentTeam;

    private int turnCount;

    private bool isReady;

    public delegate void TurnStateChanged(Turn_state newState, TEAM team, int turnNumber);
    public static event TurnStateChanged TurnEvent;

    private void Awake()
    {
        StateManager.OnGameStateChanged += GameStateChanged;
    }

    private void OnDisable()
    {
        StateManager.OnGameStateChanged -= GameStateChanged;
    }

    private void GameStateChanged(Game_state _oldstate, Game_state _newstate)
    {
        // ensure this script knows it's in over-world state
        if (_newstate == Game_state.battle)
        {
            StartTurn();
        }
    }

    public void Init()
    {
        currentTeam = startingTeam;
        turnCount = 0;

        isReady = true;
    }

    private void StartTurn()
    {
        turnCount++;

        if (TurnEvent != null)
        {
            TurnEvent(Turn_state.start, currentTeam, turnCount);
        }
    }

    public void EndTurn()
    {
        if (TurnEvent != null)
        {
            TurnEvent(Turn_state.end, currentTeam, turnCount);
        }

        currentTeam = (currentTeam == TEAM.player) ? TEAM.ai : TEAM.player;

        StartTurn();
    }

    public bool Ready
    {
        get { return isReady; }
    }
}