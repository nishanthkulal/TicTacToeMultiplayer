using System;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class RelayEample : MonoBehaviour
{
    [SerializeField] private TMP_Text joinCodeText;
    [SerializeField] private TMP_InputField _joinInputField;
    [SerializeField] private GameObject _buttons;
    [SerializeField] private GameObject realpanel;
    private UnityTransport _transport;


    public int Maxplayers = 2;

    private async void Awake()
    {
        _transport = FindObjectOfType<UnityTransport>();
        _buttons.SetActive(false);
        await Authenticate();
        _buttons.SetActive(true);
    }

    private static async Task Authenticate()
    {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();

    }
    public async void CreateGame()
    {
        _buttons.SetActive(false);

        try
        {
            Allocation a = await RelayService.Instance.CreateAllocationAsync(Maxplayers);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(a.AllocationId);

            // Display the join code in the UI
            joinCodeText.text = "Join Code: " + joinCode;
            _joinInputField.text = joinCode;

            // Use SetHostRelayData for the host
            _transport.SetHostRelayData(
                a.RelayServer.IpV4,
                (ushort)a.RelayServer.Port,
                a.AllocationIdBytes,
                a.Key,
                a.ConnectionData
            );

            NetworkManager.Singleton.StartHost();
            Hide();
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to create relay: {e.Message}");
            _buttons.SetActive(true); // Re-enable UI buttons if failed
        }
    }


    public async void JoinGame()
    {
        _buttons.SetActive(false);

        try
        {
            // Join the relay session using the join code
            JoinAllocation a = await RelayService.Instance.JoinAllocationAsync(_joinInputField.text);

            // Set up the client relay connection
            _transport.SetClientRelayData(
                a.RelayServer.IpV4,
                (ushort)a.RelayServer.Port,
                a.AllocationIdBytes,
                a.Key,
                a.ConnectionData,
                a.HostConnectionData // This is required for clients
            );

            // Start the client connection
            NetworkManager.Singleton.StartClient();
            Hide();
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to join relay: {e.Message}");
            _buttons.SetActive(true); // Re-enable UI buttons if joining fails
        }
    }

    private void Hide()
    {
        realpanel.SetActive(false);
    }

}
