using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float impulseForce;
    public float gravityModifier;

    public GameObject powerupIndicator;

    private float hInput;
    private float vInput;

    private Rigidbody playerRb;
    private Transform focalPoint;
    private CameraShake cameraShake;

    // [HideInInspector]
    public string powerup = "Null";

    public int powerupTimer;

    public float hangTime;
    public float smashSpeed;
    public float explosionForce;
    public float explosionRadius;

    private float floorY;

    private Vector3 firstPlayerPos;

    private SpawnManager spawnManager;

    private AudioSource audioSource;

    public AudioClip jumpSound;
    public AudioClip powerupSound;
    public AudioClip eqSound;

    public ParticleSystem explosion;

    // Start is called before the first frame update
    void Start()
    {
        if (!PlayerPrefs.HasKey("Volume"))
            PlayerPrefs.SetFloat("Volume", 1);

        cameraShake = GameObject.Find("Camera Shake").GetComponent<CameraShake>();

        spawnManager = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();

        audioSource = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioSource>();

        firstPlayerPos = GetComponent<Transform>().position;
        playerRb = GetComponent<Rigidbody>();
        focalPoint = GameObject.Find("Focal Point").GetComponent<Transform>();

        Physics.gravity *= gravityModifier;
    }

    // Update is called once per frame
    void Update()
    {
        hInput = Input.GetAxis("Horizontal");
        vInput = Input.GetAxis("Vertical");

        playerRb.AddForce(impulseForce * (focalPoint.right * hInput + focalPoint.forward * vInput)
                          * Time.deltaTime, ForceMode.Impulse);

        JumpCheck();



    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Powerup"))
        {
            audioSource.PlayOneShot(powerupSound, 1f);

            powerup = other.GetComponent<PowerupController>().type;
            powerupIndicator.SetActive(true);

            StartCoroutine(PowerupCountdown());

            GameObject[] onMapPowerups = GameObject.FindGameObjectsWithTag("Powerup");

            foreach (GameObject onMapPowerup in onMapPowerups)
            {
                Destroy(onMapPowerup);
            }
        }

    }

    IEnumerator PowerupCountdown()
    {
        yield return new WaitForSeconds(powerupTimer);
        powerupIndicator.SetActive(false);
        powerup = "Null";
    }

    void JumpCheck()
    {
        if (Input.GetKeyDown(KeyCode.Space) && powerup == "Jump")
        {
            audioSource.PlayOneShot(jumpSound, 1f);

            StartCoroutine(Jump());


            powerupIndicator.SetActive(false);
            powerup = "Null";
        }
    }

    IEnumerator Jump()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        floorY = transform.position.y;

        float jumpTime = Time.time + hangTime;

        while (Time.time < jumpTime)
        {
            playerRb.velocity = new Vector2(playerRb.velocity.x, smashSpeed);
            yield return null;
        }

        while (transform.position.y > floorY)
        {
            playerRb.velocity = new Vector2(playerRb.velocity.x, -smashSpeed * 2);
            yield return null;
        }

        audioSource.PlayOneShot(eqSound, 1f);
        Instantiate(explosion, transform.position, explosion.transform.rotation);
        cameraShake.shakeDuration = 0.4f;

        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] != null)
            {
                enemies[i].GetComponent<Rigidbody>().AddExplosionForce(explosionForce,
                                        transform.position, explosionRadius, 1.0f, ForceMode.Impulse);
            }

        }
    }
}