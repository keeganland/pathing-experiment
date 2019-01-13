using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathGrid : MonoBehaviour {

    public LayerMask unwalkableMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    public bool displayGridGizmos;
    public bool UseTilesToCreateGrid;
    public Tilemap tilemap;
    public TerrainType[] walkableRegions;
    private LayerMask walkableMask;
    private Dictionary<int,int> walkableRegionsDictionary = new Dictionary<int, int>();

    private PathNode[,] grid;
    private float nodeDiameter;
    private int gridSizeX, gridSizeY;
    /* Properties
     * */
    public int MaxSize
    {
        get
        {
            return gridSizeX * gridSizeY;
        }
    }

    //Monobehaviour stuff
    private void Awake()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        walkableMask = new LayerMask();
        
        foreach (TerrainType region in walkableRegions)
        {
            /* The value of a LayerMask is a 32-bit number, with exactly one of the bits flipped, representing which of the 32 layers we're dealing with.
             * To combine these, we use logical addition, AKA bitwise-OR
             * And just like the += operator, there is a |= operator
             * */
            walkableMask.value |= region.terrainMask.value;
            walkableRegionsDictionary.Add((int)Mathf.Log(region.terrainMask.value, 2), region.terrainPenalty);

        }

        Debug.Log("walkable mask value == " + walkableMask.value);
        this.CreateGrid();
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y, 0));
        if (grid != null && displayGridGizmos)
        {
            foreach (PathNode n in grid)
            {
                Gizmos.color = (n.Traversable) ? Color.white : Color.red;
                Gizmos.DrawWireCube(n.WorldPosition, Vector3.one * (nodeDiameter * 0.9f));
            }
        }
    }
   
    //Grid functionality methods
    private void CreateGrid()
    {
        grid = new PathNode[gridSizeX, gridSizeY];
        Vector2 positionV2 = transform.position; //lops the z coordinate off the position vector;
        Vector2 worldBottomLeft = positionV2 - Vector2.right * gridWorldSize.x / 2 - Vector2.up * gridWorldSize.y / 2; //unlike the video, we're ignoring z axis.
        Vector2 tileAnchorV2 = tilemap.tileAnchor;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector2 worldPoint = worldBottomLeft + Vector2.right * (x * nodeDiameter + nodeRadius) + Vector2.up * (y * nodeDiameter + nodeRadius);
                bool traversable = true;

                if (UseTilesToCreateGrid)
                {
                    //This seems to be a weird place to put the offset to me, but this works. Figure this out!!
                    traversable = !(tilemap.HasTile(Vector3Int.RoundToInt(worldPoint - tileAnchorV2)));
                }
                else
                {
                    //traversable = !(Physics2D.OverlapCircle(worldPoint, nodeRadius, unwalkableMask)); // original
                    traversable = !(Physics2D.OverlapCircle(worldPoint, nodeRadius * 0.9f, unwalkableMask)); // weirdly works better, but still pretty off
                }

                int movementPenalty = 0;

                /* Raycast stuff from Lague
                 * 
                 * */
                
                if (traversable)
                {
                    
                    Vector3 raycastOrigin = worldPoint;
                    raycastOrigin += Vector3.back * 50; //magic number 50: just needs to be "some big number"
                    Ray ray = new Ray(raycastOrigin, Vector3.forward);                                        
                    RaycastHit2D rh2d = Physics2D.GetRayIntersection(ray,Mathf.Infinity,walkableMask.value); //this is a conditional in the video, but I don't think RaycastHit2D can work like that
                    walkableRegionsDictionary.TryGetValue(rh2d.collider.gameObject.layer, out movementPenalty);

                    Debug.Log("At tile (" + worldPoint.x + "," + worldPoint.y + ") the movement penalty is: " + movementPenalty);
                }

                grid[x, y] = new PathNode(traversable, worldPoint, x, y, movementPenalty);
            }
        }
    }
    /*Used to e.g. find the node that the player is currently standing on, for example
     */
    public PathNode NodeFromWorldPoint (Vector2 worldPosition)
    {
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.y + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        return grid[x, y];
    }
    /* Does not return an array, because there's no guarantee how many neighbours that a node will have. 
     */
    public List<PathNode> FindNeighbours(PathNode pathNode)
    {
        List<PathNode> neighbours = new List<PathNode>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = pathNode.GridCoordX + x;
                int checkY = pathNode.GridCoordY + y;

                //If we are looking at something on the grid/if our index is not out of bounds
                if (checkX >= 0 && checkX < this.gridSizeX && checkY >= 0 && checkY < this.gridSizeY) 
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }

    [System.Serializable]
    public class TerrainType
    {
        public LayerMask terrainMask;
        public int terrainPenalty;
    }
}
