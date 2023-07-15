using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{

    private const string LOBBY_DATA_RELAY_ALLOCATION_JOIN_CODE = "RelayAllocationJoinCode";
    private const string RELAY_UNITY_TRANSPORT_PROTOCOL = "dtls";

    public static LobbyManager Instance { get; private set; }

    public event EventHandler<OnLobbyListUpdatedEventArgs> OnLobbyListUpdated;
    public class OnLobbyListUpdatedEventArgs : EventArgs {
        public List<Lobby> lobbies;
    }
    public event EventHandler OnTryingCreateLobby;
    public event EventHandler OnFailedCreateLobby;
    public event EventHandler OnTryingJoinLobby;
    public event EventHandler OnTryingQuickJoinLobby;
    public event EventHandler OnFailedJoinLobby;
    public event EventHandler OnFailedQuickJoinLobby;

    private Lobby joinedLobby;

    // Timers
    private float heartbeatTimer;
    [SerializeField] private float heartbeatInterval;
    private float updateLobbiesTimer;
    [SerializeField] private float updateLobbiesInterval;

    private void Awake() {
        if (Instance != null) {
            Debug.LogError("Trying to create more than one LobbyManagers");
            return;
        }
        Instance = this;
        heartbeatTimer = heartbeatInterval;
        updateLobbiesTimer = updateLobbiesInterval;

        DontDestroyOnLoad(this);
        InitializeServiceAndAuthenticate();
        SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
    }

    private void Update() {
        HandleHeartbeats();
        HandleLobbiesUpdate();
    }

    private async void HandleHeartbeats() {
        if (!IsLobbyHost()) {
            return;
        }
        try {
            heartbeatTimer -= Time.deltaTime;
            if (heartbeatTimer <= 0) {
                heartbeatTimer = heartbeatInterval;
                await LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
            }
        } catch (LobbyServiceException exception) {
            Debug.LogError(exception);
        }
    }

    private void HandleLobbiesUpdate() {
        if (!IsAutherized() || IsInLobby()) {
            return;
        }
        updateLobbiesTimer -= Time.deltaTime;
        if (updateLobbiesTimer <= 0) {
            updateLobbiesTimer = updateLobbiesInterval;
            QueryAvailableLobbies();
        }
    }

    private void SceneManager_activeSceneChanged(Scene prevScene, Scene newScene) {
        Debug.Log("Running Inside SceneManager_activeSceneChanged With The Parameters: " + prevScene.name + ", " + newScene.name);
        if (!SceneLoader.IsCurrentScene(SceneLoader.Scene.LobbyScene) && 
            !SceneLoader.IsCurrentScene(SceneLoader.Scene.CharacterSelectionScene) && 
            !SceneLoader.IsCurrentScene(SceneLoader.Scene.LoadingScene)) {
            Debug.Log("Destroying LobbyManager!");
            DestroySelf();
        }
    }

    private async void InitializeServiceAndAuthenticate() {
        try {
            Debug.Log("Uninitialized state: " + (UnityServices.State == ServicesInitializationState.Uninitialized).ToString());
            if (UnityServices.State == ServicesInitializationState.Uninitialized) {
                InitializationOptions initializationOptions = new InitializationOptions();
                // initializationOptions.SetProfile(UnityEngine.Random.Range(0, 1000).ToString()); // Enable for testing locally with multiple instances
                await UnityServices.InitializeAsync(initializationOptions);
            }
            Debug.Log("IsAuthorized: " + AuthenticationService.Instance.IsAuthorized.ToString());
            if (!AuthenticationService.Instance.IsAuthorized) {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
        } catch (Exception exception) {
            Debug.LogError(exception);
        }
    }

    private async Task<Allocation> AllocateRelay(int maxPlayers) {
        try {
            return await RelayService.Instance.CreateAllocationAsync(maxPlayers - 1);
        } catch (RelayServiceException exception) {
            Debug.LogError(exception);
            return default;
        }
    }

    private async Task<string> GetRelayCode(Allocation relayAllocation) {
        try {
            return await RelayService.Instance.GetJoinCodeAsync(relayAllocation.AllocationId);
        } catch (RelayServiceException exception) {
            Debug.LogError(exception);
            return default;
        }
    }

    private async Task<JoinAllocation> JoinRelay(string relayCode) {
        try {
            return await RelayService.Instance.JoinAllocationAsync(relayCode);
        } catch (RelayServiceException exception) {
            Debug.LogError(exception);
            return default;
        }
    }

    public async void CreateLobby(string lobbyName, int maxPlayers, bool isPrivate = false) {
        try {
            CreateLobbyOptions createLobbyOptions = new() {
                IsPrivate = isPrivate
            };
            OnTryingCreateLobby?.Invoke(this, EventArgs.Empty);
            joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, createLobbyOptions);

            Allocation relayAllocation = await AllocateRelay(joinedLobby.MaxPlayers);
            string relayCode = await GetRelayCode(relayAllocation);
            try {
                await LobbyService.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions() {
                    Data = new Dictionary<string, DataObject>() {
                    { LOBBY_DATA_RELAY_ALLOCATION_JOIN_CODE, new DataObject(DataObject.VisibilityOptions.Member, relayCode) }
                }
                });
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(relayAllocation, RELAY_UNITY_TRANSPORT_PROTOCOL));
            } catch (RelayServiceException exception) {
                Debug.LogError(exception);
            }

            MultiplayerManager.Instance.StartHost();
        } catch (LobbyServiceException exception) {
            Debug.LogError(exception);
            OnFailedCreateLobby?.Invoke(this, EventArgs.Empty);
        }
    }

    public async void JoinLobbyByCode(string lobbyCode) {
        try {
            OnTryingJoinLobby?.Invoke(this, EventArgs.Empty);
            joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);
            JoinAllocation relayAllocation = await JoinRelay(joinedLobby.Data[LOBBY_DATA_RELAY_ALLOCATION_JOIN_CODE].Value);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(relayAllocation, RELAY_UNITY_TRANSPORT_PROTOCOL));
            MultiplayerManager.Instance.StartClient();
        } catch (LobbyServiceException exception) {
            Debug.LogError(exception);
            OnFailedJoinLobby?.Invoke(this, EventArgs.Empty);
        }
    }

    public async void JoinLobbyById(string lobbyId) {
        try {
            OnTryingJoinLobby?.Invoke(this, EventArgs.Empty);
            joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);
            JoinAllocation relayAllocation = await JoinRelay(joinedLobby.Data[LOBBY_DATA_RELAY_ALLOCATION_JOIN_CODE].Value);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(relayAllocation, RELAY_UNITY_TRANSPORT_PROTOCOL));
            MultiplayerManager.Instance.StartClient();
        } catch (LobbyServiceException exception) {
            Debug.LogError(exception);
            OnFailedJoinLobby?.Invoke(this, EventArgs.Empty);
        }
    }

    public async void QuickJoinLobby() {
        try {
            OnTryingQuickJoinLobby?.Invoke(this, EventArgs.Empty);
            joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();
            JoinAllocation relayAllocation = await JoinRelay(joinedLobby.Data[LOBBY_DATA_RELAY_ALLOCATION_JOIN_CODE].Value);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(relayAllocation, RELAY_UNITY_TRANSPORT_PROTOCOL));
            MultiplayerManager.Instance.StartClient();
        } catch (LobbyServiceException exception) {
            Debug.LogError(exception);
            OnFailedQuickJoinLobby?.Invoke(this, EventArgs.Empty);
        }
    }

    public async void QueryAvailableLobbies() {
        try {
            QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync(new QueryLobbiesOptions() {
                Filters = new List<QueryFilter>() { new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT) }
            });
            OnLobbyListUpdated?.Invoke(this, new OnLobbyListUpdatedEventArgs() {
                lobbies = queryResponse.Results
            });
        } catch (LobbyServiceException exception) {
            Debug.LogError(exception);
        }
    }

    public async void LeaveLobby() {
        try {
            await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
            joinedLobby = null;
        } catch (LobbyServiceException exception) {
            Debug.LogError(exception);
        }
    }

    public async void KickPlayerFromLobby(string playerId) {
        if (IsLobbyHost()) {
            try {
                await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, playerId);
            } catch (LobbyServiceException exception) {
                Debug.LogError(exception);
            }
        }
    }

    public async void DestroyLobby(string lobbyId) {
        try {
            await LobbyService.Instance.DeleteLobbyAsync(lobbyId);

            joinedLobby = null;
        } catch (LobbyServiceException exception) {
            Debug.LogError(exception);
        }
    }

    public Lobby GetLobby() {
        return joinedLobby;
    }

    private bool IsLobbyHost() {
        return IsInLobby() && AuthenticationService.Instance.PlayerId == joinedLobby.HostId;
    }

    private bool IsInLobby() {
        return joinedLobby != null;
    }

    private bool IsAutherized() {
        return AuthenticationService.Instance.IsAuthorized;
    }

    public void DestroySelf() {
        SceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;
        if (joinedLobby != null) {
            LeaveLobby();
        }
        Destroy(gameObject);
    }

    public void ClearLobby() {
        joinedLobby = null;
    }


}
