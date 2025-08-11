using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayerConnectionBufferHandler : NetworkBehaviour
{
    const string Header = "Connected Players";
    const string TitleHeader = "Waiting for Players to Connect";
    const string ConfirmationHeader = "All Players Connected Starting Shortly";

    [SerializeField] private TextMeshProUGUI screenTitle;
    [SerializeField] private TextMeshProUGUI connectedPlayersDisplay;
    [SerializeField] private GameObject continueBtn;
    private NetworkVariable<int> currentConnectedPlayerCount = new NetworkVariable<int>();
    private NetworkVariable<int> expectedConnectedPlayerCount = new NetworkVariable<int>();

    private void Start()
    {
        if (IsServer)
        {
            expectedConnectedPlayerCount.Value = MultiplayerManager.Instance.expectedPlayerCount;
        }
        continueBtn.SetActive(false);
    }

    public override void OnNetworkSpawn()
    {
        UpdateConnectedPlayersDisplay();
    }

    private void Update()
    {
        if (IsServer)
        {
            UpdateValues();
            MinimumPlayersConnectedCheck();
        }
        AllPlayersConnectedCheck();
        UpdateConnectedPlayersDisplay();
    }

    private void UpdateConnectedPlayersDisplay()
    {
        connectedPlayersDisplay.text = $"{Header} {currentConnectedPlayerCount.Value}/{expectedConnectedPlayerCount.Value}";
    }

    private void AllPlayersConnectedCheck()
    {
        if (currentConnectedPlayerCount.Value == expectedConnectedPlayerCount.Value)
        {
            screenTitle.text = ConfirmationHeader;
            if (IsServer)
            {
                MultiplayerSceneManager.Instance.ToPlayerKartScene();
                MultiplayerManager.Instance.Reset();
            }
        }
    }

    private void MinimumPlayersConnectedCheck()
    {
        if (currentConnectedPlayerCount.Value > 1)
        {
            continueBtn.SetActive(true);
        } else if (currentConnectedPlayerCount.Value < 2){
            continueBtn.SetActive(false);
        }
    }

    private void UpdateValues()
    {
        currentConnectedPlayerCount.Value = NetworkManager.ConnectedClientsList.Count;
        expectedConnectedPlayerCount.Value = MultiplayerManager.Instance.expectedPlayerCount;
    }

    public void ContinueSkip()
    {
        if (currentConnectedPlayerCount.Value > 1)
        {
            MultiplayerSceneManager.Instance.ToPlayerKartScene();
            MultiplayerManager.Instance.Reset();
        }
    }
}
