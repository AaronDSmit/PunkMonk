using UnityEngine;

// Needs to be all caps to avoid conflict with Properties
public enum TEAM { player, ai, neutral }

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

    public enum TurnState { start, end }

    public delegate void TurnStateChanged(TurnState a_nextState, int a_turnNumber);
    public event TurnStateChanged PlayerTurnEvent;
    public event TurnStateChanged AITurnEvent;
    public event TurnStateChanged EveryTurnEvent;

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

    public void EndTurn(TEAM a_endingTeam)
    {
        if (currentTeam == a_endingTeam)
        {
            if (currentTeam == TEAM.ai)
            {
                if (AITurnEvent != null)
                {
                    AITurnEvent(TurnState.end, turnCount);
                }
            }
            else if (currentTeam == TEAM.player)
            {
                if (PlayerTurnEvent != null)
                {
                    PlayerTurnEvent(TurnState.end, turnCount);
                }
            }

            currentTeam = (currentTeam == TEAM.player) ? TEAM.ai : TEAM.player;

            StartTurn();
        }
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

        if (currentTeam == TEAM.ai)
        {
            if (AITurnEvent != null)
            {
                AITurnEvent(TurnState.start, turnCount);
            }
        }
        else if (currentTeam == TEAM.player)
        {
            if (PlayerTurnEvent != null)
            {
                PlayerTurnEvent(TurnState.start, turnCount);
            }
        }

        if (EveryTurnEvent != null)
        {
            EveryTurnEvent(TurnState.start, turnCount);
        }
    }

    #endregion
}