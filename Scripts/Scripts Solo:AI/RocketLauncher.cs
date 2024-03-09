using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class RocketLauncher : MonoBehaviour
{
    public int rocketsAmount = 5;

    public GameObject rocketPrefab;

    private string powerup;

    private int compteur = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        powerup = GetComponent<PlayerController>().powerup;

        if (Input.GetKeyDown(KeyCode.Space) && powerup == "Rockets")
        {
            InvokeRepeating("RocketInstantiate", 0, 0.5f);

        }
    }

    void RocketInstantiate()
    {
        compteur++;

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies)
        {
            Vector3 lookDirection = (enemy.transform.position - transform.position).normalized;

            GameObject newRocket = Instantiate(rocketPrefab, transform.position + new Vector3(0, 3, 0), rocketPrefab.transform.rotation);

            newRocket.GetComponent<RocketMovement>().target = enemy;
        }

        if (compteur == rocketsAmount)
        {
            compteur = 0;
            CancelInvoke("RocketInstantiate");

            PlayerController player = GameObject.Find("Player Sphere").GetComponent<PlayerController>();

            player.powerupIndicator.SetActive(false);
            player.powerup = "Null";
        }
    }
}