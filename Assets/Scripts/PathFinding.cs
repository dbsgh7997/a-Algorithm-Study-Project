using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    private PathRequestManager requestManager;
    private AGrid grid;

    //[SerializeField] private Transform StartObject;
    //[SerializeField] private Transform TargetObject;

    private void Awake()
    {
        requestManager = GetComponent<PathRequestManager>();
        grid = GetComponent<AGrid>();
    }

    public void StartFindPath(Vector3 _startPos, Vector3 _targetPos)
    {
        StartCoroutine(FindPath(_startPos, _targetPos));
    }

    private IEnumerator FindPath(Vector3 _startPos, Vector3 _targetPos)
    {
        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;

        ANode startNode = grid.GetNodeFromWorldPoint(_startPos);
        ANode targetNode = grid.GetNodeFromWorldPoint(_targetPos);

        if(startNode.WalkAble() && targetNode.WalkAble())
        {
            List<ANode> opneList = new List<ANode>();
            List<ANode> closedList = new List<ANode>();
            opneList.Add(startNode);

            while (opneList.Count > 0)
            {
                ANode currentNode = opneList[0];
                for (int i = 1; i < opneList.Count; i++)
                {
                    // OpenList에서 F Cost가 가장 작은 노드를 찾는다. 만약 F Cost가 같다면 H Cost가 적은 노드를 탐색한다.
                    if (opneList[i].GetfCost() < currentNode.GetfCost() || opneList[i].GetfCost() == currentNode.GetfCost() && opneList[i].GethCost() < currentNode.GethCost())
                    {
                        currentNode = opneList[i];
                    }
                }

                // 탐색된 노드는 openList에서 제거하고 closeList에 추가한다.
                opneList.Remove(currentNode);
                closedList.Add(currentNode);

                // 탐색된 노드가 targetNode라면 탐색 종료.
                if (currentNode == targetNode)
                {
                    pathSuccess = true;

                    break;
                }

                // 계속 탐색(이웃 노드)
                foreach (ANode node in grid.GetNeighbours(currentNode))
                {
                    // unWalkable 노드이거나 closeList에 있는 경우 스킵.
                    if (!node.WalkAble() || closedList.Contains(node))
                    {
                        continue;
                    }

                    // 이웃 노드들의 G Cost와 H Cost를 계산하여 openList에 추가한다.
                    int newCurrentToNeighbourCost = currentNode.GetgCost() + GetDistanceCost(currentNode, node);

                    if (newCurrentToNeighbourCost < node.GetgCost() || !opneList.Contains(node))
                    {
                        node.SetgCost(newCurrentToNeighbourCost);
                        node.SethCost(GetDistanceCost(node, targetNode));
                        node.SetParentNode(currentNode);

                        if (!opneList.Contains(node))
                        {
                            opneList.Add(node);
                        }
                    }
                }
            }
        }
        yield return null;
        if(pathSuccess)
        {
            waypoints = RetracePath(startNode, targetNode);
        }

        // 노드들의 WorldPosition을 담은 waypoints와 성공여부를 매니저 함수에게 알려준다.
        requestManager.finishedProcessingPath(waypoints, pathSuccess);
    }

    private Vector3[] RetracePath(ANode _startNode, ANode _endNode)
    {
        List<ANode> path = new List<ANode>();
        ANode currentNode = _endNode;

        while (currentNode != _startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.GetParentNode();
        }
        Vector3[] waypoints = SimplifyPath(path);
        Array.Reverse(waypoints); // targetNode부터 거꾸로 내려가는 순서를 뒤집어 startNode부터 순차적으로 만들어줌.
        return waypoints;        
    }

    // path 리스트에 있는 노드들의 WorldPosition을 Vector3[] 배열에 담아 리턴
    private Vector3[] SimplifyPath(List<ANode> _path)
    {
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 directionOld = Vector2.zero;

        for (int i = 1; i < _path.Count; i++)
        {
            Vector2 directionNew = new Vector2(_path[i - 1].GetGridX() - _path[i].GetGridX(), _path[i - 1].GetGridY() - _path[i].GetGridY());
            if(directionNew != directionOld)
            {
                waypoints.Add(_path[i].WorldPos());
            }
            directionOld = directionNew;
        }
        return waypoints.ToArray();
    }

    // 두 노드간의 거리로 Cost를 계산한다.
    private int GetDistanceCost(ANode _nodeA, ANode _nodeB)
    {
        int distX = Mathf.Abs(_nodeA.GetGridX() - _nodeB.GetGridX());
        int distY = Mathf.Abs(_nodeA.GetGridY() - _nodeB.GetGridY());
        
        // 두 노드간의 x축 길이 distX와 y축 길이 distY를 구해 x축이동, y축이동, 대각선 이동에 따른 Cost 계산.
        // x,y축 1칸 이동에 10, 대각선 1칸 이동은 피타고라스 정리에 의한 10루트2의 근사값으로 14로 가중치를 둠.
        if(distX > distY)
        {
            return 14 * distY + 10 * (distX - distY);
        }
        return 14 * distX + 10 * (distY - distX);
    }
}
