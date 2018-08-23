using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Coordinates the AI_Agents to take their turns one at a time 
/// </summary>
public class AI_Controller : MonoBehaviour
{
    #region Unity Inspector Fields

    [Tooltip("The highlighted colour of the unit that is currently doing its move")]
    [SerializeField]
    private Color currentUnitColour = Color.black;

    [Tooltip("The time in seconds at the start of the turn before making decisions")]
    [SerializeField]
    private float delayBeforeStarting = 1.0f;

    [Tooltip("The time in seconds at the end of the turn before ending the turn")]
    [SerializeField]
    private float delayBeforeEnding = 1.0f;

    #endregion

    #region Reference Fields

    private GridManager grid = null;
    private TurnManager turnManager = null;
    private Unit[] players = null;

    #endregion

    #region Local Fields

    private List<AI_Agent> agents = new List<AI_Agent>();

    public static AI_Controller instance;

    #endregion

    #region Properties

    /// <summary>
    /// The AI_Agents in the game
    /// </summary>
    public List<AI_Agent> Agents { get { return agents; } }

    #endregion

    #region Public Methods

    /// <summary>
    /// Used to add a AI_Agent that just spawned
    /// </summary>
    /// <param name="a_newAgent">The agent that should be added</param>
    public Unit[] AddUnit(AI_Agent a_newAgent)
    {
        agents.Add(a_newAgent);
        a_newAgent.OnDeath += RemoveUnit;
        return players;
    }

    /// <summary>
    /// Used to initialize the instance of this class
    /// </summary>
    public void Init(Unit a_player1, Unit a_player2)
    {
        players = new Unit[] { a_player1, a_player2 };
        grid = GameObject.FindGameObjectWithTag("Grid").GetComponent<GridManager>();
    }

    #endregion

    #region Unity Life-cycle Methods

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            turnManager = Manager.instance.TurnController;
            turnManager.AITurnEvent += TurnEvent;
        }
        else
        {
            Debug.LogError("More than 1 AI_Controller is in the scene, deleting it");

            //stop the game from having more than one AI_Controller
            Destroy(gameObject);
            return;
        }
    }

    private void OnDestroy()
    {
        turnManager.AITurnEvent -= TurnEvent;
    }

    #endregion

    #region Local Methods

    private void TurnEvent(TurnManager.TurnState a_newState, int a_turnNumber)
    {
        // Check if it is the AI's turn
        if (a_newState == TurnManager.TurnState.start)
        {
            // Refresh all units
            foreach (Unit unit in agents)
                unit.Refresh();

            StartCoroutine(DoTurn());
        }
    }

    private IEnumerator DoTurn()
    {
        yield return new WaitForSeconds(delayBeforeStarting);

        foreach (AI_Agent agent in agents)
        {
            agent.Highlight(true, currentUnitColour);
            agent.StartTurn(grid);

            yield return new WaitUntil(() => agent.TurnComplete);

            agent.Highlight(false, currentUnitColour);
        }

        yield return new WaitForSeconds(delayBeforeEnding);

        EndTurn();
    }

    private void EndTurn()
    {
        turnManager.EndTurn(TEAM.ai);
    }

    private void RemoveUnit(LivingEntity a_unit)
    {
        agents.Remove((AI_Agent)a_unit);
    }

    #endregion
}