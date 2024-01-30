using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundFX : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip getPointClip;
    [SerializeField] private AudioClip getNothingClip;

    // Start is called before the first frame update
    public void PlayGetPoint()
    {  // when get point, play SoundFX
        audioSource.clip = getPointClip;
        audioSource.volume = 0.4f;
        audioSource.Play();
    } // Play Get Point

    public void PlayGetNothing()
    {
        audioSource.clip = getNothingClip;
        audioSource.volume = 0.4f;
        audioSource.Play();
    } // Play Get Nothing

    // Update is called once per frame
    //void Update()
    
}
