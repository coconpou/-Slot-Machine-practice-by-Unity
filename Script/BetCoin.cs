using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class BetCoin : MonoBehaviour
{
    public static int betCoin;
    public TextMeshProUGUI showBetCoin;
    // Start is called before the first frame update
    void Start()
    {
        betCoin = 0;
        showBetCoin.text = "Bet Coin: " + betCoin;
    } // start

    public void PlusBet()
    {
        if (betCoin < SlotGameCoinBoard.slotCoin) betCoin += 1;
    } // plus bet

    public void MinusBet()
    {
        Debug.Log(betCoin + "-1");
        if ( betCoin > 0  ) betCoin -= 1;
    } // minus bet

    public void ALLin()
    {
        betCoin = SlotGameCoinBoard.slotCoin;
    } // all in

    // Update is called once per frame
    void Update()
    {  

        showBetCoin.text = "Bet Coin: " + betCoin;

    } // unpdate
}
