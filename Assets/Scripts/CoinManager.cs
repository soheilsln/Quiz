using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    private int coins = 100;
    [SerializeField]
    private int correctAnswerCoins = 50;
    [SerializeField]
    private int fiftyFiftyCoins = -50;

    public static CoinManager instance; //Singleton
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        if (!PlayerPrefs.HasKey("coins"))
            PlayerPrefs.SetInt("coins", coins);
        else
            coins = PlayerPrefs.GetInt("coins");
    }

    private void Start()
    {
        UIController.OnAnswered += this.OnAnswered;
        UIController.FiftyFiftyClicked += this.OnFiftyFiftyClicked;
    }

    private void OnDestroy()
    {
        UIController.OnAnswered -= this.OnAnswered;
        UIController.FiftyFiftyClicked -= this.OnFiftyFiftyClicked;
    }

    public int GetCoins()
    {
        return coins;
    }

    public int GetFiftyFiftyCoins()
    {
        return fiftyFiftyCoins;
    }

    private void ChangeCoins(int value)
    {
        coins += value;
        PlayerPrefs.SetInt("coins", coins);
    }

    private void OnAnswered(bool value)
    {
        if (value)
            coins += correctAnswerCoins;
    }

    private void OnFiftyFiftyClicked()
    {
        ChangeCoins(fiftyFiftyCoins);
    }

}
