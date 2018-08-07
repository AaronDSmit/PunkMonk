using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Controller : MonoBehaviour
{
    private int currentAgentsTurn = 0;

    private bool myTurn = true;

    private List<AI_Agent> agents = new List<AI_Agent>();

    public List<AI_Agent> Agents { get { return agents; } }

    public void AddUnit(AI_Agent a_newAgent)
    {
        agents.Add(a_newAgent);
    }

    private void Awake()
    {
        Manager.instance.TurnController.TurnEvent += TurnEvent;
    }

    private void Update()
    {
        if (myTurn)
        {

        }
    }

    private void TurnEvent(Turn_state a_newState, TEAM a_team, int a_turnNumber)
    {
        // Check if it is the AI's turn
        if (a_newState == Turn_state.start && a_team == TEAM.ai)
        {
            StartTurn();

            // Refresh all units
            foreach (Unit unit in agents)
                unit.Refresh();
        }
    }

    private void StartTurn()
    {
        myTurn = true;
        currentAgentsTurn = 0;
    }

    private void EndTurn()
    {
        
    }

}
