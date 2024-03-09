using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketMovement : MonoBehaviour
{
    // public int rocketsNumber = 3;

    public float impulseForce = 10;

    public GameObject target;

    private Rigidbody rocketRb;

    private AudioSource audioSource;

    public AudioClip rocketSound;
    public AudioClip explosionSound;

    public ParticleSystem explosion;
    public ParticleSystem explosion2;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioSource>();

        rocketRb = GetComponent<Rigidbody>();
        audioSource.PlayOneShot(rocketSound, 1);
        
    }

    // Update is called once per frame
    void Update()
    {
        GoToTarget(target);
    }

    void GoToTarget(GameObject target)
    {
        if (target == null)
        {
            Destroy(gameObject);
        }

        transform.LookAt(target.transform);
        transform.Rotate(90, 0, 0);

        rocketRb.velocity = transform.up * impulseForce;

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Rocket"))
        {
            audioSource.PlayOneShot(explosionSound, 0.5f);
            Instantiate(explosion, transform.position, explosion.transform.rotation);
            Instantiate(explosion2, transform.position, explosion.transform.rotation);

            Destroy(gameObject);
        }
    }

}
