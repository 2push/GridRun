using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class GridBasics : MonoBehaviour
{
    public static void GridGeneration(LevelGenerationData lvlData)
    {
        Vector3[,] cells = new Vector3[lvlData.rows, lvlData.columns];       
        Vector3 gridLeftBottomInWorld = -Vector3.right * lvlData.gridSize.x / 2 - Vector3.forward * lvlData.gridSize.y / 2;

        for (int x = 0; x < lvlData.columns; x++)
        {
            for (int y = 0; y < lvlData.rows; y++)
            {
                Vector3 worldPoint = gridLeftBottomInWorld + Vector3.right * (x * lvlData.cellDiameter + lvlData.cellDiameter / 2)
                    + Vector3.forward * (y * lvlData.cellDiameter + lvlData.cellDiameter / 2);
                cells[x, y] = worldPoint;
            }
        }       
        lvlData.callBack(cells);
    }
}

public struct LevelGenerationData
{
    public Vector2 gridSize;
    public float cellDiameter;
    public Action<Vector3[,]> callBack;
    public int rows;
    public int columns;

    public LevelGenerationData(Vector2 _gridSize, float _cellDiameter, Action<Vector3[,]> _callBack, int _rows, int _columns)
    {
        gridSize = _gridSize;
        cellDiameter = _cellDiameter;
        callBack = _callBack;
        rows = _rows;
        columns = _columns;
    }
}
