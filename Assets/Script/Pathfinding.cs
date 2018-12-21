﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class Pathfinding : MonoBehaviour {

    PathRequestManager requestManager;

    PathGrid pathGrid;

    private void Awake()
    {
        pathGrid = GetComponent<PathGrid>();
        requestManager = GetComponent<PathRequestManager>();
    }
    public void StartFindPath(Vector2 startPos, Vector2 targetPos)
    {
        StartCoroutine(FindPath(startPos, targetPos));
    }


    private IEnumerator FindPath (Vector2 startPos, Vector2 targetPos)
    {
        //Diagnostic tools
        Stopwatch sw = new Stopwatch();
        sw.Start();

        Vector2[] waypoints = new Vector2[0];
        bool pathSuccess = false;


        //Initialize the open set and closed set and stick 
        PathNode startNode = pathGrid.NodeFromWorldPoint(startPos);
        PathNode targetNode = pathGrid.NodeFromWorldPoint(targetPos);
        if (startNode.IsTraversable() && targetNode.IsTraversable())
        {
            //List<PathNode> openSet = new List<PathNode>();
            Heap<PathNode> openSet = new Heap<PathNode>(pathGrid.MaxSize);
            HashSet<PathNode> closedSet = new HashSet<PathNode>();
            openSet.Add(startNode);

            //Main loop for search algo
            while (openSet.Count > 0)
            {
                /*
                 * Causing issues when replaced with heap - stack overflows
                 * 
                 * Note from a youtube comment: duh!!!
                 * +ReMi001 In the Node class, when you implement HeapIndex, be very careful and in get and set write heapIndex, not HeapIndex.﻿
                 *
                 */
                PathNode currentNode = openSet.RemoveFirst();
                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    sw.Stop();
                    print("Path found: " + sw.ElapsedMilliseconds + "ms");
                    pathSuccess = true;
                    break;
                }

                foreach (PathNode neighbour in pathGrid.FindNeighbours(currentNode))
                {
                    if (!neighbour.IsTraversable() || closedSet.Contains(neighbour))
                    {
                        continue;
                    }

                    int newMovementCostToNeighbour = currentNode.Get_gCost() + this.CalculateDistance(currentNode, neighbour);
                    if (newMovementCostToNeighbour < neighbour.Get_gCost() || !openSet.Contains(neighbour))
                    {
                        neighbour.Set_gCost(newMovementCostToNeighbour);
                        neighbour.Set_hCost(this.CalculateDistance(currentNode, targetNode));
                        neighbour.SetParent(currentNode);

                        if (!openSet.Contains(neighbour))
                        {
                            openSet.Add(neighbour);
                        }
                        else
                        {
                            openSet.UpdateItem(neighbour);
                        }
                    }
                }
            }
        }
        yield return null; //Makes the co-routine wait one frame before returning?
        if (pathSuccess)
        {
          waypoints = this.RetracePath(startNode, targetNode);
        }
        requestManager.FinishedProcessingPath(waypoints, pathSuccess);
    }
    /**
     * Helper. Called once we've found the target node: generates the actual path by looking at the parents 
     */
    private Vector2[] RetracePath(PathNode startNode, PathNode endNode)
    {
        List<PathNode> path = new List<PathNode>();
        PathNode currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.GetParent();
        }
        Vector2[] waypoints = this.SimplifyPath(path);
        Array.Reverse(waypoints); //Necessary because the path list ends up backwards, since we started at the end and gave it nodes one by one until we reached the beginning.

        return waypoints;
    }

    /*
     * Will be used for path smoothing later. For now, however, it represents an optimization
     */
    private Vector2[] SimplifyPath(List<PathNode> path)
    {
        List<Vector2> waypoints = new List<Vector2>();
        Vector2 directionOld = Vector2.zero;
        for (int i = 1; i < path.Count; i++)
        {
            Vector2 directionNew = new Vector2(path[i - 1].GetGridCoordX() - path[i].GetGridCoordX(), path[i - 1].GetGridCoordY() - path[i].GetGridCoordY());
            if(directionNew != directionOld) //i.e. if the path has changed direction
            {
                waypoints.Add(path[i].GetWorldPosition());
            }
            directionOld = directionNew;
        }

        return waypoints.ToArray();
    }

    private int CalculateDistance(PathNode nodeA, PathNode nodeB)
    {
        /* Conventionally, we multiply distances by 10 so we can use 1.4 * 10 = 14 as an approximation of the diagonal distance
         */

        /* https://youtu.be/mZfyt03LDH4?t=839
         * Imagine the coordinates of the two nodes make a rectangle. 
         * The distance is the diagonal of the square of the shorter side (14 * shorter side)
         * Plus whatever leftover bits there are from the longer side (10 * (longer side - shorter side)
         * 
         * (of course, it's just a straight line if it's on either the same x axis or the same y axis.
         */

        int dstX = Mathf.Abs(nodeA.GetGridCoordX() - nodeB.GetGridCoordX());
        int dstY = Mathf.Abs(nodeA.GetGridCoordY() - nodeB.GetGridCoordY());

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);                
    }

}
