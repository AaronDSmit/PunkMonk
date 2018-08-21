using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoringInfo
{
    private GridManager grid;
    private AI_Agent agent;
    private Unit player;
    private bool isInAttackRange;
    private List<Hex> path;
    private int movesToAttackRange;

    public GridManager Grid { get { return grid; } }
    public AI_Agent Agent { get { return agent; } }
    public Unit Player { get { return player; } }
    public bool IsInAttackRange { get { return isInAttackRange; } }
    public List<Hex> Path { get { return path; } }
    public int MovesToAttackRange { get { return movesToAttackRange; } }

    public ScoringInfo(GridManager a_grid, AI_Agent a_agent, Unit a_player)
    {
        if (a_grid == null || a_agent == null || a_player == null)
        {
            Debug.LogError("ScoringInfo was given a null value in its constructor.");
        }

        grid = a_grid;
        agent = a_agent;
        player = a_player;

        path = ClacPathToAttackRange(grid, agent, player);
        isInAttackRange = path != null && path.Count == 1 && path[0] == agent.CurrentTile;

        movesToAttackRange = path == null ? int.MinValue : ClacMovesToAttackRange(player, path);
    }

    private static int ClacMovesToAttackRange(Unit a_unit, List<Hex> a_path)
    {
        return a_path.Count / a_unit.MoveRange;
    }

    private static List<Hex> ClacPathToAttackRange(GridManager a_grid, Unit a_fromUnit, Unit a_toUnit)
    {

        List<Hex> shortestPath = null;

        // Ignore dead players
        if (a_toUnit.IsDead)
            return null;

        // Find all of the tiles within attack range
        List<Hex> tiles = new List<Hex>(a_grid.GetTilesWithinDistance(a_toUnit.CurrentTile, a_fromUnit.AttackRange, false));
        foreach (Hex targetTile in tiles)
        {
            // Ignore the tile that the player is on
            if (targetTile.IsTraversable == false)
            {
                if (targetTile == a_fromUnit.CurrentTile)
                    shortestPath = new List<Hex> { a_fromUnit.CurrentTile };

                continue;
            }

            // TODO: Ignore the tiles that can't attack the player
            //if (HasClearShot(tile, players[i]) == false)
            //    continue;
            // Pathfind to the tile
            List<Hex> path = Navigation.FindPath(a_fromUnit.CurrentTile, targetTile);
            if (path == null)
            {
                //Debug.LogError("No path found", a_agent.gameObject);
                continue;
            }

            if (shortestPath == null) // Check if this is the first path
            {
                shortestPath = path;
            }
            // Set it as the shortest path if it is shorter than the current shortest
            else if (path.Count < shortestPath.Count)
            {
                shortestPath = path;
            }
        }


        return shortestPath;
    }
}
