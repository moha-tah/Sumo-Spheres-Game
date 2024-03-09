using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SetName : MonoBehaviour
{
    private TMP_InputField nameBox;

    void Awake()
    {
        nameBox = GetComponent<TMP_InputField>();
        if (PlayerPrefs.HasKey("Name"))
            nameBox.text = PlayerPrefs.GetString("Name");
    }

    public void OnValueChanged(string nameToSet)
    {
        PlayerPrefs.SetString("Name", nameToSet);
    }
}
