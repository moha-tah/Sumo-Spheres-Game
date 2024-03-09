using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayersCountUI : MonoBehaviour
{
    private TextMeshProUGUI textBox;

    public GameManager gameManager;

    void Awake()
    {
        textBox = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetText(int currentAlivePlayers, int currentTotalPlayers)
    {
        string s = "s";
        if (currentTotalPlayers < 2)
            s = "";

        if (gameManager.currentState.Value == GameManager.State.Round)
            textBox.text = currentAlivePlayers + "/" + currentTotalPlayers + " joueur" + s +" en vie";
        else
            textBox.text = currentTotalPlayers + " joueur" + s + " connecté" + s;
    }

}
