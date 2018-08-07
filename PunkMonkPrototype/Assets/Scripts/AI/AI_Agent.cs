using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Agent : Unit
{
    [SerializeField] private float damage;
    [SerializeField] private float movement;

    private AI_Controller ai_Controller = null;

    private Unit[] players = null;

    private bool turnComplete = false;

    public float Damage { get { return damage; } }
    public float Movement { get { return movement; } }
    public bool TurnComplete { get { return turnComplete; } }

    protected override void Awake()
    {
        base.Awake();

        // Setup the actions
        foreach (AI_Action action in actions)
        {
            action.Init(this);
        }

    }


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

    private IEnumerator DoTurn(GridManager a_grid)
    {
        // TODO: Implement agents turn

        List<Hex> shortestPath1;
        List<Hex> shortestPath2;

        for (int i = 0; i < 2; i++)
        {
            Unit currentPlayer = players[i];
            // Find all of the tiles within attack range
            //List<Hex> tiles = a_grid.GetTilesWithinDistance(currentPlayer.CurrentTile, 3);
            // Remove the tiles that can't attack the player
            // For each tile:
            // Pathfind to the tile
            // Set it as the shortest path if it is shorter than the current shortest (or use a list of shortest, add it to the list if it is the same distance)
        }

        // Path find to the closest tile considering each player
        // Attack the player if possible


        turnComplete = true;
        yield return null;
    }
}
