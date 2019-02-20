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
    [SerializeField, Range(3.5f, 10f)]
    private float chaseInnacuracy;

    private int levelToStart = 1;
    public static GameController instance;

    UIController uiController;
    LevelGenerator levelGenerator;
    BonusManager bonusManager;
    int currentLevel;
    bool isProtected;
    int coinsCollected;
    int coinsOnLevel;
    int playerLifesLeft;
    bool isRoundWinner;
    float currentChaseInaccuracy;
    private float chaseInaccuracyReduction = 1;

    [SerializeField, Range(0, 5)] private int enemyDamage;
    Queue<GameObject> enemiesStorage;
    
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
        playerLifesLeft = playerLifes;
        levelGenerator = GameObject.FindGameObjectWithTag("LevelGrid").GetComponent<LevelGenerator>();
        uiController = GetComponent<UIController>();
        uiController.LifesAmountUpdate(playerLifesLeft);
        bonusManager = FindObjectOfType<BonusManager>();
        currentChaseInaccuracy = chaseInnacuracy;
    }

    public float GetInaccuracyValue
    {
        get { return currentChaseInaccuracy; }
    }
    
    private void RefreshPlayerLifes()
    {
        playerLifesLeft = playerLifes;
        uiController.LifesAmountUpdate(playerLifesLeft);
    }

    private void LevelSetter(bool isNext)
    {
        ClearLevel();
        if (isNext)
        {
            levelGenerator.GenerateLevel(++currentLevel);
            uiController.UpdateLevelText(currentLevel);
            RefreshPlayerLifes();
            return;
        }
        levelGenerator.GenerateLevel(currentLevel);       
    }   

    private void RestartGame()
    {
        ClearLevel();
        RefreshStats();
        currentLevel = levelToStart;
        currentChaseInaccuracy = chaseInnacuracy; 
        uiController.UpdateLevelText(currentLevel);     
    }

    private void OnRoundWin()
    {
        uiController.RoundCompleted();
        if (!(currentLevel < levelGenerator.GetLevelsAmount))
        {
            OnGameWin();
            return; 
        }
        currentChaseInaccuracy -= chaseInaccuracyReduction;
        LevelSetter(true);
    }

    private void OnGameWin()
    {
        RestartGame();
        uiController.ActivateWinnerScreen(LevelSetter); //contains restart callback
    }

    public void OnPlayerDamaged()
    {
        if (isProtected)
            return;
        StartCoroutine(ProtectionActivator());
        uiController.PlayerDamaged();
        playerLifesLeft -= enemyDamage;
        uiController.LifesAmountUpdate(playerLifesLeft);
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

    //refreshing game stats on lose/win e.g. collected coins
    private void RefreshStats()
    {
        RefreshPlayerLifes();
        coinsOnLevel = 0;
        coinsCollected = 0;
        uiController.AcquiredCoinsUpdate(coinsCollected);       
    }

    //used to get rid of all on-level objects in terms 
    private void ClearLevel()
    {        
        bonusManager.ClearBonuses(); //used to inform all spawned bonuses to get destroyed
        foreach (Transform child in levelGenerator.transform)
        {
            Destroy(child.gameObject);
        }
    }

    //used to detect coin collection by event in Coin
    public void CollectCoin()
    {
        uiController.AcquiredCoinsUpdate(++coinsCollected);
        if (coinsCollected == coinsOnLevel && coinsCollected > 0)
        {
            OnRoundWin();
        }
    }

    //used to detect how many coins were spawned on level by GridSpawner
    public void NewCoinsAmount(int spawned = 1)
    {
        coinsOnLevel += spawned;
    }

    public IEnumerator ProtectionActivator()
    {
        isProtected = true;
        yield return new WaitForSeconds(protectionDuration);
        isProtected = false;
    }

    public IEnumerator GhostFormActivator(float duration)
    {
        isProtected = true;
        yield return new WaitForSeconds(duration);
        isProtected = false;
    }

    public void IncreasePlayerHealth()
    {
        playerLifesLeft += 1;
        uiController.LifesAmountUpdate(playerLifesLeft);
    }
}
