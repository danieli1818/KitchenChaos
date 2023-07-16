using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using UnityEngine;

public class MultiplayerManager : NetworkBehaviour
{

    private const string PLAYER_NAME_PLAYER_PREFS_ID = "PlayerName";

    public static MultiplayerManager Instance { get; private set; }

    public static bool IsMultiplayer { get; set; } = true;

    public event EventHandler OnStartingHost;
    public event EventHandler OnTryingToConnect;
    public event EventHandler OnClientDisconnected;
    public event EventHandler OnPlayersDataListChanged;
    public event EventHandler OnPlayerTryingToSelectTheSameColor;
    public event EventHandler OnPlayerTryingToSelectColorSelectedByAnotherPlayer;

    [SerializeField] private List<Color> playerColors;

    private NetworkList<PlayerData> playersDataList;

    private string playerName;

    private int maxPlayers;

    private void Awake() {
        if (Instance != null) {
            Debug.LogError("Trying to create more than one instance of MultiplayerManager!");
            return;
        }
        Instance = this;
        playerName = PlayerPrefs.GetString(PLAYER_NAME_PLAYER_PREFS_ID, "Player " + UnityEngine.Random.Range(1, 10000).ToString());
        playersDataList = new NetworkList<PlayerData>();
        playersDataList.OnListChanged += PlayersDataList_OnListChanged;
        DontDestroyOnLoad(this);
    }

    private void Start() {
        if (!IsMultiplayer) {
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData("127.0.0.1", 7777);
            StartHost(1);
        }
    }

    private void PlayersDataList_OnListChanged(NetworkListEvent<PlayerData> changeEvent) {
        OnPlayersDataListChanged?.Invoke(this, EventArgs.Empty);
    }

    public void StartHost(int maxPlayers = 4) {
        if (maxPlayers <= 0) {
            Debug.LogError("Invalid max players amount, smaller or equal to 0");
            return;
        }
        this.maxPlayers = maxPlayers;
        OnStartingHost?.Invoke(this, EventArgs.Empty);
        NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallbackMaxPlayersAndGameHasStarted;
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_Server_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Server_OnClientDisconnectCallback;
        NetworkManager.Singleton.StartHost();
        SceneLoader.LoadNetworkScene(SceneLoader.Scene.CharacterSelectionScene);
    }

    private void NetworkManager_Server_OnClientDisconnectCallback(ulong clientId) {
        playersDataList.RemoveAt(GetPlayerIndexFromClientId(clientId));
    }

    private void NetworkManager_Server_OnClientConnectedCallback(ulong clientId) {
        playersDataList.Add(new PlayerData() {
            clientId = clientId,
            colorIndex = GetFirstAvailableColorIndex(),
            playerId = AuthenticationService.Instance.PlayerId,
            playerName = playerName
        });
    }

    private bool IsColorAvailable(int colorIndex) {
        foreach (PlayerData playerData in playersDataList) {
            if (playerData.colorIndex == colorIndex) {
                return false;
            }
        }
        return true;
    }

    private int GetFirstAvailableColorIndex() {
        for (int colorIndex = 0; colorIndex < playerColors.Count; colorIndex++) {
            if (IsColorAvailable(colorIndex)) {
                return colorIndex;
            }
        }
        Debug.LogError("There isn't any available colors!");
        return -1;
    }

    private void NetworkManager_ConnectionApprovalCallbackMaxPlayersAndGameHasStarted(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response) {
        if (response.Reason != null && response.Reason != "") {
            return;
        }
        if (NetworkManager.Singleton.ConnectedClientsIds.Count >= maxPlayers) {
            response.Approved = false;
            response.Reason = "The game is full";
        } else if (!SceneLoader.IsCurrentScene(SceneLoader.Scene.CharacterSelectionScene)) {
            response.Approved = false;
            response.Reason = "The game has already started";
        } else {
            response.Approved = true;
        }
    }

    public void StartClient() {
        OnTryingToConnect?.Invoke(this, EventArgs.Empty);

        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Client_OnClientDisconnectCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_Client_OnClientConnectedCallback;
        NetworkManager.Singleton.StartClient();
    }

    private void NetworkManager_Client_OnClientConnectedCallback(ulong clientId) {
        SetPlayerNameServerRpc(playerName);
        SetPlayerIdServerRpc(AuthenticationService.Instance.PlayerId);
    }

