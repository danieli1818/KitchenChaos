using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatesCounter : BaseCounter
{

    public event EventHandler OnPlateSpawned;
    public event EventHandler OnPlateRemoved;

    private float platesSpawnTimer;
    [SerializeField] float platesSpawnTime;

    private int platesCounter;
    [SerializeField] private int maxPlatesCounter;

    [SerializeField] private KitchenObjectSO plateKitchenObjectSO;

    private void Awake() {
        platesCounter = 0;
    }

    private void Start() {
        platesSpawnTimer = 0f;
    }

    private void Update() {
        if (platesCounter < maxPlatesCounter) {
            UpdatePlatesSpawnTimer();
            if (platesSpawnTimer >= platesSpawnTime) {
                SpawnPlate();
                platesSpawnTimer = 0f;
            }
        }
    }

    public override void Interact(PlayerController player) {
        if (!player.HasHeldKitchenObject() && platesCounter > 0) {
            GivePlateToPlayer(player);
        }
    }

    private void UpdatePlatesSpawnTimer() {
        platesSpawnTimer += Time.deltaTime;
    }

    private void SpawnPlate() {
        platesCounter++;
        OnPlateSpawned?.Invoke(this, EventArgs.Empty);
    }

    private void GivePlateToPlayer(PlayerController player) {
        platesCounter--;
        KitchenObject.SpawnKitchenObject(plateKitchenObjectSO, player);
        OnPlateRemoved?.Invoke(this, EventArgs.Empty);
    }

}
