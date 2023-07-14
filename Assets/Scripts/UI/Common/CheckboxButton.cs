using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckboxButton : MonoBehaviour
{

    public bool IsChecked { get; private set; }
    [SerializeField] private Button button;
    [SerializeField] private GameObject checkedGameObject;

    private void Awake() {
        IsChecked = false;
        checkedGameObject.SetActive(false);
    }

    private void Start() {
        button.onClick.AddListener(() => {
            ToggleCheck();
        });
    }

    private void ToggleCheck() {
        IsChecked = !IsChecked;
        checkedGameObject.SetActive(IsChecked);
    }

}
