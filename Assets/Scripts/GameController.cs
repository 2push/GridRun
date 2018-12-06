using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameController: MonoBehaviour
{
    [SerializeField]
    private int playerLifes;
    [SerializeField]
    private float protectionDuration;

    public static GameController instance;

    UIController uiController;
    LevelGenerator levelGenerator;
    int currentLevel;
    WaitForSeconds waitForProtectionToOff; 
    bool isProtected;
    int coinsCollected;
    int coinsOnLevel;
    int playerLifesLeft;
    bool isRoundWinner;
    float enemyInaccuracyReduce;
    int enemiesAmount;

    private void Awake()
    {
        #region Singltone
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        #endregion
    }
    
    private void Start()
    {
        Init();
        LevelSetter(true);
    }

    private void Init()
    {
        waitForProtectionToOff = new WaitForSeconds(protectionDuration);
        playerLifesLeft = playerLifes;
        levelGenerator = GameObject.FindGameObjectWithTag("LevelGrid").GetComponent<LevelGenerator>();
        uiController = GetComponent<UIController>();
        uiController.SetLifesAmount(playerLifesLeft);
    }
    
    private void RefreshPlayerLifes()
    {
        playerLifesLeft = playerLifes;
        uiController.SetLifesAmount(playerLifesLeft);
    }

    private void LevelSetter(bool isNext)
    {
            if (isNext)
            {
                EnemyController.ChangeInaccuracy(++enemyInaccuracyReduce);
                ClearLevel();
                levelGenerator.GenerateLevel(++currentLevel);
                uiController.UpdateLevelText(currentLevel);
                RefreshPlayerLifes(); 
                return;
            }
            ClearLevel();
            levelGenerator.GenerateLevel(currentLevel);       
    }

    private void RestartGame()
    {
        ClearLevel();
        RefreshStats();
        currentLevel = Values.firstLevel;
        uiController.UpdateLevelText(currentLevel);     
    }

    private void OnRoundWin()
    {
        Debug.Log("Won the lvl!");
        uiController.RoundCompleted();
        if (!(currentLevel < levelGenerator.GetLevelsAmount))
        {
            OnGameWin();
            return; 
        }
        LevelSetter(true);
    }

    private void OnGameWin()
    {
        RestartGame();
        uiController.ActivateWinnerScreen(LevelSetter);
    }

    public void OnPlayerDamaged()
    {
        if (isProtected)
            return;
        StartCoroutine(ProtectionActivator());
        uiController.PlayerDamaged();
        Debug.Log("You have been damaged!");
        playerLifesLeft -= Values.enemyDamage;
        uiController.SetLifesAmount(playerLifesLeft);
        if (playerLifesLeft < 1)
        {
            PlayerDied();
        }
    }
    
    private void PlayerDied()
    {
        RestartGame();
        LevelSetter(false);
    }

    private void RefreshStats()
    {
        RefreshPlayerLifes();
        coinsOnLevel = 0;
        coinsCollected = 0;
        enemyInaccuracyReduce = 0;
        uiController.AcquiredCoinsUpdate(coinsCollected);       
    }

    private void ClearLevel()
    {
        foreach (Transform child in levelGenerator.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void CollectCoin()
    {
        uiController.AcquiredCoinsUpdate(++coinsCollected);
        Debug.Log("Coin collected!");
        if (coinsCollected == coinsOnLevel && coinsCollected > 0)
        {
            OnRoundWin();
        }
    }

    public void NewCoinsAmount(int spawned = 1)
    {
        coinsOnLevel += spawned;
    }

    public IEnumerator ProtectionActivator()
    {
        isProtected = true;
        yield return waitForProtectionToOff;
        isProtected = false;
    }
}
