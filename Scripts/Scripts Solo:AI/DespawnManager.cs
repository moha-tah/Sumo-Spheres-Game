using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DespawnManager : MonoBehaviour
{
    public float minY;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < minY)
        {
            GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();
            

            if (CompareTag("Player"))
            {

                GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

                foreach (GameObject enemy in enemies)
                {
                    Destroy(enemy);
                }
            } else
            {
                Destroy(gameObject);
            }

            if (name == "Boss Sphere(Clone)")
            {
                GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

                foreach (GameObject enemy in enemies)
                {
                    Destroy(enemy);
                }

                CancelInvoke("SpawnMiniBoss");
            }
        }
    }

}
