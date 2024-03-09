using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManagerMJ : MonoBehaviour
{

    public bool isPaused = false;

    public GameObject gameManager;
    public GameObject pauseMenu;
    public GameObject slider;
    public GameObject pauseText;
    public GameObject tabText;

    public GameObject tabUI;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        CheckPause();

        CheckTab();
    }

    void CheckPause()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && gameManager.GetComponent<GameManager>().currentState.Value != GameManager.State.Menu)
        {
            if (!isPaused)
            {
                isPaused = true;
                pauseMenu.SetActive(true);
                slider.SetActive(true);

                pauseText.SetActive(false);
                tabText.SetActive(false);

            }
            else
            {
                isPaused = false;
                pauseMenu.SetActive(false);
                slider.SetActive(false);

                pauseText.SetActive(true);
                tabText.SetActive(true);
            }
        }
    }

    void CheckTab()
    {
        GameManager.State state = gameManager.GetComponent<GameManager>().currentState.Value;

        if (Input.GetKeyDown(KeyCode.Tab)
            && (state == GameManager.State.Game || state == GameManager.State.Round))
        {
            tabUI.SetActive(true);
        } else if (Input.GetKeyUp(KeyCode.Tab))
        {
            tabUI.SetActive(false);
        }
        
    }
}


