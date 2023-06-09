using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryResultUI : MonoBehaviour
{

    private const string POPUP_ANIMATOR_TRIGGER = "Popup";

    [SerializeField] private DeliveryCounter deliveryCounter;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Image deliveryIcon;

    [SerializeField] private Color correctDeliveryBackgroundColor;
    [SerializeField] private Color incorrectDeliveryBackgroundColor;
    [SerializeField] private Sprite correctDeliverySprite;
    [SerializeField] private Sprite incorrectDeliverySprite;

    private Animator animator;

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    private void Start() {
        deliveryCounter.OnSuccessfulDelivery += DeliveryCounter_OnSuccessfulDelivery;
        deliveryCounter.OnIncorrectDelivery += DeliveryCounter_OnIncorrectDelivery;

        gameObject.SetActive(false);
    }

    private void DeliveryCounter_OnIncorrectDelivery(object sender, System.EventArgs e) {
        gameObject.SetActive(true);
        backgroundImage.color = incorrectDeliveryBackgroundColor;
        messageText.text = "WRONG\nDELIVERY";
        deliveryIcon.sprite = incorrectDeliverySprite;
        animator.SetTrigger(POPUP_ANIMATOR_TRIGGER);
    }

    private void DeliveryCounter_OnSuccessfulDelivery(object sender, System.EventArgs e) {
        gameObject.SetActive(true);
        backgroundImage.color = correctDeliveryBackgroundColor;
        messageText.text = "DELIVERY\nSUCCESS";
        deliveryIcon.sprite = correctDeliverySprite;
        animator.SetTrigger(POPUP_ANIMATOR_TRIGGER);
    }

}
