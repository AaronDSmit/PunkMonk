using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Controller : MonoBehaviour
{
    [SerializeField] private Color currentUnitColour = Color.black;
    [SerializeField] private Color attackingUnitColour = Color.black;

    private GridManager grid = null;
    private TurnManager turnManager = null;
    private Unit[] players = null;

    private List<AI_Agent> agents = new List<AI_Agent>();

    public List<AI_Agent> Agents { get { return agents; } }

    public Unit[] AddUnit(AI_Agent a_newAgent)
    {
        agents.Add(a_newAgent);
        a_newAgent.OnDeath += RemoveUnit;
        return players;
    }

    public void Init(Unit a_player1, Unit a_player2)
    {
        players = new Unit[] { a_player1, a_player2 };
        grid = GameObject.FindGameObjectWithTag("Grid").GetComponent<GridManager>();
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
            // Refresh all units
            foreach (Unit unit in agents)
                unit.Refresh();

            StartCoroutine(DoTurn());
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

    private void RemoveUnit(LivingEntity a_unit)
    {
        agents.Remove((AI_Agent)a_unit);
    }
}
