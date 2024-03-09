using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetJoinCode : MonoBehaviour
{
    public NetworkManagerUI networkManagerUI;

    public void OnValueChanged(string joinCode)
    {
        networkManagerUI.waitingJoinCode = joinCode.ToUpper();
    }
}
