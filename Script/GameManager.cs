using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.Events;



public class GameManager : MonoBehaviour
{
   
    [SerializeField] private int boardHeight, boardWidth;
    // 老虎機陣列的長寬
    [SerializeField] private GameObject[] icons;
    [SerializeField] private GameObject GoldFrame;  // 結算畫面的框框
    // [SerializeField] private 
    [SerializeField] private UnityEvent gettingPoint;
    [SerializeField] private UnityEvent gettingNothing;
    [SerializeField] private UnityEvent BetCoin_isZero;
    [SerializeField] private UnityEvent BetCoinNotEnough;

    private List<GameObject> _matchLines;
    // 有幾種圖案
    private List<GameObject> _goldFrames;
    // 連線的框框

    private int numbersOficon;
    private int betCoin;
    private GameObject _board;  // 遊戲版面
    private GameObject[] _gameBoard; // 陣列大小

    private bool _isSpinning;
    bool goldFrameSetted = false;

    private void Initialization()
    {
        foreach (GameObject i in _matchLines)
        {
            GameObject.Destroy(i);
        } // foreach
        foreach (GameObject i in _gameBoard)
        {
            GameObject.Destroy(i);
        } // foreach
        foreach (GameObject i in _goldFrames)
        {
            GameObject.Destroy(i);
        } // foreach
        _goldFrames.Clear();
        _matchLines.Clear();
        goldFrameSetted = false;
        // initialized
    } // initialized

    void Start()
    {
        _board = GameObject.Find("TigerGameBoard");
        numbersOficon = boardHeight * boardWidth;// 5X4的老虎機陣列
        _gameBoard = new GameObject[numbersOficon];  // 總數20
        _matchLines = new List<GameObject>();
        _goldFrames = new List<GameObject>();
        _isSpinning = false;
    } //start

    public void StartSpin()
    {
        if (_isSpinning || SlotGameCoinBoard.slotCoin < BetCoin.betCoin || BetCoin.betCoin == 0)
        {
            if (SlotGameCoinBoard.slotCoin < BetCoin.betCoin) BetCoinNotEnough.Invoke();
            if (BetCoin.betCoin == 0) BetCoin_isZero.Invoke();

            return;
        }//if
        _isSpinning = true;
        betCoin = BetCoin.betCoin;
        SlotGameCoinBoard.slotCoin -= betCoin;
        Initialization() ;

        for (int i = 0; i < numbersOficon; i++)
        {   //把位置[ i ]的icon隨機賦予不同的icon(可以重複)
            GameObject gridPosition = _board.transform.Find(i.ToString()).gameObject;
            GameObject iconType = icons[UnityEngine.Random.Range(0, icons.Length)];
            if (iconType.name == "Wild") iconType = icons[UnityEngine.Random.Range(0, icons.Length)];
            // 降低wild的機率
            GameObject thisIcon = Instantiate(iconType, gridPosition.transform.position, Quaternion.identity);
            thisIcon.name = iconType.name;
            thisIcon.transform.parent = gridPosition.transform;
            _gameBoard[i] = thisIcon; // 放進gameBoard

        } // for
        StopCoroutine("WaitingEndSpin");
        StartCoroutine(WaitingEndSpin(1.1f));
        StopCoroutine("BottonCooLDown");
        StartCoroutine(BottonCooLDown(1.5f));
        StopCoroutine("CheckOutOfCoin");
        StartCoroutine(CheckOutOfCoin(1.2f));
    } // start spin

    public void ReadPattern()
    {
        string fileToRead = @"C:\Users\X512J Series\Desktop\unity\tutorial\SlotGame\Assets\Script\Pattern.txt";
        using (StreamReader ReaderObject = new StreamReader(fileToRead))
        {
            string Line;
            while(( Line = ReaderObject.ReadLine() ) != null)
            {
                string[] patternPosition = Line.Split(' ');
                int first, second, third, forth, fifth; // 連線位置
                try
                {
                    first = Int32.Parse(patternPosition[0]);
                    second = Int32.Parse(patternPosition[1]);
                    third = Int32.Parse(patternPosition[2]);
                    forth = Int32.Parse(patternPosition[3]);
                    fifth = Int32.Parse(patternPosition[4]);
                    FindMatchLine(first, second, third, forth, fifth);
                } // try
                catch
                {
                    Debug.Log("Unable to parse this Line: " + Line);
                } // catch

            } // while
        } // using (StreamReader ReaderObject = new StreamReader(fileToRead))

    } // read pattern

