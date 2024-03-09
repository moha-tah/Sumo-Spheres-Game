using UnityEngine;
using TMPro;
using Unity.Netcode;

public class AttachName : MonoBehaviour
{

    private Camera mainCamera;

    public Transform player;

    private TextMeshProUGUI nameBox;

    void Awake()
    {
        mainCamera = Camera.main;
        nameBox = GetComponent<TextMeshProUGUI>();

        bool isOwner = player.gameObject.GetComponent<NetworkBehaviour>().IsOwner;

        //SetNameServerRpc();

    }

    // Met à jour la position du texte
    void LateUpdate()
    {
        transform.position = player.position + Vector3.up * 2;
        transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
            mainCamera.transform.rotation * Vector3.up);
    }

    [ServerRpc]
    public void SetNameServerRpc()
    {
        if (PlayerPrefs.HasKey("Name"))
            nameBox.text = PlayerPrefs.GetString("Name");
        else nameBox.text = "Inconnu";

        Debug.Log("Prénom : " + nameBox.text);
    }
}