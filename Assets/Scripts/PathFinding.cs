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
                    // OpenList���� F Cost�� ���� ���� ��带 ã�´�. ���� F Cost�� ���ٸ� H Cost�� ���� ��带 Ž���Ѵ�.
                    if (opneList[i].GetfCost() < currentNode.GetfCost() || opneList[i].GetfCost() == currentNode.GetfCost() && opneList[i].GethCost() < currentNode.GethCost())
                    {
                        currentNode = opneList[i];
                    }
                }

                // Ž���� ���� openList���� �����ϰ� closeList�� �߰��Ѵ�.
                opneList.Remove(currentNode);
                closedList.Add(currentNode);

                // Ž���� ��尡 targetNode��� Ž�� ����.
                if (currentNode == targetNode)
                {
                    pathSuccess = true;

                    break;
                }

                // ��� Ž��(�̿� ���)
                foreach (ANode node in grid.GetNeighbours(currentNode))
                {
                    // unWalkable ����̰ų� closeList�� �ִ� ��� ��ŵ.
                    if (!node.WalkAble() || closedList.Contains(node))
                    {
                        continue;
                    }

                    // �̿� ������ G Cost�� H Cost�� ����Ͽ� openList�� �߰��Ѵ�.
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

        // ������ WorldPosition�� ���� waypoints�� �������θ� �Ŵ��� �Լ����� �˷��ش�.
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
        Array.Reverse(waypoints); // targetNode���� �Ųٷ� �������� ������ ������ startNode���� ���������� �������.
        return waypoints;        
    }

    // path ����Ʈ�� �ִ� ������ WorldPosition�� Vector3[] �迭�� ��� ����
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

    // �� ��尣�� �Ÿ��� Cost�� ����Ѵ�.
    private int GetDistanceCost(ANode _nodeA, ANode _nodeB)
    {
        int distX = Mathf.Abs(_nodeA.GetGridX() - _nodeB.GetGridX());
        int distY = Mathf.Abs(_nodeA.GetGridY() - _nodeB.GetGridY());
        
        // �� ��尣�� x�� ���� distX�� y�� ���� distY�� ���� x���̵�, y���̵�, �밢�� �̵��� ���� Cost ���.
        // x,y�� 1ĭ �̵��� 10, �밢�� 1ĭ �̵��� ��Ÿ��� ������ ���� 10��Ʈ2�� �ٻ簪���� 14�� ����ġ�� ��.
        if(distX > distY)
        {
            return 14 * distY + 10 * (distX - distY);
        }
        return 14 * distX + 10 * (distY - distX);
    }
}
