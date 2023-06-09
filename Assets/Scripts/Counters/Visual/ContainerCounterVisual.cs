using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerCounterVisual : MonoBehaviour
{

    private const string OPEN_CLOSE = "OpenClose";

    [SerializeField] private ContainerCounter counter;

    private Animator animator;

    private void Awake() {
        animator = transform.GetComponent<Animator>();
    }

    private void Start() {
        counter.OnPlayerGrabbedObject += Counter_OnPlayerGrabbedObject;
    }

    private void Counter_OnPlayerGrabbedObject(object sender, System.EventArgs e) {
        animator.SetTrigger(OPEN_CLOSE);
    }
}
