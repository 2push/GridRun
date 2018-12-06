using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Values {
    /// <summary>
    /// Manhattan K for diagonal nodes
    /// </summary>
    public static int manhattanKD = 14;
    /// <summary>
    /// Manhattan K for neighbour nodes
    /// </summary>
    public static int manhattanKN = 10;
    /// <summary>
    /// Number of the first level
    /// </summary>
    public static int firstLevel = 1;
    /// <summary>
    /// Amount of enemies damage
    /// </summary>
    public static int enemyDamage = 1;
    /// <summary>
    /// Indent between nodes in A Star grid for drawing gizmos only
    /// </summary>
    public static float IndentBetweenNodes = .2f;
    /// <summary>
    /// Number of X dimension in two-dimension array
    /// </summary>
    public static int xDimension = 0;
    /// <summary>
    /// Number of Y dimension in two-dimension array
    /// </summary>
    public static int yDimension = 1;
    /// <summary>
    /// Player setup position
    /// </summary>
    public static Vector2 playerPos = new Vector2(1,1);
    /// <summary>
    /// Enemy setup position indent from other corner in case of even number of nodes
    /// </summary>
    public static int enemyPosEven = 2;
    /// <summary>
    /// Enemy setup position indent from other corner in case of uneven number of nodes
    /// </summary>
    public static int enemyPosUnE = 3;
    /// <summary>
    /// Amount of outer walls at the grid
    /// </summary>
    public static int outerWalls = 2;
    /// <summary>
    /// Offset of coin spawn at Y-axis
    /// </summary>
    public static float coinOffset = 0.1f;
    /// <summary>
    /// Offset of enemy spawn at Y-axis
    /// </summary>
    public static float enemyOffset = 0.15f;
    /// <summary>
    /// Offset of player spawn at Y-axis
    /// </summary>
    public static float playerOffset = 0.2f;
}
