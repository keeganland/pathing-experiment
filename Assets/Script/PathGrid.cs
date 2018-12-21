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


    private PathNode[,] grid;
    private float nodeDiameter;
    private int gridSizeX, gridSizeY;
    //private List<PathNode> path;

    // Use this for initialization
    private void Awake()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        this.CreateGrid();
    }    
    private void CreateGrid()
    {
        grid = new PathNode[gridSizeX, gridSizeY];
        Vector2 positionV2 = transform.position; //lops the z coordinate off the position vector;
        Vector2 worldBottomLeft = positionV2 - Vector2.right * gridWorldSize.x / 2 - Vector2.up * gridWorldSize.y / 2; //unlike the video, we're ignoring z axis.
        //Vector3 worldBottomLeftV3 = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.up * gridWorldSize.y / 2; //unlike the video, we're ignoring z axis.
        Vector2 tileAnchorV2 = tilemap.tileAnchor;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector2 worldPoint = worldBottomLeft + Vector2.right * (x * nodeDiameter + nodeRadius) + Vector2.up * (y * nodeDiameter + nodeRadius);
                //Vector3 worldPoint = worldBottomLeftV3 + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.up * (y * nodeDiameter + nodeRadius); 
                bool traversable = true;
                if (UseTilesToCreateGrid)
                {
                    //This seems to be a weird place to put the offset to me, but this works. Figure this out!!
                    traversable = !(tilemap.HasTile(Vector3Int.RoundToInt(worldPoint - tileAnchorV2)));
                }
                else
                {
                    traversable = !(Physics2D.OverlapCircle(worldPoint, nodeRadius, unwalkableMask));
                }
                grid[x, y] = new PathNode(traversable, worldPoint, x, y);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y, 0));
        if (grid != null && displayGridGizmos)
        {
            foreach (PathNode n in grid)
            {
                Gizmos.color = (n.IsTraversable()) ? Color.white : Color.red;
                Gizmos.DrawWireCube(n.GetWorldPosition(), Vector3.one * (nodeDiameter * 0.9f));
            }
        }        
    }
    public int MaxSize
    {
        get
        {
            return gridSizeX * gridSizeY;
        }
    }
    /*
     * Used to e.g. find the node that the player is currently standing on, for example
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
    /*
     * Not an array, because there's no guarantee how many neighbours that it will have. 
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

                int checkX = pathNode.GetGridCoordX() + x;
                int checkY = pathNode.GetGridCoordY() + y;

                //If we are looking at something on the grid/if our index is not out of bounds
                if (checkX >= 0 && checkX < this.gridSizeX && checkY >= 0 && checkY < this.gridSizeY) 
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }
    /*
    public List<PathNode> GetPath()
    {
        return path;
    }
    public void SetPath(List<PathNode> _path)
    {
        this.path = _path;
    }
    */

}
