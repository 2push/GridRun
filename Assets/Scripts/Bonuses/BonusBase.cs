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
        print("ActivateHighSpeed1");
        if (player==null)
            player = FindObjectOfType<PlayerController>();
        print("ActivateHighSpeed2");
        player.moveSpeed *= 2;
        StartCoroutine(DeactivateHighSpeed());
    }

    private IEnumerator DeactivateHighSpeed()
    {
        yield return new WaitForSeconds(duration);
        print("DeactivateHighSpeed");
        player.moveSpeed /= 2;
    }

    public void ActivateGhostForm()
    {
        print("ActivateGhostForm");
        StartCoroutine(GameController.instance.GhostFormActivator(duration));
    }

    public void ActivateHeal()
    {
        print("ActivateHeal");
        GameController.instance.IncreasePlayerHealth();
    }

    public void ActivateKillSpider()
    {
        print("ActivateKillSpider");
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
        print("ActivateEnemySlow");
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
        print("DeactivateEnemySlow");
        foreach (EnemyController enemy in enemies)
        {
            enemy.speed *= 2;
        }
    }

    public void ActivateEnemyFast()
    {
        print("ActivateEnemyFast");
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
        print("DeactivateEnemyFast");
        foreach (EnemyController enemy in enemies)
        {
            enemy.speed /= 2;
        }
    }

    public void ActivateLowSpeed()
    {
        print("ActivateLowSpeed1");
        if (player == null)
            player = FindObjectOfType<PlayerController>();
        print("ActivateLowSpeed2");
        player.moveSpeed /= 2;
        StartCoroutine(DeactivateLowSpeed());
    }

    private IEnumerator DeactivateLowSpeed()
    {
        yield return new WaitForSeconds(duration);
        print("DeactivateLowSpeed");
        player.moveSpeed *= 2;
    }

    public void ActivateFat()
    {
        print("ActivateFat1");
        if (playerCollider == null)
            playerCollider = FindObjectOfType<PlayerController>().GetComponent<CapsuleCollider>();
        print("ActivateFat2");
        float normalRadius = playerCollider.radius;
        playerCollider.radius = 0.55f;
        StartCoroutine(DeactivateFat(normalRadius));
    }

    private IEnumerator DeactivateFat(float normalRadius)
    {
        yield return new WaitForSeconds(duration);
        print("DeactivateFat");
        if (playerCollider != null)
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