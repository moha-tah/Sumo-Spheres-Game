using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class FirstPlaceIcon : NetworkBehaviour
{
    public Transform followedPlayer;

    public float offset;

    private Transform mainCamera;

    private void Awake()
    {
        mainCamera = GameObject.Find("Main Camera").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (followedPlayer != null)
            FollowPlayer();
    }

    void FollowPlayer()
    {
        transform.LookAt(mainCamera);
        transform.position = new Vector3(followedPlayer.position.x, followedPlayer.position.y + offset, followedPlayer.position.z);
    }

    [ClientRpc]
    public void findFirstPlaceClientRpc(ulong id)
    {
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (player.GetComponent<NetworkObject>().OwnerClientId == id)
                followedPlayer = player.transform;
        }
    }
}
