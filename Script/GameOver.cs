
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    SlotGameCoinBoard slotGameBoard;
    public void Restart()
    {
        SceneManager.LoadScene(0);
        //Debug.Log(SlotGameCoinBoard.slotCoin);
        SlotGameCoinBoard.InitializeCoin();

    } // restart
} // GameOver
