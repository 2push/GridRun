using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

[Serializable]
public class BonusBase : MonoBehaviour {
    [Tooltip("Set bonus' duration")]
    [SerializeField] private float duration;
    [Tooltip("Set bonus sprite")]
    [SerializeField] private List<Sprite> sprites;    
    [Tooltip("Apply the effect")]
    [SerializeField] private List<UnityEvent> bonusEffects;

    ASGrid asGrid;
    List<BonusData> bonuses;
    PlayerController player;
    Vector3 playerLastStablePosition;
    CapsuleCollider playerCollider;
    EnemyController[] enemies;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        bonuses = new List<BonusData>();
        asGrid = FindObjectOfType<ASGrid>();
    }

    public List<BonusData> GetBonuses()
    {
        if (sprites.Count != bonusEffects.Count)
        {
            throw new Exception("Not all bonus-data lists are equal!");
        }
        for(int i = 0; i < sprites.Count; i++)
        {
            bonuses.Add(new BonusData(sprites[i], bonusEffects[i]));
        }
        return bonuses;
    }

    public void ActivateHighSpeed()
    {
        if(player==null)
            player = FindObjectOfType<PlayerController>();
        player.moveSpeed *= 2;
        StartCoroutine(DeactivateHighSpeed());
    }

    private IEnumerator DeactivateHighSpeed()
    {
        yield return new WaitForSeconds(duration);
        player.moveSpeed /= 2;
    }

    public void ActivateGhostForm()
    {
        StartCoroutine(GameController.instance.GhostFormActivator(duration));
    }

    public void ActivateHeal()
    {
        GameController.instance.IncreasePlayerHealth();
    }

    public void ActivateKillSpider()
    {
        try
        {
            GameObject enemyToDie = FindObjectOfType<EnemyController>().gameObject;
            Destroy(enemyToDie);
        }
        catch
        {
            print("There are no enemies");
        }     
    }

    public void ActivateEnemySlow()
    {
        try
        {
            enemies = FindObjectsOfType<EnemyController>(); 
            foreach(EnemyController enemy in enemies)
            {
                enemy.speed /= 2;
            }
            StartCoroutine(DeactivateEnemySlow());
        }
        catch
        {
            print("There are no enemies");
        }
    }

    private IEnumerator DeactivateEnemySlow()
    {
        yield return new WaitForSeconds(duration);
        foreach (EnemyController enemy in enemies)
        {
            enemy.speed *= 2;
        }
    }

    public void ActivateEnemyFast()
    {
        try
        {
            enemies = FindObjectsOfType<EnemyController>();
            foreach (EnemyController enemy in enemies)
            {
                enemy.speed *= 2;
            }
            StartCoroutine(DeactivateEnemyFast());
        }
        catch
        {
            print("There are no enemies");
        }
    }

    private IEnumerator DeactivateEnemyFast()
    {
        yield return new WaitForSeconds(duration);
        foreach (EnemyController enemy in enemies)
        {
            enemy.speed /= 2;
        }
    }

    public void ActivateLowSpeed()
    {
        if (player == null)
            player = FindObjectOfType<PlayerController>();
        player.moveSpeed /= 2;
        StartCoroutine(DeactivateLowSpeed());
    }

    private IEnumerator DeactivateLowSpeed()
    {
        yield return new WaitForSeconds(duration);
        player.moveSpeed *= 2;
    }

    public void ActivateFat()
    {
        if (playerCollider == null)
            playerCollider = FindObjectOfType<PlayerController>().GetComponent<CapsuleCollider>();
        float normalRadius = playerCollider.radius;
        playerCollider.radius = 0.55f;
        StartCoroutine(DeactivateFat(normalRadius));
    }

    private IEnumerator DeactivateFat(float normalRadius)
    {
        yield return new WaitForSeconds(duration);
        playerCollider.radius = normalRadius;
    }
}

/// <summary>
/// Stores components to define bonus-object
/// </summary>
public struct BonusData
{
    public Sprite sprite;
    public UnityEvent bonusEffect;

    public BonusData(Sprite _sprite, UnityEvent _bonusEffect)
    {
        sprite = _sprite;
        bonusEffect = _bonusEffect;
    }
}