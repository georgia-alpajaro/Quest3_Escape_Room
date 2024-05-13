using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinStation : MonoBehaviour
{
    private int coinSpotsFilled = 0;
    public int winningNumber = 2;
    [SerializeField] private GameManager gameManager;


    public void CheckCoinSpots()
    {
        Debug.Log("checking Coin");
        if (coinSpotsFilled == winningNumber)
        {
            //win game?
            Debug.Log("CheckCoingSpots Escaped");
            gameManager.ChangeToEscaped();
        }
        else
        {
            coinSpotsFilled++;
        }
    }
}
