using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* I like to have references to neighbouring nodes as part of the node class itself usually. 
 * 
 * I'm not doing that here (same with the video tutorial I am following). Look at the GetNeighbours method in the PathGrid for how to get the list of future nodes.
 * 
 * PathGrid is probably a superior choice this time, because it has access to the whole grid st it can check
 */

public class PathNode : IHeapItem<PathNode> {

    private bool traversable;
    private Vector2 worldPosition;
    private int gridCoordX;
    private int gridCoordY;
    private int movementPenalty;


    private int gCost; //A* algorithm: gCost is cost from the start node to this node
    private int hCost; //A* algorithm: hCost is heuristic estimate of distance of this node to the target
    private int heapIndex;
    private PathNode parent;

    /* Properties
     * 
     * */
    //read-write properties
    public int HeapIndex
    {
        get
        {
            return heapIndex;
        }

        set
        {
            heapIndex = value;
        }
    }
    public int GCost
    {
        get
        {
            return gCost;
        }
        set
        {
            gCost = value;
        }
    }
    public int HCost
    {
        get
        {
            return hCost;
        }
        set
        {
            hCost = value;
        }
    }
    public PathNode Parent
    {
        get
        {
            return parent;
        }

        set
        {
            parent = value;
        }
    }
    public int MovementPenalty
    {
        get
        {
            return movementPenalty;
        }
        set
        {
            movementPenalty = value;
        }
    }

    //read-only properties
    public int FCost //A* algorithm's methodology is always select node with lowest fCost
    {
        //Keegan NTS: does this need to be public in order to use this syntax?
        get
        {
            return gCost + hCost;
        }
    }
    public int GridCoordX
    {
        get
        {
            return gridCoordX;
        }
    }
    public int GridCoordY
    {
        get
        {
            return gridCoordY;
        }
    }
    public Vector2 WorldPosition
    {
        get
        {
            return worldPosition;
        }
    }
    public bool Traversable
    {
        get
        {
            return traversable;
        }
    }


    /* Constructor
     * 
     * */
    public PathNode (bool traversable, Vector2 worldPosition, int x, int y, int penalty)
    {
        this.traversable = traversable;
        this.worldPosition = worldPosition;
        this.gridCoordX = x;
        this.gridCoordY = y;
        this.movementPenalty = penalty;
    }

    public int CompareTo(PathNode other)
    {
        int compare = FCost.CompareTo(other.FCost);
        if (compare == 0)
        {
            compare = HCost.CompareTo(other.HCost); //remember, hCost is the tiebreaker
        }
        //CompareTo on integers treats higher as better. Because this is a pathing algorithm, our problem instead is minimization. So therefore, we return _negative_ compare.
        return -compare;
    }
}
