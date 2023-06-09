using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{

    private const string IS_WALKING = "IsWalking";

    [SerializeField] private PlayerController playerController;

    private Animator playerAnimator;

    private void Awake() {
        playerAnimator = GetComponent<Animator>();
    }

    private void Update() {
        playerAnimator.SetBool(IS_WALKING, playerController.IsWalking());
    }
}
