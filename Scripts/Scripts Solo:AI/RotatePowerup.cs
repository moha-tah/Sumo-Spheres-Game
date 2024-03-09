using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatePowerup : MonoBehaviour
{
    public float maximumHeight;
    public float heightSpeed;

    public float rotateSpeed;

    private Transform powerUp;

    // Start is called before the first frame update
    void Start()
    {
        powerUp = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        float height = Mathf.PingPong(Time.time * heightSpeed, maximumHeight) + 0.5f;

        powerUp.position = new Vector3(powerUp.position.x, height, powerUp.position.z);

        powerUp.Rotate(new Vector3(0, 100 * rotateSpeed * Time.deltaTime, 0));
    }
}
