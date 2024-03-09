using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System;

public class GameManager : NetworkBehaviour
{
	private PlayerNetwork playerNetwork;

	public GameObject slider;
	public GameObject mainMenu;
	public GameObject tab;
	public GameObject pause;

	public GameObject inGameUI;

	public GameObject inLobbyUI;

	public NetworkVariable<int> playersCount = new NetworkVariable<int>(0,
											NetworkVariableReadPermission.Everyone,
											NetworkVariableWritePermission.Server);

	public NetworkVariable<int> alivePlayersCount = new NetworkVariable<int>(0,
											NetworkVariableReadPermission.Everyone,
											NetworkVariableWritePermission.Server);

	public NetworkVariable<int> winScore = new NetworkVariable<int>(5,
											NetworkVariableReadPermission.Everyone,
											NetworkVariableWritePermission.Server);

    public NetworkList<ulong> idsList;

	public NetworkList<int> scoresList;

	public GameObject invisibleWalls;

	public GameObject playersCountUI;

	public GameObject tabCanvas;

	public GameObject[] playersUI;

	public GameObject roundWinnerText;
	public Animator roundAnimator;

    public GameObject gameWinnerText;
    public Animator gameAnimator;
    public Animator danceAnimator;

    public Animator launchGameAnimator;

    public GameObject firstPlaceIconPrefab;

    public enum State
	{
		Menu,
		Lobby,
		Game,
		Round
	}

	public NetworkVariable<State> currentState = new NetworkVariable<State>(State.Menu,
											NetworkVariableReadPermission.Everyone,
											NetworkVariableWritePermission.Server);

