using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSpawner : MonoBehaviour {
   
    [SerializeField]
    private GameObject roadPrefab;
    [SerializeField]
    private GameObject wallPrefab;
    [SerializeField]
    private GameObject coinPrefab;
    [SerializeField]
    private GameObject playerPrefab;
    [SerializeField]
    private GameObject enemyPrefab;
    [SerializeField]
    private float delayBeforeNewEnemy;

    // Offset of coin spawn at Y-axis
    private float coinOffset = 0.1f;
    // Offset of enemy spawn at Y-axis
    private float enemyOffset = 0.15f;
    // Offset of player spawn at Y-axis
    private float playerOffset = 0.2f;
    Transform gridTransform;

    public void SetGridTransformr(Transform _gridTransform)
    {
        gridTransform = _gridTransform; //used for setting LevelGrid object as parent for further refreshing of level by destroying gameobjects
    }

    public void SpawnRoad(Vector3 spawnPosition)
    {
        GameObject roadCell = Instantiate(roadPrefab, spawnPosition, Quaternion.identity);
        roadCell.transform.parent = gridTransform;
    }

    public void SpawnWall(Vector3 spawnPosition)
    {
        GameObject wallCell = Instantiate(wallPrefab, spawnPosition, Quaternion.identity);
        wallCell.transform.parent = gridTransform;
    }

    public void SpawnCoin(Vector3 spawnPosition)
    {
        GameObject coin = Instantiate(coinPrefab, spawnPosition + Vector3.up * coinOffset, Quaternion.identity);
        coin.transform.parent = gridTransform;
        coin.GetComponent<Coin>().OnCoinCollected += GameController.instance.CollectCoin;
        GameController.instance.NewCoinsAmount();
    }

    public void SpawnPlayer(Vector3 spawnPosition)
    {
        GameObject player = Instantiate(playerPrefab, spawnPosition + Vector3.up * playerOffset, Quaternion.identity);
        player.transform.parent = gridTransform;
    }

    public void SpawnEnemies(Vector3 spawnPosition, int enemiesNumber)
    {
        StartCoroutine(SomeEnemiesSpawning(spawnPosition, enemiesNumber));
    }

    private IEnumerator SomeEnemiesSpawning(Vector3 spawnPosition, int enemiesNumber)
    {
        for (int i = 0; i < enemiesNumber; i++)
        {
            GameObject enemy = Instantiate(enemyPrefab, spawnPosition + Vector3.up * enemyOffset, Quaternion.identity);
            enemy.transform.parent = gridTransform;
            enemy.GetComponent<EnemyController>().OnDamageDone += GameController.instance.OnPlayerDamaged;
            yield return new WaitForSeconds(delayBeforeNewEnemy);
        }
    }
}
