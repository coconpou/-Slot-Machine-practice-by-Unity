using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CoinRollBarController : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    // Start is called before the first frame update
    void Start()
    {
        _slider.maxValue = SlotGameCoinBoard.slotCoin;
    } // Start

    public void OnSliderValueChange()
    {
        BetCoin.betCoin = (int)_slider.value;
    } // Slider Value Change
    // Update is called once per frame
    void Update()
    {
        _slider.value = BetCoin.betCoin;
        _slider.maxValue = SlotGameCoinBoard.slotCoin;
    } // update
}