    public void FindMatchLine( int first, int second, int third, int forth, int fifth )
    { // 確認模式與版型相同

        if ( IsSameOrWild(first,second)&&IsSameOrWild(second,third)&&IsSameOrWild(first,third) )
        {    // _gameBoard[first] == _gameBoard[second] && _gameBoard[second] == _gameBoard[third]
            // 前三位一樣
            DrawPayLine(_gameBoard[first].transform.position  ,  _gameBoard[second].transform.position  , Color.yellow );
            DrawPayLine(_gameBoard[second].transform.position  , _gameBoard[third].transform.position  , Color.yellow);
            // 畫位置1~3的賠付線

            SetGoldFrame(first);
            SetGoldFrame(second);
            SetGoldFrame(third);
            goldFrameSetted = true;

            SlotGameCoinBoard.slotCoin += betCoin * 2;
            if (IsSameOrWild(third, forth) && IsSameOrWild(second, forth) )
            {   // _gameBoard[third] == _gameBoard[forth]
                // 第三、第四位一樣
                DrawPayLine(_gameBoard[third].transform.position  ,   _gameBoard[forth].transform.position  , Color.yellow);

                SetGoldFrame(forth);
                SlotGameCoinBoard.slotCoin += betCoin * 3;
                if (_gameBoard[forth].name == _gameBoard[fifth].name
                    || (IsSameOrWild(forth, fifth) && IsSameOrWild(third, fifth)))
                {   // 第四、第五位一樣
                    DrawPayLine(_gameBoard[forth].transform.position  , _gameBoard[fifth].transform.position  , Color.yellow);
                    SetGoldFrame(fifth);
                    SlotGameCoinBoard.slotCoin += betCoin * 5;
                } // if
                else
                {   // 第四、第五位不一樣
                    DrawPayLine(_gameBoard[forth].transform.position ,  _gameBoard[fifth].transform.position, Color.gray);
                } // else
            } // if
            else
            {   // 第四、第五位跟第三位不一樣
                DrawPayLine(_gameBoard[third].transform.position  ,  _gameBoard[forth].transform.position  , Color.gray);
                DrawPayLine(_gameBoard[forth].transform.position  ,  _gameBoard[fifth].transform.position  , Color.gray);
            } // else
        } // if 前三位一樣

        /*else if ( IsSameOrWild(second, third) && IsSameOrWild(third, forth) && IsSameOrWild(second, forth) )
        {  // _gameBoard[second] == _gameBoard[third] && _gameBoard[third]== _gameBoard[forth]
            // 第2、第3位跟第4位一樣, 但和第一位不一樣
            DrawPayLine(_gameBoard[second].transform.position  , _gameBoard[third].transform.position  , Color.yellow);
            DrawPayLine(_gameBoard[third].transform.position  , _gameBoard[forth].transform.position , Color.yellow);
            // 畫位置2~4的賠付線

            SetGoldFrame(second);
            SetGoldFrame(third);
            SetGoldFrame(forth);
            goldFrameSetted = true;
            SlotGameCoinBoard.slotCoin += betCoin  * 2;
            if (_gameBoard[forth].name == _gameBoard[fifth].name)
            {   // 第四、第五位一樣
                DrawPayLine(_gameBoard[forth].transform.position  , _gameBoard[fifth].transform.position,  Color.yellow);
                SetGoldFrame(fifth);
                SlotGameCoinBoard.slotCoin += betCoin  * 3;
            } // if第四、第五位一樣
            else
            { //不一樣
                DrawPayLine(_gameBoard[forth].transform.position ,  _gameBoard[fifth].transform.position , Color.gray);
            } // else 
            DrawPayLine(_gameBoard[first].transform.position , _gameBoard[second].transform.position , Color.gray);
        } // else if 第2、第3位跟第4位一樣
        else if ( IsSameOrWild(third, forth) && IsSameOrWild(forth, fifth) && IsSameOrWild(third, fifth) )
        {   //_gameBoard[third] == _gameBoard[forth]   && _gameBoard[forth] == _gameBoard[fifth]
            // 第3、第4位跟第5位一樣, 但和第1、第2位不一樣
            DrawPayLine(_gameBoard[third].transform.position ,  _gameBoard[forth].transform.position, Color.yellow);
            DrawPayLine(_gameBoard[forth].transform.position ,  _gameBoard[fifth].transform.position , Color.yellow);
            // 畫位置3~5的賠付線

            SetGoldFrame(third);
            SetGoldFrame(forth);
            SetGoldFrame(fifth);
            goldFrameSetted = true;
            SlotGameCoinBoard.slotCoin += betCoin  * 3;

            DrawPayLine(_gameBoard[first].transform.position, _gameBoard[second].transform.position , Color.gray);
            DrawPayLine(_gameBoard[second].transform.position,   _gameBoard[third].transform.position, Color.gray);
        } // else if 第3、第4位跟第5位一樣*/

        if (goldFrameSetted)
        {
            // Debug.Log(goldFrameSetted);
            gettingPoint.Invoke();
        } //if
       else
       {
            // Debug.Log(goldFrameSetted);
            gettingNothing.Invoke();
       } // else    
     } // check pay line pattern

