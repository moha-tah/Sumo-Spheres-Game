using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DanceEffect : MonoBehaviour
{
    public AudioSource audioSource;

    public AudioClip danceSound;

    void DanceSound()
    {
        audioSource.PlayOneShot(danceSound);
    }

    void DisableDance()
    {
        GetComponent<Animator>().SetBool("Dance", false);
    }
}
