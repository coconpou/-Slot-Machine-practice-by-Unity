using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.Events;



public class GameManager : MonoBehaviour
{
   
    [SerializeField] private int boardHeight, boardWidth;
    // �Ѫ���}�C�����e
    [SerializeField] private GameObject[] icons;
    [SerializeField] private GameObject GoldFrame;  // ����e�����خ�
    // [SerializeField] private 
    [SerializeField] private UnityEvent gettingPoint;
    [SerializeField] private UnityEvent gettingNothing;
    [SerializeField] private UnityEvent BetCoin_isZero;
    [SerializeField] private UnityEvent BetCoinNotEnough;

    private List<GameObject> _matchLines;
    // ���X�عϮ�
    private List<GameObject> _goldFrames;
    // �s�u���خ�

    private int numbersOficon;
    private int betCoin;
    private GameObject _board;  // �C������
    private GameObject[] _gameBoard; // �}�C�j�p

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
        numbersOficon = boardHeight * boardWidth;// 5X4���Ѫ���}�C
        _gameBoard = new GameObject[numbersOficon];  // �`��20
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
        {   //���m[ i ]��icon�H���ᤩ���P��icon(�i�H����)
            GameObject gridPosition = _board.transform.Find(i.ToString()).gameObject;
            GameObject iconType = icons[UnityEngine.Random.Range(0, icons.Length)];
            if (iconType.name == "Wild") iconType = icons[UnityEngine.Random.Range(0, icons.Length)];
            // ���Cwild�����v
            GameObject thisIcon = Instantiate(iconType, gridPosition.transform.position, Quaternion.identity);
            thisIcon.name = iconType.name;
            thisIcon.transform.parent = gridPosition.transform;
            _gameBoard[i] = thisIcon; // ��igameBoard

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
                int first, second, third, forth, fifth; // �s�u��m
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
    { // �T�{�Ҧ��P�����ۦP

        if ( IsSameOrWild(first,second)&&IsSameOrWild(second,third)&&IsSameOrWild(first,third) )
        {    // _gameBoard[first] == _gameBoard[second] && _gameBoard[second] == _gameBoard[third]
            // �e�T��@��
            DrawPayLine(_gameBoard[first].transform.position  ,  _gameBoard[second].transform.position  , Color.yellow );
            DrawPayLine(_gameBoard[second].transform.position  , _gameBoard[third].transform.position  , Color.yellow);
            // �e��m1~3���ߥI�u

            SetGoldFrame(first);
            SetGoldFrame(second);
            SetGoldFrame(third);
            goldFrameSetted = true;

            SlotGameCoinBoard.slotCoin += betCoin * 2;
            if (IsSameOrWild(third, forth) && IsSameOrWild(second, forth) )
            {   // _gameBoard[third] == _gameBoard[forth]
                // �ĤT�B�ĥ|��@��
                DrawPayLine(_gameBoard[third].transform.position  ,   _gameBoard[forth].transform.position  , Color.yellow);

                SetGoldFrame(forth);
                SlotGameCoinBoard.slotCoin += betCoin * 3;
                if (_gameBoard[forth].name == _gameBoard[fifth].name
                    || (IsSameOrWild(forth, fifth) && IsSameOrWild(third, fifth)))
                {   // �ĥ|�B�Ĥ���@��
                    DrawPayLine(_gameBoard[forth].transform.position  , _gameBoard[fifth].transform.position  , Color.yellow);
                    SetGoldFrame(fifth);
                    SlotGameCoinBoard.slotCoin += betCoin * 5;
                } // if
                else
                {   // �ĥ|�B�Ĥ��줣�@��
                    DrawPayLine(_gameBoard[forth].transform.position ,  _gameBoard[fifth].transform.position, Color.gray);
                } // else
            } // if
            else
            {   // �ĥ|�B�Ĥ����ĤT�줣�@��
                DrawPayLine(_gameBoard[third].transform.position  ,  _gameBoard[forth].transform.position  , Color.gray);
                DrawPayLine(_gameBoard[forth].transform.position  ,  _gameBoard[fifth].transform.position  , Color.gray);
            } // else
        } // if �e�T��@��

        /*else if ( IsSameOrWild(second, third) && IsSameOrWild(third, forth) && IsSameOrWild(second, forth) )
        {  // _gameBoard[second] == _gameBoard[third] && _gameBoard[third]== _gameBoard[forth]
            // ��2�B��3����4��@��, ���M�Ĥ@�줣�@��
            DrawPayLine(_gameBoard[second].transform.position  , _gameBoard[third].transform.position  , Color.yellow);
            DrawPayLine(_gameBoard[third].transform.position  , _gameBoard[forth].transform.position , Color.yellow);
            // �e��m2~4���ߥI�u

            SetGoldFrame(second);
            SetGoldFrame(third);
            SetGoldFrame(forth);
            goldFrameSetted = true;
            SlotGameCoinBoard.slotCoin += betCoin  * 2;
            if (_gameBoard[forth].name == _gameBoard[fifth].name)
            {   // �ĥ|�B�Ĥ���@��
                DrawPayLine(_gameBoard[forth].transform.position  , _gameBoard[fifth].transform.position,  Color.yellow);
                SetGoldFrame(fifth);
                SlotGameCoinBoard.slotCoin += betCoin  * 3;
            } // if�ĥ|�B�Ĥ���@��
            else
            { //���@��
                DrawPayLine(_gameBoard[forth].transform.position ,  _gameBoard[fifth].transform.position , Color.gray);
            } // else 
            DrawPayLine(_gameBoard[first].transform.position , _gameBoard[second].transform.position , Color.gray);
        } // else if ��2�B��3����4��@��
        else if ( IsSameOrWild(third, forth) && IsSameOrWild(forth, fifth) && IsSameOrWild(third, fifth) )
        {   //_gameBoard[third] == _gameBoard[forth]   && _gameBoard[forth] == _gameBoard[fifth]
            // ��3�B��4����5��@��, ���M��1�B��2�줣�@��
            DrawPayLine(_gameBoard[third].transform.position ,  _gameBoard[forth].transform.position, Color.yellow);
            DrawPayLine(_gameBoard[forth].transform.position ,  _gameBoard[fifth].transform.position , Color.yellow);
            // �e��m3~5���ߥI�u

            SetGoldFrame(third);
            SetGoldFrame(forth);
            SetGoldFrame(fifth);
            goldFrameSetted = true;
            SlotGameCoinBoard.slotCoin += betCoin  * 3;

            DrawPayLine(_gameBoard[first].transform.position, _gameBoard[second].transform.position , Color.gray);
            DrawPayLine(_gameBoard[second].transform.position,   _gameBoard[third].transform.position, Color.gray);
        } // else if ��3�B��4����5��@��*/

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
    {    // �e�ߥI�u ��l�qbegin�e��end

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
