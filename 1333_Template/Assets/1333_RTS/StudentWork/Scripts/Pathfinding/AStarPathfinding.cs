using System.Collections;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using NUnit;
using Unity.Hierarchy;
using UnityEditor.TerrainTools;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;
using UnityEngine.XR;

public class AStarPathfinding : PathFindingAlgorithm
{
    private GridManager gridManager;
    public GridManager _GridManager => gridManager;
    
    public AStarPathfinding (GridManager grid)
    {
        gridManager = grid;
    }

    public override List<GridNode> Findpath(GridNode start, GridNode end)
    {

        List<GridNode> openSet = new List<GridNode>();
        HashSet<GridNode> closedSet = new HashSet<GridNode>();
        gridManager.Front.Clear();
        gridManager.Visited.Clear();
        gridManager.Path.Clear();
        
        openSet.Add(start);
        gridManager.Front.Add(start);

        start.GCost = 0;
        start.HCost = GetHeuristic(start, end);
        start.Parent = null;
        
        while (openSet.Count > 0)
        {
            GridNode current = GetLowestFCost(openSet);
            openSet.Remove(current);
            gridManager.Front.Remove(current);
            closedSet.Add(current);
            gridManager.Visited.Add(current);

            if(current == end)
            {
                return ReconstructPath(start, end);
            }
            foreach (GridNode neighbor in gridManager.GetNeighbors(current))
            {
                if (closedSet.Contains(neighbor)) continue;
                int tentativeGCost = current.GCost + neighbor.Weight;
                if (!openSet.Contains(neighbor))
                {
                    neighbor.GCost = tentativeGCost;
                    neighbor.HCost = GetHeuristic(neighbor, end);
                    neighbor.Parent = current;

                    openSet.Add(neighbor);
                    gridManager.Front.Add(neighbor);
                }
                else if(tentativeGCost < neighbor.GCost)
                {
                    neighbor.GCost = tentativeGCost;
                    neighbor.Parent = current;
                }
            }
        }
        return null;
    }



    public override List<GridNode> Findpath(Vector3 startWorld, Vector3 endWorld)
    {
        return Findpath(gridManager.GetNodeFromWorldPosition(startWorld), gridManager.GetNodeFromWorldPosition(endWorld));
    }

    private int GetHeuristic(GridNode a, GridNode b)
    {
        Vector2 posA = new Vector2(a.WorldPosition.x, a.WorldPosition.z);
        Vector2 posB = new Vector2(b.WorldPosition.x, b.WorldPosition.z);
        return Mathf.RoundToInt(Vector2.Distance(posA, posB));
    }

    private GridNode GetLowestFCost(List<GridNode> nodes)
    {
        GridNode lowest = nodes[0];
        foreach (var node  in nodes)
        {
            if (node.FCost < lowest.FCost || (node.FCost == lowest.FCost && node.HCost < lowest.HCost))
            {
                lowest = node;
            }
        }
        return lowest;
    }

    

    private List<GridNode> ReconstructPath(GridNode startNode, GridNode endNode)
    {
        List<GridNode> path =  new List<GridNode>();
        GridNode current = endNode;
        while (current != startNode)
        {
            path.Add(current);
            current = current.Parent;
        }
        path.Add(startNode);
        path.Reverse();

        gridManager.Path = path;
        return path;
    }

    


}
