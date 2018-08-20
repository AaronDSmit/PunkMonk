using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class AI_Agent : Unit
{
    #region Unity Inspector Fields

    [SerializeField] private float damage = 100;
    [SerializeField] private float damgeDelayTimer = 0;

    #endregion

    #region Reference Fields

    private AI_Controller ai_Controller = null;

    private Unit[] players = null;

    #endregion

    #region Local Fields

    private bool turnComplete = false;

    private bool isPerformingAction = true;

    private Hex[] tilesToAttack = null;

    #endregion

    #region Properties

    public float Damage { get { return damage; } }
    public bool TurnComplete { get { return turnComplete; } }

    #endregion

    #region Public Methods

    public override void Spawn(Hex a_startingTile)
    {
        base.Spawn(a_startingTile);

        // Get the AI Controller and tell it that this agent was spawned
        ai_Controller = GameObject.FindGameObjectWithTag("AI_Controller").GetComponent<AI_Controller>();
        players = ai_Controller.AddUnit(this);
    }

    public void StartTurn(GridManager a_grid)
    {
        StartCoroutine(DoTurn(a_grid));
    }

    #endregion

    #region Unity Life-cycle Methods

    protected override void Awake()
    {
        base.Awake();

        Manager.instance.StateController.OnGameStateChanged += GameStateChanged;

        // Setup the actions
        foreach (AI_Action action in actions)
        {
            action.Init(this);
        }
    }

    #endregion

    #region Local Methods

    protected override void DoBasicAttack(Hex[] a_targetTiles, System.Action a_start, System.Action a_finished)
    {
        tilesToAttack = a_targetTiles;

        StartCoroutine(BasicAttackDamageDelay(damgeDelayTimer, a_finished));
    }

    struct PathInfo
    {
        public bool isAlreadyInRange;
        public List<Hex> path;

        public void Init()
        {
            isAlreadyInRange = false;
            path = null;
        }

    }

    private IEnumerator DoTurn(GridManager a_grid)
    {

        List<Hex>[] shortestPaths = new List<Hex>[] { null, null };

        for (int i = 0; i < 2; i++)
        {
            Unit currentPlayer = players[i];

            // TODO: Check if this agent is already in range of the player


            // Ignore dead players
            if (currentPlayer.IsDead)
                continue;

            // Find all of the tiles within attack range
            List<Hex> tiles = new List<Hex>(a_grid.GetTilesWithinDistance(currentPlayer.CurrentTile, attackRange, false));
            foreach (Hex targetTile in tiles)
            {
                // Ignore the tile that the player is on
                if (targetTile.IsTraversable == false)
                {
                    if (targetTile == currentTile)
                        shortestPaths[i] = new List<Hex> { currentTile };

                    continue;
                }

                // TODO: Ignore the tiles that can't attack the player
                //if (HasClearShot(tile, players[i]) == false)
                //    continue;
                // Pathfind to the tile
                List<Hex> path = Navigation.FindPath(CurrentTile, targetTile);
                if (path == null)
                {
                    Debug.LogError("No path found", gameObject);
                    continue;
                }

                if (shortestPaths[i] == null) // Check if this is the first path
                {
                    shortestPaths[i] = path;
                }
                // Set it as the shortest path if it is shorter than the current shortest
                else if (path.Count < shortestPaths[i].Count)
                {
                    shortestPaths[i] = path;
                }
            }
        }

        // Check for the shortest paths being null
        if (shortestPaths[0] == null && shortestPaths[1] == null)
        {
            turnComplete = true;
            Debug.LogError("No path found to players", gameObject);
            yield break;
        }

        int playerToAttack = -1;
        if (shortestPaths[0] == null)
            playerToAttack = 1;
        else if (shortestPaths[1] == null)
            playerToAttack = 0;
        else
            // Choose the closest player
            playerToAttack = shortestPaths[0].Count <= shortestPaths[1].Count ? 0 : 1;

        // Remove the path that is out of the movement range
        if (shortestPaths[playerToAttack].Count > MoveRange)
        {
            shortestPaths[playerToAttack].RemoveRange(MoveRange, shortestPaths[playerToAttack].Count - MoveRange);
        }

        // Path find and wait for it to finish
        isPerformingAction = true;
        finishedWalking = FinishedAction;
        StartCoroutine(Walk(shortestPaths[playerToAttack]));
        yield return new WaitUntil(() => isPerformingAction == false);

        // Attack the player, checking if it is in range && if we have a clear shot
        if (HexUtility.Distance(currentTile, players[playerToAttack].CurrentTile) <= attackRange)// && HasClearShot(this, players[playerToAttack]))
        {
            isPerformingAction = true;
            BasicAttack(new Hex[] { players[playerToAttack].CurrentTile }, null, FinishedAction);
            yield return new WaitUntil(() => isPerformingAction == false);
        }

        turnComplete = true;
    }

    private IEnumerator BasicAttackDamageDelay(float a_timer, System.Action a_finished)
    {
        //wait for timer before runing code
        yield return new WaitForSeconds(a_timer);

        //go though each tile and deal damage to the enemy
        foreach (Hex tile in tilesToAttack)
        {
            //if there is a unit
            if (tile.CurrentUnit != null)
            {
                //make sure we arent damaging other ai
                if (tile.CurrentUnit.Team != TEAM.ai)
                {
                    //deal damage to that unit
                    tile.CurrentUnit.TakeDamage(damage);
                }
            }
        }
        //call the finished call back function
        a_finished();
    }

    private void FinishedAction()
    {
        isPerformingAction = false;
    }

    public override void Refresh()
    {
        base.Refresh();

        turnComplete = false;
    }

    private void GameStateChanged(GameState a_oldstate, GameState a_newstate)
    {
        if (a_oldstate == GameState.battle && a_newstate != GameState.battle)
        {
            Die();
        }
    }

    #endregion
}
