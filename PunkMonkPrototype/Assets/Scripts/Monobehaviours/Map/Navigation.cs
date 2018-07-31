using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Navigation : MonoBehaviour
{
    public static List<Tile> FindPath(Tile starting, Tile target)
    {
        List<Tile> openSet = new List<Tile>();
        HashSet<Tile> closedSet = new HashSet<Tile>();

        openSet.Add(starting);

        while (openSet.Count > 0)
        {
            Tile current = openSet[0];
            current.HScore = Tile.Distance(current, target);
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].FScore < current.FScore || (openSet[i].FScore == current.FScore && openSet[i].FScore < current.FScore))
                {
                    current = openSet[i];
                }
            }

            closedSet.Add(current);
            openSet.Remove(current);

            if (current == target)
            {
                foreach (Tile node in closedSet)
                {
                    node.GScore = 0;
                }

                return RetracePath(starting, target);
            }

            foreach (Tile neighbour in current.Neighbours)
            {
                if (!neighbour.IsWalkable || closedSet.Contains(neighbour))
                {
                    continue;
                }

                float newMovemntCostToNeighour = current.GScore + Tile.Distance(current, neighbour);
                if (newMovemntCostToNeighour < neighbour.GScore || !openSet.Contains(neighbour))
                {
                    neighbour.GScore = newMovemntCostToNeighour;
                    neighbour.HScore = Tile.Distance(neighbour, target);
                    neighbour.Parent = current;

                    if (!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                    }
                }
            }
        }

        return null;
    }

    public static int PathLength(Tile a_starting, Tile a_target)
    {
        List<Tile> path = FindPath(a_starting, a_target);

        if (path != null)
        {
            return path.Count;
        }

        return 0;
    }

    private static List<Tile> RetracePath(Tile start, Tile end)
    {
        List<Tile> path = new List<Tile>();
        Tile current = end;

        while (current != start)
        {
            path.Add(current);
            current = current.Parent;
        }

        path.Reverse();

        return path;
    }
}
