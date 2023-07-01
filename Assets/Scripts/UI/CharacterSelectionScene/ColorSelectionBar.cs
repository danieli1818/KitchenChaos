using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorSelectionBar : MonoBehaviour
{

    public event EventHandler OnColorSelected;
    public class OnColorSelectedEventArgs : EventArgs {
        public Color color;
    }

    [SerializeField] private Transform colorSelectionButtonTemplateTransform;

    private void Start() {
        for (int i = 0; i < MultiplayerManager.Instance.GetNumberOfColors(); i++) {
            Transform colorSelectionButtonTransform = Instantiate(colorSelectionButtonTemplateTransform, transform);
            ColorSelectionButton colorSelectionButton = colorSelectionButtonTransform.GetComponent<ColorSelectionButton>();
            colorSelectionButton.SetColorIndex(i);
            colorSelectionButton.gameObject.SetActive(true);
        }
    }

}
