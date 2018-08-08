﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Agent : Unit
{
    [SerializeField] private float damage = 100;
    [SerializeField] private float damgeDelayTimer = 0;

    private AI_Controller ai_Controller = null;

    private Unit[] players = null;

    private bool turnComplete = false;

    private bool isPerformingAction = true;

    private Hex[] tilesToAttack = null;

    public float Damage { get { return damage; } }
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

    protected override void DoBasicAttack(Hex[] a_targetTiles, System.Action a_start, System.Action a_finished)
    {
        tilesToAttack = a_targetTiles;

        StartCoroutine(BasicAttackDamageDelay(damgeDelayTimer, a_finished));
    }

    private IEnumerator DoTurn(GridManager a_grid)
    {
        List<Hex>[] shortestPaths = new List<Hex>[] { null, null };

        for (int i = 0; i < 2; i++)
        {
            Unit currentPlayer = players[i];
            // Find all of the tiles within attack range
            List<Hex> tiles = new List<Hex>(a_grid.GetTilesWithinDistance(currentPlayer.CurrentTile, attackRange, true));
            foreach (Hex tile in tiles)
            {
                // Ignore the tile that the player is on
                if (tile == currentPlayer.CurrentTile)
                    continue;
                // TODO: Ignore the tiles that can't attack the player

                // Pathfind to the tile
                List<Hex> path = Navigation.FindPath(tile, currentPlayer.CurrentTile);
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

        // Choose the closest player
        int closestPlayer = shortestPaths[0].Count <= shortestPaths[1].Count ? 0 : 1;

        // Path find and wait for it to finish
        isPerformingAction = true;
        finishedWalking = FinishedAction;
        StartCoroutine(Walk(shortestPaths[closestPlayer]));
        yield return new WaitUntil(() => isPerformingAction == false);

        // Attack the player, checking if it is in range // TODO check if possible
        if (HexUtility.Distance(currentTile, players[closestPlayer].CurrentTile) <= attackRange)
        {
            // Attack the player
            isPerformingAction = true;
            BasicAttack(new Hex[] { players[closestPlayer].CurrentTile }, null, FinishedAction);
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
                    tile.CurrentUnit.TakeDamage(Element.earth, damage);
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
}