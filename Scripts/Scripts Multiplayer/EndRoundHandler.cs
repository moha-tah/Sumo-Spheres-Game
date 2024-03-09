using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndRoundHandler : MonoBehaviour
{
    public AudioSource audioSource;

    public AudioClip[] eliminationSounds;

    public AudioClip endRound;

    public AudioClip startGame;

    void SetStartFalse()
    {
        GetComponent<Animator>().SetBool("Start", false);
    }

    void PlaySound()
    {
        audioSource.PlayOneShot(endRound);
    }

    void PlayEliminationSound()
    {
        audioSource.PlayOneShot(eliminationSounds[Random.Range(0, eliminationSounds.Length)]);
    }

    void PlayLaunchGameSound()
    {
        audioSource.PlayOneShot(startGame);
    }
}
