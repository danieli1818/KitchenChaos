using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveCounterVisual : MonoBehaviour
{

    [SerializeField] private StoveCounter stoveCounter;
    [SerializeField] private GameObject[] activeStoveGameObjects;

    private void Start() {
        stoveCounter.OnStateChanged += StoveCounter_OnStateChanged;
    }

    private void StoveCounter_OnStateChanged(object sender, StoveCounter.OnStateChangedEventArgs e) {
        if (e.nextState == StoveCounter.State.Frying || e.nextState == StoveCounter.State.Burning) {
            ShowActiveStoveGameObjects();
        } else {
            HideActiveStoveGameObjects();
        }
    }

    private void ShowActiveStoveGameObjects() {
        foreach (GameObject activeStoveGameObject in activeStoveGameObjects) {
            activeStoveGameObject.SetActive(true);
        }
    }

    private void HideActiveStoveGameObjects() {
        foreach (GameObject activeStoveGameObject in activeStoveGameObjects) {
            activeStoveGameObject.SetActive(false);
        }
    }

}
