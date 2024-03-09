using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using System;

public class PlayerNetwork : NetworkBehaviour
{
    public float xRange;
    public float zRange;

    public float impulseForce;

    private float hInput;
    private float vInput;

    private Rigidbody playerRb;

    private Transform focalPoint;

    public Material material;

    public GameObject nameObject;
    private TextMeshProUGUI nameBox;

    private GameManager gameManager;
    
    public Dictionary<ulong, string> playersIdsNames = new Dictionary<ulong, string>();

    public Dictionary<ulong, Color> playersIdsColors = new Dictionary<ulong, Color>();

    public bool isAlive = true;

    public override void OnNetworkSpawn()
    {
        GetReferences();

        if (IsOwner)
            OnConnect();

        if (IsServer)
        {
            SetNewScore();
            StartCoroutine(gameManager.OnPlayerSpawned(this));
        }
    }

    public override void OnDestroy()
    {
        OnDisconnect();
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;

        hInput = Input.GetAxis("Horizontal");
        vInput = Input.GetAxis("Vertical");

        if (hInput != 0f || vInput != 0f)
        {
            Vector3 force = impulseForce * Time.deltaTime * (focalPoint.right * hInput + focalPoint.forward * vInput);

            MovePlayerServerRpc(force);
        }
    }

    private void GetReferences()
    {
        playerRb = GetComponent<Rigidbody>();

        focalPoint = GameObject.Find("Focal Point").GetComponent<Transform>();

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        nameBox = nameObject.GetComponent<TextMeshProUGUI>();
    }

    private void OnConnect()
    {
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
            player.name = player.GetComponent<NetworkObject>().OwnerClientId.ToString();

        SpawningPositionServerRpc();

        string name;

        if (PlayerPrefs.HasKey("Name"))
            name = PlayerPrefs.GetString("Name");
        else name = "Inconnu";

        SetNameServerRpc(name);

        float r = UnityEngine.Random.value;
        float g = UnityEngine.Random.value;
        float b = UnityEngine.Random.value;

        SetRandomColorServerRpc(new Color(r, g, b, 1f));

        gameManager.SetScores();

    }

    public void OnDisconnect()
    {
        gameManager.QuitIfNoHost();

        gameManager.SetScoreUI();

        if (!IsServer)
        {   
            return;
        }

        ulong oldSum = 0;

        foreach(GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            oldSum += player.GetComponent<NetworkObject>().OwnerClientId;
        }

        ulong newSum = 0;

        foreach(var id in playersIdsNames.Keys)
        {
            newSum += id;
        }

        ulong leftID = newSum - oldSum;

        int newArrayID = 0;

        while (newArrayID < gameManager.idsList.Count &&
               gameManager.idsList[newArrayID] != leftID)
            newArrayID++;

        StartCoroutine(gameManager.SetIdScore(newArrayID, 99));

        playersIdsNames.Remove(leftID);
        playersIdsColors.Remove(leftID);

        gameManager.playersCount.Value--;

        if (gameManager.playersCount.Value == 1)
        {
            gameManager.LeaveGame();
        }

        if (isAlive)
            gameManager.alivePlayersCount.Value--;

    }

    public void SetAlive()
    {
        isAlive = true;
    }

    void SetNewScore()
    {
        int rightArrayID = 0;

        while (rightArrayID < gameManager.idsList.Count
               && gameManager.idsList[rightArrayID] != 99)
        {
            rightArrayID++;
        }

        StartCoroutine(gameManager.SetIdScore(rightArrayID, OwnerClientId));
    }

    // RPC //



    [ServerRpc]
    private void MovePlayerServerRpc(Vector3 force)
    {
        playerRb.AddForce(force, ForceMode.Impulse);
    }

    [ServerRpc]
    private void SpawningPositionServerRpc()
    {   
        float xPos = UnityEngine.Random.Range(-xRange, xRange);
        float zPos = UnityEngine.Random.Range(-zRange, zRange);

        if (gameManager.currentState.Value != GameManager.State.Round)
            transform.position = new Vector3(xPos, transform.position.y, zPos);
        else
            transform.position = new Vector3(0, -25, 0);
    }

    [ServerRpc]
    private void SetNameServerRpc(string name)
    {
        gameObject.name = OwnerClientId.ToString();
        nameBox.text = name;

        GameObject[] arrayPlayers = GameObject.FindGameObjectsWithTag("Player");

        Dictionary<ulong, string> realPlayersIdsNames = playersIdsNames;

        foreach (GameObject player in arrayPlayers)
        {
            if (player.GetComponent<NetworkObject>().OwnerClientId == 0)
            {
                // on ajoute les id/names au dico du host.
                realPlayersIdsNames = player.GetComponent<PlayerNetwork>().playersIdsNames;
                realPlayersIdsNames.Add(OwnerClientId, name);
            }
        }

        // on transfère les id/names au dico de tous les clients.
        playersIdsNames = realPlayersIdsNames;

        foreach (KeyValuePair<ulong, string> idName in playersIdsNames)
        {
            SendNamesClientRpc(idName.Key, idName.Value);
        }
        
    }

    [ServerRpc]
    void SetRandomColorServerRpc(Color color)
    {
        GetComponent<Renderer>().material.color = color;
        GetComponent<TrailRenderer>().startColor = color;
        GetComponent<TrailRenderer>().endColor = new Color(color.r, color.g, color.b, 0f);

        GameObject[] arrayPlayers = GameObject.FindGameObjectsWithTag("Player");

        Dictionary<ulong, Color> realPlayersIdsColors = playersIdsColors;

        foreach (GameObject player in arrayPlayers)
        {
            if (player.GetComponent<NetworkObject>().OwnerClientId == 0)
            {
                // on ajoute les id/names au dico du host.
                realPlayersIdsColors = player.GetComponent<PlayerNetwork>().playersIdsColors;
                realPlayersIdsColors.Add(OwnerClientId, color);
            }
        }

        // on transfère les id/names au dico de tous les clients.
        playersIdsColors = realPlayersIdsColors;

        foreach (KeyValuePair<ulong, Color> idColor in playersIdsColors)
        {
            SendColorsClientRpc(idColor.Key, idColor.Value);
        }
    }

    [ClientRpc]
    private void SendNamesClientRpc(ulong id, string name)
    {
        GameObject[] arrayPlayers = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in arrayPlayers)
        {
            if (player.GetComponent<NetworkObject>().OwnerClientId == id)
            {
                player.transform.Find("Canvas").transform.Find("Name").GetComponent<TextMeshProUGUI>().text = name;
            }
        }
    }

    [ClientRpc]
    private void SendColorsClientRpc(ulong id, Color color)
    {
        GameObject[] arrayPlayers = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in arrayPlayers)
        {
            if (player.GetComponent<NetworkObject>().OwnerClientId == id)
            {
                player.GetComponent<Renderer>().material.color = color;
                player.GetComponent<TrailRenderer>().startColor = color;
                player.GetComponent<TrailRenderer>().endColor = new Color(color.r, color.g, color.b, 0f);
            }
        }
    }
}