	// Start is called before the first frame update
	void Start()
	{
		if (!PlayerPrefs.HasKey("Volume"))
			PlayerPrefs.SetFloat("Volume", 0);

        idsList = new NetworkList<ulong>(new List<ulong>(){99, 99, 99, 99, 99, 99, 99, 99, 99, 99},
                                            NetworkVariableReadPermission.Everyone,
                                            NetworkVariableWritePermission.Server);


        scoresList = new NetworkList<int>(new List<int>(){0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                                            NetworkVariableReadPermission.Everyone,
                                            NetworkVariableWritePermission.Server);

        SyncPlayersCount();
		SyncCurrentState();
		SyncCurrentScores();
    }

	// Update is called once per frame
	void Update()
	{
		if (Input.GetMouseButtonDown(1))
			roundAnimator.SetTrigger("Start");
	}

	public void QuitIfNoHost()
	{
		if (GameObject.FindGameObjectsWithTag("Player").Length == 0)
		{
			LeaveGame();
		}
	}

	public IEnumerator OnPlayerSpawned(PlayerNetwork player)
	{
		yield return new WaitForSeconds(0.1f);
		playersCount.Value++;
	}

	public IEnumerator CurrentStateYield()
	{
		yield return new WaitForSeconds(0.5f);
		currentState.Value = GameManager.State.Lobby;
	}

	void SyncPlayersCount()
	{
		playersCount.OnValueChanged += (int previousValue, int newValue) =>
		{   
			SetText();

			if (playersCount.Value == 0)
				LeaveGame();

			if (IsServer)
				SetScoreUI();
		};

		alivePlayersCount.OnValueChanged += (int previousValue, int newValue) =>
		{
			SetText();

			if (IsServer)
				SetScoreUI();
		};
	}

	void SyncCurrentState()
	{
		currentState.OnValueChanged += (State previousValue, State newValue) =>
		{
			SetScores();
			SetText();

			if (previousValue == State.Lobby && newValue == State.Round)
			{
				GameObject player = GameObject.Find(OwnerClientId.ToString());

				if (player == null) return;

				playerNetwork = player.GetComponent<PlayerNetwork>();

				// animation qui présente la nouvelle partie.
				// sound ?
			}
		};
	}

	public void SyncCurrentScores()
	{
		idsList.OnListChanged += (nothing) =>
        {
			if (currentState.Value != State.Menu)
			{
                SetScores();
                SetScoreUI();
            }
        };

        scoresList.OnListChanged += (nothing) =>
        {
            if (currentState.Value != State.Menu)
            {
                SetScores();
                SetScoreUI();
            }
        };

		winScore.OnValueChanged += (int previousScore, int newScore) =>
		{
			SetScores();
		};
	}

	void SetText()
	{
		if (playersCountUI != null)
			playersCountUI.GetComponent<PlayersCountUI>().SetText(alivePlayersCount.Value, playersCount.Value);
	}

	public void SetScores()
	{
		for (int i = 0; i < 5; i++)
		{
			ulong id = idsList[i];
			int score = scoresList[i];

			if (playersUI[i] != null && id != 99)
				playersUI[i].transform.Find("Stars").GetComponent<TextMeshProUGUI>().text = StarsText(i);
		}
	}

	public IEnumerator SetIdScore(int i, ulong id)
	{
		yield return new WaitForSeconds(.1f);
        idsList[i] = id;
        scoresList[i] = 0;
    }

	public string StarsText(int arrayID)
	{
		string stars = "";

		for (int i = 0; i < winScore.Value; i++)
		{
			if (i == 5)
				stars += "\n";

			if (i < scoresList[arrayID])
				stars += "★";
			else
				stars += "☆";
		}

		return stars;
	}

	public void SetScoreUI()
	{
		GameObject player = GameObject.Find(OwnerClientId.ToString());

		if (player == null) return;

		playerNetwork = player.GetComponent<PlayerNetwork>();

		Dictionary<ulong, string> playersIdsNames = playerNetwork.playersIdsNames;
		Dictionary<ulong, Color> playersIdsColors = playerNetwork.playersIdsColors;

		ulong[] sortedNamesKeys = playersIdsColors.Keys.OrderBy(key => key).ToArray();

		ulong[] sortedColorsKeys = playersIdsColors.Keys.OrderBy(key => key).ToArray();

		for (int i = 0; i < Mathf.Min(sortedNamesKeys.Length, playersUI.Length); i++)
		{
			SetNameUIClientRpc(i, playersIdsNames[sortedNamesKeys[i]]);

			SetColorUIClientRpc(i, playersIdsColors[sortedColorsKeys[i]]);

			// SetScoreUIClientRpc(i, playersIdsScores[sortedKeys[i]]);

		}
	}

	public void QuitMainMenu()
	{
		slider.SetActive(false);
		mainMenu.SetActive(false);

        tab.SetActive(true);
        pause.SetActive(true);

        inGameUI.SetActive(true);

		StartCoroutine(CurrentStateYield());


		Invoke("QuitIfNoHost", 5f);
	}

	public void CheckEndRound()
	{
		if (alivePlayersCount.Value == 1)
		{
			GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

			foreach(GameObject player in players)
			{
				if (player.GetComponent<PlayerNetwork>().isAlive)
					Winner(player);
			}
		}
	}

	public void Winner(GameObject winner)
	{
		if (!IsServer) return;

		ulong id = winner.GetComponent<PlayerNetwork>().OwnerClientId;

		for (int i = 0; i < idsList.Count; i++)
		{
            if (idsList[i] == id)
			{
                scoresList[i]++;

				int count = 0;

				foreach (int score in scoresList)
				{
					if (scoresList[i] <= score)
					{
						count++;
					}
                }

				if (count == 1)
					setFirstPlaceIcon(id);

                string winnerName = winner.transform.Find("Canvas").transform.Find("Name").GetComponent<TextMeshProUGUI>().text;

                if (currentState.Value == State.Round)
				{
                    if (scoresList[i] == winScore.Value)
                    {
                        StartCoroutine(SetNewLobby(winnerName));
                    }
					else
					{
                        StartCoroutine(SetNewRound(winnerName));
                    }
                }
			}
		}

		SetScoresClientRpc();

        // afficher quelque chose à l'écran pour le gagnant ?
    }

	public void SetNewPositions()
	{
		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

		foreach (GameObject player in players)
		{
			PlayerNetwork pNtwk = player.GetComponent<PlayerNetwork>();

			pNtwk.isAlive = true;

			float xPos = UnityEngine.Random.Range(-pNtwk.xRange, pNtwk.xRange);
			float zPos = UnityEngine.Random.Range(-pNtwk.zRange, pNtwk.zRange);

			player.transform.position = new Vector3(xPos, 5, zPos);

			player.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
		}
	}

	public IEnumerator SetNewRound(string name)
	{
		SetAnimatorRoundClientRpc(name);

		yield return new WaitForSeconds(2.5f);

		alivePlayersCount.Value = playersCount.Value;

		SetNewPositions();
	}

	public IEnumerator SetNewLobby(string name)
	{
        SetAnimatorGameClientRpc(name);

        yield return new WaitForSeconds(6.27f);

        foreach (GameObject icon in GameObject.FindGameObjectsWithTag("Icon"))
            Destroy(icon);

        currentState.Value = State.Lobby;

        SetNewPositions();
		alivePlayersCount.Value = 0;
        invisibleWalls.SetActive(true);

        if (IsServer)
		{
            inLobbyUI.SetActive(true);

            for (int i = 0; i < scoresList.Count; i++)
            {
				scoresList[i] = 0;
            }
        }

    }

    public void LaunchGame()
	{
		if (playersCount.Value > 1)
		{
            inLobbyUI.SetActive(false);

            if (IsServer)
                currentState.Value = State.Round;

            alivePlayersCount.Value = playersCount.Value;
            invisibleWalls.SetActive(false);

            foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
            {
                player.GetComponent<PlayerNetwork>().isAlive = true;
            }
        }

		SetLaunchGameAnimatorClientRpc();
}

	public void LeaveGame()
	{
		Cleanup();

		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	void Cleanup()
	{
		if (NetworkManager.Singleton != null)
		{
			NetworkManager.Singleton.Shutdown();

			Destroy(NetworkManager.Singleton.gameObject);
		}
	}

	void setFirstPlaceIcon(ulong id)
	{
		foreach (GameObject icon in GameObject.FindGameObjectsWithTag("Icon"))
			Destroy(icon);

		GameObject spawnedIcon = Instantiate(firstPlaceIconPrefab);

        spawnedIcon.GetComponent<NetworkObject>().Spawn(true);

        spawnedIcon.GetComponent<FirstPlaceIcon>().findFirstPlaceClientRpc(id);
    }


	// RPCs

	[ClientRpc]
	public void SetNameUIClientRpc(int i, string name)
	{
		playersUI[i].SetActive(true);

		playersUI[i].transform.Find("Nom").GetComponent<TextMeshProUGUI>().text = name;

		for (int j = i+1; j < playersUI.Length; j++)
			playersUI[j].SetActive(false);
	}

	[ClientRpc]
	public void SetColorUIClientRpc(int i, Color color)
	{
		playersUI[i].transform.Find("Logo").GetComponent<RawImage>().color = color;
	}

	[ClientRpc]
	public void SetScoresClientRpc()
	{
		SetScores();
	}

    [ClientRpc]
    public void SetAnimatorRoundClientRpc(string name)
    {
		roundWinnerText.GetComponent<TextMeshProUGUI>().text = name + " gagne la manche !";
        roundAnimator.SetBool("Start", true);
    }

    [ClientRpc]
    public void SetAnimatorGameClientRpc(string name)
    {
        gameWinnerText.GetComponent<TextMeshProUGUI>().text = name + " a gagné la partie !";
        gameAnimator.SetBool("Start", true);

		danceAnimator.SetBool("Dance", true);
    }

    [ClientRpc]
    public void SetLaunchGameAnimatorClientRpc()
    {
        launchGameAnimator.SetBool("Start", true);
    }
}
