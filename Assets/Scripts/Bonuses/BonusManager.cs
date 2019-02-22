using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusManager : MonoBehaviour
{

    [SerializeField] private GameObject bonusPrefab;
    [SerializeField] private float bonusSpawnInterval;
    [SerializeField] private float bonusLifeTime;
    [SerializeField] private int poolSize;
    [SerializeField, Range(0f, 3f)] private float ySpawnHigh;
    [SerializeField] private GameObject particles;

    Vector2 spawnArea;
    Vector3 spawnAreaPos;
    Coroutine bonusGeneration;
    List<BonusData> bonuses; //all kinds of bonuses

    Queue<GameObject> bonusStorage; 
    Dictionary<GameObject, BonusPrefabData> bonusesData; 

    GameObject pickUpEffect;
    ParticleSystem effectParticles;

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        bonuses = GetComponent<BonusBase>().GetBonuses();
        bonusStorage = new Queue<GameObject>();
        bonusesData = new Dictionary<GameObject, BonusPrefabData>();
        #region Pre-creating bonus pool
        for (int i = 0; i < poolSize; i++)
        {           
            GameObject prefab = Instantiate(bonusPrefab, Vector3.zero, Quaternion.AngleAxis(90, Vector3.right));
            prefab.transform.parent = transform;
            BonusAction script = prefab.GetComponent<BonusAction>();
            SpriteRenderer renderer = prefab.GetComponent<SpriteRenderer>();
            BonusPrefabData prefabData = new BonusPrefabData(script, prefab.transform, renderer);
            bonusesData.Add(prefab, prefabData);
            prefab.SetActive(false);
            bonusStorage.Enqueue(prefab);
        }
        #endregion
        #region Pre-creating particles
        pickUpEffect = Instantiate(particles);
        effectParticles = pickUpEffect.GetComponent<ParticleSystem>();
        #endregion
    }

    //Being launched by LevelGenerator as soon as level size and position are known
    public void Launch(Vector2 levelSize, Vector3 levelPosition, float cellD)
    {
        spawnArea = new Vector2(levelSize.x - cellD * 2, levelSize.y - cellD * 2); //to get rid of borders
        spawnAreaPos = levelPosition;
        if (bonusGeneration != null)
            StopCoroutine(bonusGeneration);
        bonusGeneration = StartCoroutine(BonusGeneration());
    }

    public void ClearBonuses()
    {
        foreach (Transform bonus in transform)
        {
            if (bonus.gameObject.activeSelf) //check if bonus is spawned
                ReturnBonusToPool(bonus.gameObject, false);
        }
    }

    public void ReturnBonusToPool(GameObject bonus, bool ifSpawnEffect)
    {
        bonus.SetActive(false);
        bonusStorage.Enqueue(bonus);
        if (ifSpawnEffect)
        {        
            pickUpEffect.transform.position = bonus.transform.position;
            effectParticles.Play();
        }
    }
    private IEnumerator BonusGeneration()
    {
        while (true)
        {
            yield return new WaitForSeconds(bonusSpawnInterval);
            if (bonusStorage.Count > 0)
            {
                float x = Random.Range(spawnAreaPos.x - spawnArea.x * 0.5f, spawnAreaPos.x + spawnArea.x * 0.5f);
                float y = spawnAreaPos.y + ySpawnHigh;
                float z = Random.Range(spawnAreaPos.y - spawnArea.y * 0.5f, spawnAreaPos.y + spawnArea.y * 0.5f);
                Vector3 bonusSpawnPosition = new Vector3(x, y, z);
                int bonusNumber = Random.Range(0, bonuses.Count);
                GameObject bonus = bonusStorage.Dequeue();
                bonus.SetActive(true);
                bonusesData[bonus].transform.position = bonusSpawnPosition;
                bonusesData[bonus].script.SetUp(bonuses[bonusNumber].bonusEffect);
                bonusesData[bonus].spriteRenderer.sprite = bonuses[bonusNumber].sprite;             
            }           
        }
    }
}

/// <summary>
/// Caches components used by bonus prefabs
/// </summary>
public struct BonusPrefabData
{
    public BonusAction script;
    public Transform transform;
    public SpriteRenderer spriteRenderer;
    
    public BonusPrefabData(BonusAction _script, Transform _transform, SpriteRenderer _spriteRenderer)
    {
        script = _script;
        transform = _transform;
        spriteRenderer = _spriteRenderer;
    }
}