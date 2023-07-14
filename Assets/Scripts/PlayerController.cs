using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour, IKitchenObjectHolder
{

    public static PlayerController LocalInstance { get; private set; }

    public static event EventHandler OnAnyPickupKitchenObject;
    public static event EventHandler OnAnyPlayerSpawned;

    [SerializeField] private float velocity = 7f;
    [SerializeField] private LayerMask interactionsLayerMask;
    [SerializeField] private LayerMask collisionsLayerMask;
    [SerializeField] private Transform holdingPoint;
    [SerializeField] private List<Vector3> spawningPoints;
    [SerializeField] private PlayerVisual playerVisual;

    public event EventHandler<OnSelectionChangeEventArgs> OnSelectionChange;

    private KitchenObject heldKitchenObject;
    public class OnSelectionChangeEventArgs {
        public ISelectable prevSelectable;
        public ISelectable newSelectable;
    }

    private IInteractable selectedObject;

    private bool isWalking;

    private Vector3 lastMoveDir;

    public static void ResetStaticData() {
        OnAnyPickupKitchenObject = null;
        OnAnyPlayerSpawned = null;
    }

    private bool CanMove(Vector3 moveDir, float distance) {
        float playerRadius = 0.7f;
        // float playerHeight = 2f;
        return !Physics.BoxCast(transform.position, Vector3.one * playerRadius, moveDir, Quaternion.identity, distance, collisionsLayerMask);
    }

    private void Awake() {
        // Instance = this;
        OnSelectionChange += Select;
    }

    private void Start() {
        InputsHandler.Instance.OnInteract += InputsHandler_OnInteract;
        InputsHandler.Instance.OnInteractAlternate += InputsHandler_OnInteractAlternate;
        playerVisual.SetPlayerColor(MultiplayerManager.Instance.GetColorByIndex(MultiplayerManager.Instance.GetPlayerDataFromClientId(OwnerClientId).colorIndex));
    }

    public override void OnNetworkSpawn() {
        if (IsOwner) {
            LocalInstance = this;
        }

        transform.position = spawningPoints[MultiplayerManager.Instance.GetPlayerIndexFromClientId(OwnerClientId)];
        OnAnyPlayerSpawned?.Invoke(this, EventArgs.Empty);

        if (IsServer) {
            NetworkManager.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
        }
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientId) {
        if (clientId == OwnerClientId) {
            if (HasHeldKitchenObject()) {
                GetKitchenObject().DestroySelf();
            }
        }
    }

    private void InputsHandler_OnInteractAlternate(object sender, EventArgs e) {
        if (!GameManager.Instance.IsGamePlaying()) {
            return;
        }
        selectedObject?.InteractAlternate(this);
    }

    private void InputsHandler_OnInteract(object sender, EventArgs e) {
        if (!GameManager.Instance.IsGamePlaying()) {
            return;
        }
        selectedObject?.Interact(this);
    }

    private void Update() {
        if (!IsOwner) {
            return;
        }

        HandleMovement();
        HandleInteractions();
    }

    public bool IsWalking() {
        return isWalking;
    }

    private void HandleMovement() {

        Vector2 inputVectorNormalized = InputsHandler.Instance.GetInputVectorNormalized();

        Vector3 moveDir = new(inputVectorNormalized.x, 0f, inputVectorNormalized.y);

        float distance = Time.deltaTime * velocity;

        bool canMove = CanMove(moveDir, distance);

        if (!canMove) {
            canMove = Math.Abs(moveDir.x) > 0.5f && CanMove(new Vector3(moveDir.x, 0, 0).normalized, distance);
            if (canMove) {
                moveDir = new Vector3(moveDir.x, 0, 0).normalized;
            } else {
                canMove = Math.Abs(moveDir.z) > 0.5f && CanMove(new Vector3(0, 0, moveDir.z).normalized, distance);
                if (canMove) {
                    moveDir = new Vector3(0, 0, moveDir.z).normalized;
                }
            }
        }

        if (canMove) {
            // Walk
            transform.position += distance * moveDir;

        }

        isWalking = moveDir != Vector3.zero;
        float rotationSpeed = 10f;
        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotationSpeed);
    }

    private void HandleInteractions() {
        Vector2 inputVectorNormalized = InputsHandler.Instance.GetInputVectorNormalized();

        Vector3 moveDir = new(inputVectorNormalized.x, 0f, inputVectorNormalized.y);

        if (moveDir != Vector3.zero) {
            lastMoveDir = moveDir;
        }

        float interactionDistance = 2f;
        if (Physics.Raycast(transform.position, lastMoveDir, out RaycastHit hitInfo, interactionDistance, interactionsLayerMask)) {
            if (hitInfo.transform.TryGetComponent(out IInteractable interactable)) {
                if (selectedObject != interactable) {
                    SetSelectedInteractable(interactable);
                }
            } else {
                SetSelectedInteractable(null);
            }
        } else {
            SetSelectedInteractable(null);
        }

    }

    private void SetSelectedInteractable(IInteractable interactable) {
        ISelectable prevSelectable = selectedObject;
        selectedObject = interactable;
        OnSelectionChange?.Invoke(this, new OnSelectionChangeEventArgs {
            prevSelectable = prevSelectable,
            newSelectable = selectedObject
        });
    }

    private void Select(object sender, OnSelectionChangeEventArgs args) {
        args.prevSelectable?.UnSelect();
        args.newSelectable?.Select();
    }

    public bool SetHeldKitchenObject(KitchenObject kitchenObject) {
        if (kitchenObject != null && HasHeldKitchenObject()) {
            Debug.LogError("Trying to set kitchen object to player with held item");
            return false;
        }
        heldKitchenObject = kitchenObject;
        if (heldKitchenObject != null) {
            OnAnyPickupKitchenObject?.Invoke(this, EventArgs.Empty);
        }
        return true;
    }

    public bool HasHeldKitchenObject() {
        return heldKitchenObject != null;
    }

    public bool ClearHeldKitchenObject() {
        return SetHeldKitchenObject(null);
    }

    public Transform GetHoldingPoint() {
        return holdingPoint;
    }

    public KitchenObject GetKitchenObject() {
        return heldKitchenObject;
    }

    public NetworkObject GetNetworkObject() {
        return NetworkObject;
    }

}
