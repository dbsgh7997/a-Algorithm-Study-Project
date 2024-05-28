using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ANode
{
    private bool isWalkAble;
    private Vector3 worldPos;
    private int gridX;
    private int gridY;

    private int gCost;
    private int hCost;
    private ANode parentNode;

    public ANode(bool _walkAble, Vector3 _worldPos, int _gridX, int _gridY)
    {
        isWalkAble = _walkAble;
        worldPos = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
    }

    public bool WalkAble()
    {
        return isWalkAble;
    }

    public Vector3 WorldPos()
    {
        return worldPos;
    }

    public int GetGridX()
    {
        return gridX;
    }

    public int GetGridY()
    {
        return gridY;
    }

    public int GetfCost()
    {
        return gCost + hCost;
    }

    public int GetgCost()
    {
        return gCost;
    }
    public void SetgCost(int _gCost)
    {
        this.gCost = _gCost;
    }

    public int GethCost()
    {
        return hCost;
    }
    public void SethCost(int _hCost)
    {
        this.hCost = _hCost;
    }
    public ANode GetParentNode()
    {
        return parentNode;
    }
    public void SetParentNode(ANode _node)
    {
        this.parentNode = _node;
    }
}
