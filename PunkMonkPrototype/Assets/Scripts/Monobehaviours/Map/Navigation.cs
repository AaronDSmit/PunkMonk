using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Navigation : MonoBehaviour
{
    //public List<Tile> FindPath(Tile starting, Tile target)
    //{
    //    List<NavNode> openSet = new List<NavNode>();
    //    HashSet<NavNode> closedSet = new HashSet<NavNode>();

    //    openSet.Add(starting.Node);

    //    while (openSet.Count > 0)
    //    {
    //        NavNode current = openSet[0];
    //        current.HScore = Tile.Distance(current.Tile, target);
    //        for (int i = 1; i < openSet.Count; i++)
    //        {
    //            if (openSet[i].FScore < current.FScore || (openSet[i].FScore == current.FScore && openSet[i].FScore < current.FScore))
    //            {
    //                current = openSet[i];
    //            }
    //        }

    //        closedSet.Add(current);
    //        openSet.Remove(current);

    //        if (current.Tile == target)
    //        {
    //            foreach (NavNode node in closedSet)
    //            {
    //                node.GScore = 0;
    //            }

    //            return RetracePath(starting, target);
    //        }

    //        foreach (NavNode neighbour in current.Neighbours)
    //        {
    //            if (!neighbour.IsWalkable || closedSet.Contains(neighbour))
    //            {
    //                continue;
    //            }

    //            float newMovemntCostToNeighour = current.GScore + Tile.Distance(current.Tile, neighbour.Tile);
    //            if (newMovemntCostToNeighour < neighbour.GScore || !openSet.Contains(neighbour))
    //            {
    //                neighbour.GScore = newMovemntCostToNeighour;
    //                neighbour.HScore = Tile.Distance(neighbour.Tile, target);
    //                neighbour.Parent = current;

    //                if (!openSet.Contains(neighbour))
    //                {
    //                    openSet.Add(neighbour);
    //                }
    //            }
    //        }
    //    }



    //    return null;
    //}

    //private List<Tile> RetracePath(Tile start, Tile end)
    //{
    //    List<Tile> path = new List<Tile>();
    //    NavNode current = end.Node;

    //    while (current.Tile != start)
    //    {
    //        path.Add(current.Tile);
    //        current = current.Parent;
    //    }

    //    path.Reverse();

    //    return path;
    //}
}
