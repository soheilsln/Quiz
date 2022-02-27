using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    [SerializeField]
    private int coins = 100; //set for starting coins
    public int Coins { get { return coins; } }
    [SerializeField]
    private int firstRankCoins = 100;
    public int FirstRankCoins { get { return firstRankCoins; } }
    [SerializeField]
    private int secondRankCoins = 50;
    public int SecondRankCoins { get { return secondRankCoins; } }
    [SerializeField]
    private int thirdRankCoins = 0;
    public int ThirdRankCoins { get { return thirdRankCoins; } }
    [SerializeField]
    private int fiftyFiftyCoins = 50;
    public int FiftyFiftyCoins { get { return fiftyFiftyCoins; } }

    private void Awake()
    {
        if (!PlayerPrefs.HasKey("coins"))
            PlayerPrefs.SetInt("coins", coins);
        else
            coins = PlayerPrefs.GetInt("coins");
    }

    private void Start()
    {
        UIController.FiftyFiftyClicked += this.OnFiftyFiftyClicked;
        UIController.OnQuizFinished += this.OnQuizFinished;
    }

    private void OnDestroy()
    {
        UIController.FiftyFiftyClicked -= this.OnFiftyFiftyClicked;
        UIController.OnQuizFinished -= this.OnQuizFinished;
    }

    private void ChangeCoins(int value)
    {
        coins += value;
        PlayerPrefs.SetInt("coins", coins);
    }

    private void OnQuizFinished(int rank)
    {
        if (rank == 1)
            ChangeCoins(firstRankCoins);
        else if (rank == 2)
            ChangeCoins(secondRankCoins);
        else
            ChangeCoins(thirdRankCoins);
    }

    private void OnFiftyFiftyClicked()
    {
        ChangeCoins(-fiftyFiftyCoins);
    }

}
