using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UIController : MonoBehaviour {
    [SerializeField]
    private GameObject winnerScreen;
    [SerializeField]
    private GameObject ui;
    [SerializeField]
    private GameObject damageTaken;
    [SerializeField]
    private GameObject roundWin;
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
        playerDamagedImage = damageTaken.GetComponent<Image>();
        roundWinImage = roundWin.GetComponent<Image>();
    }

    private void Update()
    {
        if (isPaused && Input.GetKeyDown(KeyCode.Space))
        {
            isPaused = false;
            ui.SetActive(true);
            winnerScreen.SetActive(false);
            Time.timeScale = 1f;
            unPauseCallback(false);
        }
    }

    public void ActivateWinnerScreen(Action<bool> methodToUnpause)
    {
        isPaused = true;
        ui.SetActive(false);
        winnerScreen.SetActive(true);
        Time.timeScale = 0f;
        unPauseCallback = methodToUnpause;
    }

    public void RoundCompleted()
    {
        StartCoroutine(ScreenBlick(roundWinImage));
    }

    public void UpdateLevelText(int level)
    {
        levelText.text = String.Format("Level {0}", level);
    }

    public void PlayerDamaged()
    {
        StartCoroutine(ScreenBlick(playerDamagedImage));
    }

    private IEnumerator ScreenBlick(Image image)
    {
        Color tmpColor = image.color;

        while (image.color.a < 1f)
        {
            tmpColor.a += blickSpeed;
            image.color = tmpColor;
            yield return null;
        }
        while (image.color.a > 0)
        {
            tmpColor.a -= blickSpeed;
            image.color = tmpColor;
            yield return null;
        }
    }
    
    public void AcquiredCoinsUpdate(int collectedCoins)
    {
        collectedCoinsText.text = collectedCoins.ToString();
    }

    public void SetLifesAmount(int lifes)
    {
        lifesAmountText.text = lifes.ToString();
    }
}