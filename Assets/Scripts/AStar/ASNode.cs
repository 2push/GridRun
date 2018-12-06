using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ASNode {

    public int x;
    public int y;
    public Vector3 worldPos;
    public bool walkable;
    public int gCost;
    public int hCost;
    public ASNode parentNode;

    public ASNode(int _x, int _y, Vector3 _worldPos, bool _walkable)
    {
        x = _x;
        y = _y;
        worldPos = _worldPos;
        walkable = _walkable;
    }

    public int FCost
    {
        get { return gCost + hCost; }
    }
}