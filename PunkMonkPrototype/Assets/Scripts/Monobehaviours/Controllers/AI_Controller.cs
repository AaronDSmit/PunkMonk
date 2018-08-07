using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Controller : MonoBehaviour
{
    [SerializeField] private Color currentUnitColour;
    [SerializeField] private Color attackingUnitColour;

    private GridManager grid = null;
    private TurnManager turnManager = null;
    private Unit[] players = null;

    private List<AI_Agent> agents = new List<AI_Agent>();

    public List<AI_Agent> Agents { get { return agents; } }

    public Unit[] AddUnit(AI_Agent a_newAgent)
    {
        agents.Add(a_newAgent);

        return players;
    }

    public void Init(Unit a_player1, Unit a_player2, GridManager a_gridManager)
    {
        players = new Unit[] { a_player1, a_player2 };
        grid = a_gridManager;
    }

    private void Awake()
    {
        turnManager = Manager.instance.TurnController;
        turnManager.TurnEvent += TurnEvent;
    }

    private void TurnEvent(Turn_state a_newState, TEAM a_team, int a_turnNumber)
    {
        // Check if it is the AI's turn
        if (a_newState == Turn_state.start && a_team == TEAM.ai)
        {
            StartCoroutine(DoTurn());

            // Refresh all units
            foreach (Unit unit in agents)
                unit.Refresh();
        }
    }

    private IEnumerator DoTurn()
    {
        foreach (AI_Agent agent in agents)
        {
            agent.Highlight(true, currentUnitColour);
            agent.StartTurn(grid);

            yield return new WaitUntil(() => agent.TurnComplete);

            agent.Highlight(false, currentUnitColour);
        }

        EndTurn();
    }

    private void EndTurn()
    {
        turnManager.EndTurn();
    }

}
