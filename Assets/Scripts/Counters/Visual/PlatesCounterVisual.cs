using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatesCounterVisual : MonoBehaviour
{

    [SerializeField] PlatesCounter platesCounter;
    [SerializeField] Transform topPoint;
    [SerializeField] Transform plateVisualPrefab;

    private float plateSpawnOffsetY = 0.1f;
    private List<GameObject> platesGameObjects;

    private void Awake() {
        platesGameObjects = new List<GameObject>();
    }

    private void Start() {
        platesCounter.OnPlateSpawned += PlatesCounter_OnPlateSpawned;
        platesCounter.OnPlateRemoved += PlatesCounter_OnPlateRemoved;
    }

    private void PlatesCounter_OnPlateRemoved(object sender, System.EventArgs e) {
        GameObject lastPlateGameObject = platesGameObjects[platesGameObjects.Count - 1];
        platesGameObjects.Remove(lastPlateGameObject);
        Destroy(lastPlateGameObject);
    }

    private void PlatesCounter_OnPlateSpawned(object sender, System.EventArgs e) {
        GameObject plateGameObject = Instantiate(plateVisualPrefab.gameObject, topPoint);
        plateGameObject.transform.localPosition = new Vector3(0f, platesGameObjects.Count * plateSpawnOffsetY, 0f);
        platesGameObjects.Add(plateGameObject);
    }
}
