using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public event Action OnCoinCollected;

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (OnCoinCollected != null)
            {
                OnCoinCollected();
            }
            Destroy(gameObject);
        }
    }

    private void OnDisable()
    {
        if (OnCoinCollected != null)
        {
            OnCoinCollected -= GameController.instance.CollectCoin;
        }
    }
}