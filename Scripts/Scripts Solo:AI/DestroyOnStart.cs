using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnStart : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (!GameObject.Find("Spawn Manager").GetComponent<SpawnManager>().gameOver)
            Destroy(gameObject);
    }
}
