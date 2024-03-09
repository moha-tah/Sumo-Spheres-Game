using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float impulseForce;

    private Rigidbody enemyRb;

    private GameObject player;

    private Vector3 lookDirection;

    public AudioClip kickSound;
    public AudioClip collisionSound;
    private AudioSource audioSource;

    public ParticleSystem kick;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioSource>();

        enemyRb = GetComponent<Rigidbody>();

        player = GameObject.Find("Player Sphere");
    }

    // Update is called once per frame
    void Update()
    {
        lookDirection = (player.transform.position - transform.position).normalized;

        if (transform.position.y > 0)
        {
            enemyRb.AddForce(impulseForce * lookDirection
                         * Time.deltaTime, ForceMode.Impulse);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        audioSource.PlayOneShot(collisionSound, 1);

        PlayerController player = GameObject.Find("Player Sphere").GetComponent<PlayerController>();

        if (collision.gameObject.CompareTag("Player") && player.powerup == "Force")
        {
            ApplyForcePowerUp(1000);
            audioSource.PlayOneShot(kickSound, 1);
            Instantiate(kick, transform.position, kick.transform.rotation);

            player.powerupIndicator.SetActive(false);
            player.powerup = "Null";
        }
    }

    public void ApplyForcePowerUp(int impulse)
    {
        enemyRb.AddForce(impulse * impulseForce * (-lookDirection) * Time.deltaTime, ForceMode.Impulse);
    }
}

