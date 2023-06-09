using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlateIconsSingleIconUI : MonoBehaviour
{

    [SerializeField] private Image iconSprite;

    public void UpdateToKitchenObjectSO(KitchenObjectSO kitchenObjectSO) {
        iconSprite.sprite = kitchenObjectSO.GetIcon();
    }

}
