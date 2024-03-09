using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreSlider : MonoBehaviour
{

    public GameObject scoreMaxText;

    public GameObject gameManager;

    public void OnMaxScoreChanged(float score)
    {
        scoreMaxText.GetComponent<TextMeshProUGUI>().text = "Score max : " + score;

        gameManager.GetComponent<GameManager>().winScore.Value = (int)score;
    }
}
