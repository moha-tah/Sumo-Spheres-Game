using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public float timeScale;

    public bool isPaused = false;

    public GameObject spawnManager;
    public GameObject pauseMenu;
    public GameObject slider;
    public GameObject pauseText;

    // Start is called before the first frame update
    void Start()
    {
        timeScale = Time.timeScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !spawnManager.GetComponent<SpawnManager>().gameOver)
        {
            if (!isPaused)
            {
                isPaused = true;
                pauseMenu.SetActive(true);
                slider.SetActive(true);

                Time.timeScale = 0;

                pauseText.SetActive(false);

            } else
            {
                Time.timeScale = timeScale;

                isPaused = false;
                pauseMenu.SetActive(false);
                slider.SetActive(false);

                pauseText.SetActive(true);
            }
        }

        if (spawnManager.GetComponent<SpawnManager>().gameOver)
        {
            pauseText.SetActive(false);
        }
    }
}