    public void SetLocalPlayerName(string playerName) {
        this.playerName = playerName;

        PlayerPrefs.SetString(PLAYER_NAME_PLAYER_PREFS_ID, playerName);
        PlayerPrefs.Save();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerNameServerRpc(string playerName, ServerRpcParams serverRpcParams = default) {
        int playerIndex = GetPlayerIndexFromClientId(serverRpcParams.Receive.SenderClientId);
        PlayerData playerData = GetPlayerData(playerIndex);
        playerData.playerName = playerName;
        playersDataList[playerIndex] = playerData;
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerIdServerRpc(string playerId, ServerRpcParams serverRpcParams = default) {
        int playerIndex = GetPlayerIndexFromClientId(serverRpcParams.Receive.SenderClientId);
        PlayerData playerData = GetPlayerData(playerIndex);
        playerData.playerId = playerId;
        playersDataList[playerIndex] = playerData;
    }

    private void NetworkManager_Client_OnClientDisconnectCallback(ulong clientId) {
        OnClientDisconnected?.Invoke(this, EventArgs.Empty);
    }

    public void ShutdownAndDestroyNetworkManager() {
        Debug.Log("Destroying Network Manager and Multiplayer Manager!");
        if (LobbyManager.Instance != null) {
            LobbyManager.Instance.DestroySelf();
        }
        Destroy(NetworkManager.Singleton.gameObject);
        NetworkManager.Singleton.Shutdown();
        Destroy(gameObject);
    }

    public bool IsPlayerConnected(int playerIndex) {
        return playerIndex < playersDataList.Count;
    }

    public PlayerData GetPlayerData(int playerIndex) {
        if (IsPlayerConnected(playerIndex)) {
            return playersDataList[playerIndex];
        }
        Debug.LogError("Tried to get player data of a player with an index outside of bounds!");
        return default;
    }

    public Color GetColorByIndex(int colorIndex) {
        // Always will be called with colorIndex inside the list
        return playerColors[colorIndex];
    }

    public int GetPlayerIndexFromClientId(ulong clientId) {
        for (int i = 0; i < playersDataList.Count; i++) {
            if (playersDataList[i].clientId == clientId) {
                return i;
            }
        }
        return -1;
    }

    public ulong GetClientIdFromPlayerIndex(int playerIndex) {
        if (IsPlayerConnected(playerIndex)) {
            return playersDataList[playerIndex].clientId;
        }
        Debug.LogError("Tried to get player data of a player with an index outside of bounds!");
        return default;
    }

    public PlayerData GetPlayerDataFromClientId(ulong clientId) {
        return GetPlayerData(GetPlayerIndexFromClientId(clientId));
    }

    public int GetNumberOfColors() {
        return playerColors.Count;
    }

    public PlayerData GetLocalPlayerData() {
        return GetPlayerDataFromClientId(NetworkManager.Singleton.LocalClientId);
    }

    public void SetLocalPlayerColor(int colorIndex) {
        if (GetLocalPlayerData().colorIndex == colorIndex) {
            OnPlayerTryingToSelectTheSameColor?.Invoke(this, EventArgs.Empty);
            return;
        }
        SetPlayerColorServerRpc(colorIndex);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerColorServerRpc(int colorIndex, ServerRpcParams serverRpcParams = default) {
        if (!IsColorAvailable(colorIndex)) {
            ClientRpcParams clientRpcParams = new ClientRpcParams {
                Send = new ClientRpcSendParams {
                    TargetClientIds = new ulong[] { serverRpcParams.Receive.SenderClientId }
                }
            };
            OnPlayerTryingToSelectColorSelectedByAnotherPlayerClientRpc(clientRpcParams);
        } else {
            int playerIndex = GetPlayerIndexFromClientId(serverRpcParams.Receive.SenderClientId);
            PlayerData playerData = GetPlayerData(playerIndex);
            playerData.colorIndex = colorIndex;
            playersDataList[playerIndex] = playerData;
        }
    }

    [ClientRpc]
    private void OnPlayerTryingToSelectColorSelectedByAnotherPlayerClientRpc(ClientRpcParams clientRpcParams = default) {
        OnPlayerTryingToSelectColorSelectedByAnotherPlayer?.Invoke(this, EventArgs.Empty);
    }

    // Server Side Function Only
    public void KickPlayer(ulong clientId) {
        LobbyManager.Instance.KickPlayerFromLobby(GetPlayerDataFromClientId(clientId).playerId.ToString());
        NetworkManager.Singleton.DisconnectClient(clientId, "You have been kicked");
        NetworkManager_Server_OnClientDisconnectCallback(clientId);
    }

    public List<int> GetMaxPlayersOptions() {
        return new List<int>() { 2, 3, 4 };
    }

    public string GetLocalPlayerName() {
        return playerName;
    }

}
