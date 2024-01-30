using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class ObjectShine : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer.DOColor(Color.yellow, 0.2f).SetLoops(4, LoopType.Yoyo).ChangeStartValue(spriteRenderer.color);
        
    } // start

    // Update is called once per frame
    //void Update()

}
