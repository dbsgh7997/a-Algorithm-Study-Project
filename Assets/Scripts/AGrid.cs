using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AGrid : MonoBehaviour
{
    [SerializeField] private LayerMask unWalkableMask;
    [SerializeField] private Vector2 gridWorldSize;
    [SerializeField] float nodeRadius;

    private ANode[,] grid;
    private float nodeDiameter;
    private int gridSizeX;
    private int gridSizeY;
    private List<ANode> path;

    private void Awake()
    {
        nodeDiameter = nodeRadius * 2; // Inspector에서 설정한 반지름으로 지름을 구함.
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter); // Width for grid
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter); // Height for grid
        CreateGrid();
    }

    private void CreateGrid()
    {
        grid = new ANode[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;
        Vector3 worldPoint;
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unWalkableMask));
                grid[x, y] = new ANode(walkable, worldPoint, x, y);
            }
        }
    }

    public List<ANode> GetNeighbours(ANode node) // OpenList에 추가하기 위한 함수.
    {
        List<ANode> neighbours = new List<ANode>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                {
                    continue; // 8방 검색에서 자기 자신은 제외.
                }

                int checkX = node.GetGridX() + x;
                int checkY = node.GetGridY() + y;

                if(checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }
        return neighbours;
    }

    public ANode GetNodeFromWorldPoint(Vector3 worldPosition) // Scene에 있는 Current Object와 Target Object가 있는 노드를 얻기 위한 함수.
    {
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        return grid[x, y];
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

        if(grid != null)
        {
            foreach (ANode node in grid)
            {
                Gizmos.color = node.WalkAble() ? Color.white : Color.red;
                if(path != null)
                {
                    if(path.Contains(node))
                    {
                        Gizmos.color = Color.black;
                    }
                }
                Gizmos.DrawCube(node.WorldPos(), Vector3.one * (nodeDiameter - .1f));
            }
        }
    }

    public void SetGridPath(List<ANode> _path)
    {
        path = _path;
    }
}
