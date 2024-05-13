using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinStation : MonoBehaviour
{
    private int coinSpotsFilled = 0;
    public int winningNumber = 3;
    [SerializeField] private GameManager gameManager;

    public void CheckCoinSpots()
    {
        if (coinSpotsFilled == winningNumber)
        {
            //win game?
            gameManager.ChangeToEscaped();
        }
        else
        {
            coinSpotsFilled++;
        }
    }
}
