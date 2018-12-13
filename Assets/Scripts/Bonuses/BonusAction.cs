using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Stores bonus effect for this particular bonus-object
/// </summary>
public class BonusAction : MonoBehaviour {

    UnityEvent bonusEffect;
    BonusManager bonusTestManager;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        bonusTestManager = FindObjectOfType<BonusManager>();
    }

    public void SetUp(UnityEvent startEffect)
    {
        bonusEffect = startEffect;
    } 

    private void StartEffect()
    {
        if (bonusEffect != null)
            bonusEffect.Invoke();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            StartEffect();
            bonusTestManager.ReturnBonusToPool(gameObject, true);
        }
    }
}