using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UIController : MonoBehaviour
{
    [SerializeField]
    private GameObject winnerUI;
    [SerializeField]
    private GameObject ui;
    [SerializeField]
    private GameObject damageUI;
    [SerializeField]
    private GameObject roundWinUI;
    [SerializeField, Range(0.1f, 1f)]
    private float blickSpeed;
    [SerializeField]
    private Text levelText;
    [SerializeField]
    private Text collectedCoinsText;
    [SerializeField]
    private Text lifesAmountText;

    Image playerDamagedImage;
    Image roundWinImage;
    Action<bool> unPauseCallback;
    bool isPaused;

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        playerDamagedImage = damageUI.GetComponent<Image>();
        roundWinImage = roundWinUI.GetComponent<Image>();
    }

    private void Update()
    {
        if (isPaused && Input.GetKeyDown(KeyCode.Space))
        {
            isPaused = false;
            ui.SetActive(true);
            winnerUI.SetActive(false);
            Time.timeScale = 1f;
            unPauseCallback(false);
        }
    }

    public void ActivateWinnerScreen(Action<bool> methodToUnpause)
    {
        isPaused = true;
        ui.SetActive(false);
        winnerUI.SetActive(true);
        Time.timeScale = 0f;
        unPauseCallback = methodToUnpause;
    }

    public void RoundCompleted()
    {
        StartCoroutine(ScreenBlick(roundWinImage, roundWinUI));
    }

    public void UpdateLevelText(int level)
    {
        levelText.text = String.Format("Level {0}", level);
    }

    public void PlayerDamaged()
    {
        StartCoroutine(ScreenBlick(playerDamagedImage, damageUI));
    }

    private IEnumerator ScreenBlick(Image screenImage, GameObject uiScreen)
    {      
        Color tmpColor = screenImage.color;
        uiScreen.SetActive(true);
        while (screenImage.color.a < 1f)
        {
            tmpColor.a += blickSpeed;
            screenImage.color = tmpColor;
            yield return null;
        }
        while (screenImage.color.a > 0)
        {
            tmpColor.a -= blickSpeed;
            screenImage.color = tmpColor;
            yield return null;
        }
        uiScreen.SetActive(false);
    }
    
    public void AcquiredCoinsUpdate(int collectedCoins)
    {
        collectedCoinsText.text = collectedCoins.ToString();
    }

    public void LifesAmountUpdate(int lifes)
    {
        lifesAmountText.text = lifes.ToString();
    }
}