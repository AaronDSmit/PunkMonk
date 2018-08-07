using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Navigation : MonoBehaviour
{
    public static List<Hex> FindPath(Hex a_starting, Hex a_target)
    {
        List<Hex> openSet = new List<Hex>();
        HashSet<Hex> closedSet = new HashSet<Hex>();

        openSet.Add(a_starting);

        while (openSet.Count > 0)
        {
            Hex current = openSet[0];
            current.HScore = HexUtility.Distance(current, a_target);
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].FScore < current.FScore || (openSet[i].FScore == current.FScore && openSet[i].FScore < current.FScore))
                {
                    current = openSet[i];
                }
            }

            closedSet.Add(current);
            openSet.Remove(current);

            if (current == a_target)
            {
                foreach (Hex node in closedSet)
                {
                    node.GScore = 0;
                }

                return RetracePath(a_starting, a_target);
            }

            foreach (Hex neighbour in current.Neighbours)
            {
                if (!neighbour.IsTraversable || closedSet.Contains(neighbour))
                {
                    continue;
                }

                float newMovemntCostToNeighour = current.GScore + HexUtility.Distance(current, neighbour);
                if (newMovemntCostToNeighour < neighbour.GScore || !openSet.Contains(neighbour))
                {
                    neighbour.GScore = newMovemntCostToNeighour;
                    neighbour.HScore = HexUtility.Distance(neighbour, a_target);
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

    public static int PathLength(Hex a_starting, Hex a_target)
    {
        List<Hex> path = FindPath(a_starting, a_target);

        if (path != null)
        {
            return path.Count;
        }

        return 0;
    }

    private static List<Hex> RetracePath(Hex a_start, Hex a_end)
    {
        List<Hex> path = new List<Hex>();
        Hex current = a_end;

        while (current != a_start)
        {
            path.Add(current);
            current = current.Parent;
        }

        path.Reverse();

        return path;
    }
}