    bool IsSameOrWild(int first, int second)
    {
        if (_gameBoard[first].name == _gameBoard[second].name)
            return true; // if both are same icon
        else if (_gameBoard[first].name == "Wild")
            return true;  // else if first is wild
        else if (_gameBoard[second].name == "Wild")
            return true;  // else if second is wild
        else return false;
    } // is wild

    public void DrawPayLine( Vector3 begin, Vector3 end, Color lineColor )
    {    // 畫賠付線 位子從begin畫到end

        GameObject payLine = new GameObject();
        payLine.transform.position = begin;
        payLine.AddComponent<LineRenderer>();
        LineRenderer lineRenderer = payLine.GetComponent<LineRenderer>();
       
        lineRenderer.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
        Debug.Log("Line material is setted.\n");
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(lineColor, 0.5f), new GradientColorKey(lineColor, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) }
        );
        lineRenderer.colorGradient = gradient;

        if (lineColor == Color.yellow) lineRenderer.sortingOrder = 3;
        else lineRenderer.sortingOrder = 2;

        lineRenderer.startWidth = .1f;
        lineRenderer.endWidth = .1f;
        lineRenderer.SetPosition(0, begin);
        lineRenderer.SetPosition(1, end);
        _matchLines.Add(payLine);
    } // draw pay line

    void SetGoldFrame( int gameBoardindex )
    {  // set gold frame to icon's position
        GameObject tempFrame = Instantiate(GoldFrame, _gameBoard[gameBoardindex].transform.position, Quaternion.identity);
        _goldFrames.Add(tempFrame);

    } // set gold frame

    void StopSpin()
    {
        StopCoroutine("WaitingEndSpin");
        ReadPattern();
        
    } // stop spin
    private IEnumerator WaitingEndSpin(float duration)
    {
       
        yield return new WaitForSeconds(duration);
        StopSpin();

    } //  WaitToEndSpin

    private IEnumerator BottonCooLDown(float duration)
    {
        yield return new WaitForSeconds(duration);
        _isSpinning = false;

    } //  WaitToEndSpin

    private IEnumerator CheckOutOfCoin(float duration)
    {
        yield return new WaitForSeconds(duration);
        if (SlotGameCoinBoard.slotCoin <= 0) SlotGameCoinBoard.whenStop_isOutOfCoin = true;

    } //  CheckOutOfCoin
    /*void Update()
   { 

   } // update*/
}
