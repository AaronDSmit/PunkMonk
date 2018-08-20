using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoringInfo
{
    private GridManager grid;
    private AI_Agent agent;
    private PlayerInfo[] playerInfo;

    private struct PlayerInfo
    {
        public Unit player;

        public List<Hex> path;

        public int movesToAttackRange;

        public PlayerInfo(Unit a_player, List<Hex> a_path, int a_movesToAttackRange)
        {
            player = a_player;
            path = a_path;
            movesToAttackRange = a_movesToAttackRange;
        }

    }

    ScoringInfo(GridManager a_grid, AI_Agent a_agent, Unit[] a_players)
    {
        if (a_grid == null || a_agent == null || a_players == null || a_players[0] == null || a_players[1] == null)
        {
            Debug.LogError("ScoringInfo was given a null value in its constructor.");
        }

        grid = a_grid;
        agent = a_agent;
        playerInfo = new PlayerInfo[2];

        for (int i = 0; i < 2; i++)
        {
            List<Hex> path = ClacPathToAttackRange(grid, agent, a_players[i]);

            playerInfo[i] = new PlayerInfo(a_players[i], path, ClacMovesToAttackRange(a_players[i], path));
        }
    }

    static int ClacMovesToAttackRange(Unit a_unit, List<Hex> a_path)
    {
        return a_path.Count / a_unit.MoveRange;
    }

    static List<Hex> ClacPathToAttackRange(GridManager a_grid, Unit a_fromUnit, Unit a_toUnit)
    {
        // TODO: Check if this agent is already in range of the player

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
