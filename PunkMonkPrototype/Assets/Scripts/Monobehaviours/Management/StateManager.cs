using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Game_state { mainmenu, overworld, battle }
public enum Turn_state { start, end }
public enum TEAM { player, ai, neutral }

public class StateManager : MonoBehaviour
{
    [SerializeField] private Game_state currentState;

    [SerializeField] private TEAM startingTeam;

    [SerializeField] private TEAM currentTeam;

    private int turnCount;

    public delegate void TurnStateChanged(Turn_state newState, TEAM team, int turnNumber);
    public static event TurnStateChanged TurnEvent;

    private void Start()
    {
        currentTeam = startingTeam;

        currentState = Game_state.battle;

        StartTurn();
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
}