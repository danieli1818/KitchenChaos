using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CountdownUI : MonoBehaviour
{

    private const string NUMBER_POPUP_ANIMATOR_TRIGGER = "NumberPopup";

    [SerializeField] private TextMeshProUGUI countdownText;

    private Animator animator;
    private int prevCountdownNumber;

    private void Awake() {
        HideCountdownText();

        animator = GetComponent<Animator>();
    }

    private void Start() {
        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;
    }

    private void GameManager_OnStateChanged(object sender, GameManager.OnStateChangedEventArgs e) {
        if (e.newState == GameManager.State.Countdown) {
            UpdateCountdownText();
            ShowCountdownText();
        } else {
            HideCountdownText();
        }
    }

    private void Update() {
        if (GameManager.Instance.IsCountdownToStartActive()) {
            UpdateCountdownText();
        }
    }

    private void UpdateCountdownText() {
        int countdownNumber = Mathf.CeilToInt(GameManager.Instance.GetStateTimer());
        if (prevCountdownNumber != countdownNumber) {
            countdownText.text = countdownNumber.ToString();
            prevCountdownNumber = countdownNumber;
            animator.SetTrigger(NUMBER_POPUP_ANIMATOR_TRIGGER);
            SoundManager.Instance.PlayCountdownSound();
        }
    }

    private void ShowCountdownText() {
        countdownText.gameObject.SetActive(true);
    }

    private void HideCountdownText() {
        countdownText.gameObject.SetActive(false);
    }

}
