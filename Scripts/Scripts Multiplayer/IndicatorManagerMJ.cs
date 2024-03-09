using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorManagerMJ : MonoBehaviour
{
    public float rotateSpeed;

    private float ecart;

    private Transform player;
    private PlayerControllerMJ playerControllerScript;

    private string powerup;

    public GameObject forceIcon;
    public GameObject jumpIcon;
    public GameObject rocketsIcon;
    public GameObject noPowerup;

    public Material yellowIndicator;
    public Material redIndicator;
    public Material greenIndicator;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0, 100 * rotateSpeed * Time.deltaTime, 0));
        transform.position = player.position - new Vector3(0, ecart, 0);
    }

    void OnEnable()
    {
        player = GameObject.Find("Player Sphere").GetComponent<Transform>();
        playerControllerScript = GameObject.Find("Player Sphere").GetComponent<PlayerControllerMJ>();

        ecart = player.position.y - transform.position.y;

        DisableAll();

        ActivePowerup().SetActive(true);
    }

    void OnDisable()
    {
        DisableAll();
    }

    GameObject ActivePowerup()
    {
        powerup = playerControllerScript.powerup;

        GameObject powerupIcon;

        if (powerup == "Force")
        {
            powerupIcon = forceIcon;
            GetComponent<Renderer>().material = yellowIndicator;
        }
        else if (powerup == "Jump")
        {
            powerupIcon = jumpIcon;
            GetComponent<Renderer>().material = greenIndicator;
        }
        else if (powerup == "Rockets")
        {
            powerupIcon = rocketsIcon;
            GetComponent<Renderer>().material = redIndicator;
        }
        else
        {
            powerupIcon = noPowerup;
        }

        return powerupIcon;
    }

    void DisableAll()
    {
        forceIcon.SetActive(false);
        jumpIcon.SetActive(false);
        rocketsIcon.SetActive(false);
        noPowerup.SetActive(false);
    }
}
