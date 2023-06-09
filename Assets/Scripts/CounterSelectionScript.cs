using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CounterSelectionScript : MonoBehaviour
{

    [SerializeField] private BaseCounter selectable;
    [SerializeField] private GameObject[] visualGameObjectArray;

    private void Start() {
        selectable.OnSelect += OnSelect;
        selectable.OnUnSelect += OnUnSelect;
    }

    private void OnSelect(object sender, System.EventArgs e) {
        foreach (GameObject visualGameObject in visualGameObjectArray) {
            visualGameObject.SetActive(true);
        }
    }

    private void OnUnSelect(object sender, System.EventArgs e) {
        foreach (GameObject visualGameObject in visualGameObjectArray) {
            visualGameObject.SetActive(false);
        }
    }

}
