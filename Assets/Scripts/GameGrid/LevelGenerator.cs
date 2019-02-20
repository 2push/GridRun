using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

public class LevelGenerator : MonoBehaviour {

    [SerializeField, Range(1,5)]
    private int enemiesNumber;
    [SerializeField, Range(1,3)]
    private int enemiesAddPerWave;
    [SerializeField]
    private Vector2 gridSize;
    [SerializeField]
    private float cellDiameter;
    [SerializeField]
    private int needRoadsPerColumn;
    [SerializeField, Range(0f, 1f)]
    private float wallSpawnChance;
    // Player setup position
    [SerializeField] private Vector2 playerSpawnPosition;
    // Enemy setup position indent from other corner in case of uneven number of nodes
    private int enemyPosIndent = 3;
    // Enemy setup position indent from other corner in case of even number of nodes
    private int enemyPosIndentEven = 2;
    // Number of X dimension in two-dimension array
    private int xDimension = 0;
    // Number of Y dimension in two-dimension array
    private int yDimension = 1;
    // Amount of outer walls at the grid
    private int outerWalls = 2;
    GridSpawner gridSpawner;
    Dictionary<int, Action<Vector3[,]>> levelsList;
    LevelGenerationData lvlGenData;
    ASGrid aStarGrid;
    Transform thisTransform;
    CameraController camController;
    BonusManager bonusTestManager;

    private void Start()
    {
        Init();    
    }
    
    private void Init()
    {
        aStarGrid = GameObject.FindGameObjectWithTag("AStar").GetComponent<ASGrid>();
        camController = Camera.main.GetComponent<CameraController>();
        gridSpawner = GetComponent<GridSpawner>();
        bonusTestManager = FindObjectOfType<BonusManager>();
        gridSpawner.SetGridTransformr(transform);
        thisTransform = transform;
        camController.SetCamSize(gridSize.x, gridSize.y, cellDiameter);
        Action<Vector3[,]> level1 = SetLevel1;
        Action<Vector3[,]> level2 = SetLevel2;
        Action<Vector3[,]> level3 = SetLevel3;
        levelsList = new Dictionary<int, Action<Vector3[,]>>
        {
            { 1, level1 },
            { 2, level2 },
            { 3, level3 }
        };
    }

    public void GenerateLevel(int level)
    {
        int rows = Mathf.RoundToInt(gridSize.y / cellDiameter) + 1;
        int columns = Mathf.RoundToInt(gridSize.x / cellDiameter) + 1;
        lvlGenData = new LevelGenerationData(gridSize, cellDiameter, levelsList[level], rows, columns);
        AStarGenerationData ASGenData = new AStarGenerationData(gridSize, cellDiameter, thisTransform.position, columns, rows);      
        GridBasics.GridGeneration(lvlGenData);
        aStarGrid.ProceedLevel(ASGenData);
        bonusTestManager.Launch(gridSize, thisTransform.position, cellDiameter);
    }

    public int GetLevelsAmount
    {
        get { return levelsList.Count; }
    }

    private void SetLevel1(Vector3[,] cellData)
    {
        for (int x = 0; x < cellData.GetLength(xDimension); x++)
        {
            for (int y = 0; y < cellData.GetLength(yDimension); y++)
            {
                #region Side walls spawn
                if (x == 0 || y == 0 ||
                            x == gridSize.x || y == gridSize.y)
                {
                    gridSpawner.SpawnWall(cellData[x, y]); //side walls spawn
                    continue;
                }
                #endregion
                #region Road spawn
                gridSpawner.SpawnRoad(cellData[x,y]);
                #endregion
                #region Enemies spawn
                int enemyXPos = cellData.GetLength(xDimension) % 2 == 0 ? enemyPosIndent : enemyPosIndentEven;
                int enemyYPos = cellData.GetLength(yDimension) % 2 == 0 ? enemyPosIndent : enemyPosIndentEven;
                if (x == cellData.GetLength(xDimension) - enemyXPos && y == cellData.GetLength(yDimension) - enemyYPos)
                {
                    gridSpawner.SpawnEnemies(cellData[x, y], enemiesNumber); //enemies spawn
                }
                #endregion
                #region Coins spawn
                if (!(x == 1 && y == 1))
                {
                    gridSpawner.SpawnCoin(cellData[x, y]); //coins spawn
                }
                #endregion
                #region Player spawn
                if (x == playerSpawnPosition.x && y == playerSpawnPosition.y)
                {
                    gridSpawner.SpawnPlayer(cellData[x, y]); //player spawn
                }
                #endregion
            }
        }
    }

