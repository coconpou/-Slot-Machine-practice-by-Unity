using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class SlotGameCoinBoard : MonoBehaviour
{
    public static int slotCoin;
    public static bool whenStop_isOutOfCoin;
    public TextMeshProUGUI showSlotScore;
    [SerializeField] private UnityEvent OutOfCoins;
    private void Awake()
    {
        InitializeCoin();
    } // awake
    
    public static void InitializeCoin()
    {
        slotCoin = 10;
        whenStop_isOutOfCoin = false;
    } // initialize Coin
    public void Update()
    {
        UpdateScore();
    } // plus point
 
    void UpdateScore()
    {   if (whenStop_isOutOfCoin) OutOfCoins.Invoke();
        else showSlotScore.text = "Coin: " + slotCoin.ToString();
    } // update score
}
