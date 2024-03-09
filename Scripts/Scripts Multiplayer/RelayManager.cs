using System.Collections;
using System.Collections.Generic;
using Unity.Services.Core;
using UnityEngine;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using TMPro;

public class RelayManager : MonoBehaviour
{
    public TextMeshProUGUI joinCodeText;

    // Start is called before the first frame update
    private async void Start()
    {
        await UnityServices.InitializeAsync();

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async void CreateRelay()
    {
        try
        {
            Allocation alloc = await RelayService.Instance.CreateAllocationAsync(9);

            string joinCode = await RelayService.Instance.GetJoinCodeAsync(alloc.AllocationId);

            SetJoinCode(joinCode);

            // Debug.Log(joinCode);

            RelayServerData relayServerData = new RelayServerData(alloc, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartHost();

        } catch (RelayServiceException error)
        {
            Debug.Log(error);
        }
    }

    public async void JoinRelay(string joinCode)
    {
        try
        {
            Debug.Log(joinCode);

            JoinAllocation joinAlloc = await RelayService.Instance.JoinAllocationAsync(joinCode);

            RelayServerData relayServerData = new RelayServerData(joinAlloc, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartClient();

        }
        catch (RelayServiceException error)
            {
                Debug.Log(error);
            }
    }

    private void SetJoinCode(string joinCode)
    {
        joinCodeText.text = "Code : " + joinCode;
    }
}