    private void SetLevel2(Vector3[,] cellData)
    {
        int tempEnemiesNumber = enemiesNumber + enemiesAddPerWave;
        for (int x = 0; x < cellData.GetLength(xDimension); x++)
        {
            for (int y = 0; y < cellData.GetLength(yDimension); y++)
            {

                #region Walls spawn
                if ((x % 2 == 0 && y % 2 == 0) ||
                        x == 0 || y == 0 ||
                        x == gridSize.x || y == gridSize.y)
                {
                    gridSpawner.SpawnWall(cellData[x, y]); //walls spawn
                    continue;
                }
                #endregion
                #region Road spawn
                gridSpawner.SpawnRoad(cellData[x, y]); 
                #endregion
                #region Enemies spawn
                int enemyXPos = cellData.GetLength(xDimension) % 2 == 0 ? enemyPosIndent : enemyPosIndentEven;
                int enemyYPos = cellData.GetLength(yDimension) % 2 == 0 ? enemyPosIndent : enemyPosIndentEven;
                if (x == cellData.GetLength(xDimension) - enemyXPos && y == cellData.GetLength(yDimension) - enemyYPos)
                {
                    gridSpawner.SpawnEnemies(cellData[x, y], tempEnemiesNumber); //enemies spawn
                }
                #endregion
                #region Coins spawn
                if (!(x == 1 && y == 1))
                {
                    gridSpawner.SpawnCoin(cellData[x, y]); //coins spawn
                }
                #endregion
                #region Player spawn
                if (x == playerSpawnPosition.x && y == playerSpawnPosition.y)
                {
                    gridSpawner.SpawnPlayer(cellData[x, y]); //player spawn
                } 
                #endregion
            }
        }
    }

    private void SetLevel3(Vector3[,] cellData)
    {

        int tempEnemiesNumber = enemiesNumber + enemiesAddPerWave * 2;
        for (int x = 0; x < cellData.GetLength(xDimension); x++)
        {
            int wallsPerColumn = 0;
            for (int y = 0; y < cellData.GetLength(yDimension); y++)
            {
                
                float rndWallSpawn = UnityEngine.Random.Range(0f, 1f);
                #region Side walls spawn
                if (x == 0 || y == 0 ||
                            x == gridSize.x || y == gridSize.y)
                {
                    gridSpawner.SpawnWall(cellData[x, y]); //side walls spawn
                    continue;
                }
                #endregion
                #region Inner walls spawn
                if (x % 2 == 0)
                {
                    if (rndWallSpawn >= 1f - wallSpawnChance && !(
                    wallsPerColumn >= cellData.GetLength(xDimension) - outerWalls - needRoadsPerColumn))
                    {
                        gridSpawner.SpawnWall(cellData[x, y]); //inner walls spawn
                        wallsPerColumn++;
                        continue;
                    }
                }
                #endregion
                #region Road spawn
                gridSpawner.SpawnRoad(cellData[x, y]);
                #endregion
                #region Enemies spawn
                int enemyXPos = cellData.GetLength(xDimension) % 2 == 0 ? enemyPosIndent : enemyPosIndentEven;
                int enemyYPos = cellData.GetLength(yDimension) % 2 == 0 ? enemyPosIndent : enemyPosIndentEven;
                if (x == cellData.GetLength(xDimension) - enemyXPos && y == cellData.GetLength(yDimension) - enemyYPos)
                {
                    gridSpawner.SpawnEnemies(cellData[x, y], tempEnemiesNumber); //enemies spawn
                }
                #endregion
                #region Coins spawn
                if (!(x == 1 && y == 1))
                {
                    gridSpawner.SpawnCoin(cellData[x, y]); //coins spawn
                }
                #endregion
                #region Player spawn
                if (x == playerSpawnPosition.x && y == playerSpawnPosition.y)
                {
                    gridSpawner.SpawnPlayer(cellData[x, y]); //player spawn
                }
                #endregion
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireCube(transform.position, new Vector3(gridSize.x, 0.5f, gridSize.y));
    }
}

public class AStarGenerationData
{
    public Vector2 gridWorldSize;    
    public float nodeDiameter;   
    public Vector3 gridCenterPosition;
    public int nodesInGridX;
    public int nodesInGridY;

    public AStarGenerationData(Vector2 _gridWorldSize, float _nodeDiameter, Vector3 _gridCenterPosition, int _nodesInGridX, int _nodesInGridY)
    {
        gridWorldSize = _gridWorldSize;
        nodeDiameter = _nodeDiameter;
        gridCenterPosition = _gridCenterPosition;
        nodesInGridX = _nodesInGridX;
        nodesInGridY = _nodesInGridY; 
    }
}