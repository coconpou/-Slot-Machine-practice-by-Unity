using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SpinAnimationController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    private Vector2 initPosition;
    private float stopYPosition;

    float speed;
    //[SerializeField] private GameObject spinBackground;
    void Start()
    {
        initPosition = new Vector2(transform.position.x, transform.position.y);
        speed = 10.0f;
        stopYPosition = -3.9f;
        StartSpin();
    } // start spin

    //IEnumerator
    void StartSpin()
    {
        speed = Random.Range(-10f, -29f);
        rb.velocity = new Vector2(rb.velocity.x, speed);

        // yield return new WaitForSeconds(2);
        //stop = true;
        StopCoroutine("WaitToEndSpin");
        StartCoroutine(WaitToEndSpin(1.0f));
    } // start spin
      // Update is called once per frame
    private IEnumerator WaitToEndSpin(float duration)
    {
        yield return new WaitForSeconds(duration);
        StopSpin();
    } //  WaitToEndSpin

    void CheckBottom()
    {
        if (transform.position.y < stopYPosition)
        {
            //Debug.Log("attach bottom");
            transform.position = new Vector2(transform.position.x, Random.Range(3.8f, 3.9f));
        } // if
    } // check is bottom
    void StopSpin()
    {
        StopCoroutine("WaitToEndSpin");
        transform.position = initPosition;
        rb.velocity = Vector3.zero;
        rb.Sleep();
    } // stop spin
    void Update()
    {
        CheckBottom();
        //StartSpin();

    } // update
}
