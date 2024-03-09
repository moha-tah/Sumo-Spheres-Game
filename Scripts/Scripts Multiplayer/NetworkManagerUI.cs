using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NetworkManagerUI : MonoBehaviour
{
    public Button hostButton;
    public Button clientButton;

    public GameManager gameManager;

    public GameObject relayManager;

    public string waitingJoinCode;

    // Start is called before the first frame update
    private void Awake()
    {
        hostButton.onClick.AddListener(() =>
        {
            relayManager.GetComponent<RelayManager>().CreateRelay();

            GoOnServer();

            gameManager.inLobbyUI.SetActive(true);
        }
        );

        clientButton.onClick.AddListener(() =>
        {
            if (waitingJoinCode.Length != 6) return;

            relayManager.GetComponent<RelayManager>().JoinRelay(waitingJoinCode);

            NetworkManager.Singleton.StartClient();

            GoOnServer();

        }
        );
    }

    private void GoOnServer()
    {
        gameManager.QuitMainMenu();
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
}
