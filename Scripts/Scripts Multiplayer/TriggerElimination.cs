using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class TriggerElimination : NetworkBehaviour
{
    private GameManager gameManager;

    private AudioSource audioSource;

    public AudioClip[] eliminationSounds;

    public AudioClip collisionSound;

    private Animator eliminationAnimator;

    private void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        eliminationAnimator = GameObject.Find("Canvas Elimination").GetComponent<Animator>();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Triggerer") && gameManager.currentState.Value == GameManager.State.Round)
        {
            if (IsServer)
                gameManager.alivePlayersCount.Value--;

            gameObject.GetComponent<PlayerNetwork>().isAlive = false;
            gameManager.CheckEndRound();

            if (IsOwner && gameManager.alivePlayersCount.Value > 1)
            {
                eliminationAnimator.SetBool("Start", true);
            }
        }
    }

    public void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            audioSource.PlayOneShot(collisionSound);
        }
    }
}
