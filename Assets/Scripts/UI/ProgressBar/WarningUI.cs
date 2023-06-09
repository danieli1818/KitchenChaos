using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WarningUI : MonoBehaviour
{

    private const string IS_WARNING_ON_ANIMATOR_PARAMETER = "IsWarningOn";

    [SerializeField] private GameObject warningAlerterGameObject;
    [SerializeField] private Image warningLogo;

    private IHasWarningAlert warningAlerter;
    private Animator animator;

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    private void Start() {
        warningAlerter = warningAlerterGameObject.GetComponent<IHasWarningAlert>();
        if (warningAlerter == null) {
            Debug.LogError("GameObject doesn't have IHasWarningAlert script!");
        }

        warningAlerter.OnWarningStateChanged += WarningAlerter_OnWarningStateChanged;
        HideWarningLogo();
    }

    private void WarningAlerter_OnWarningStateChanged(object sender, IHasWarningAlert.OnWarningStateChangedEventArgs e) {
        if (e.isWarningOn) {
            ShowWarningLogo();
        } else {
            HideWarningLogo();
        }
    }

    private void ShowWarningLogo() {
        warningLogo.gameObject.SetActive(true);
        animator.SetBool(IS_WARNING_ON_ANIMATOR_PARAMETER, true);
    }

    private void HideWarningLogo() {
        warningLogo.gameObject.SetActive(false);
        animator.SetBool(IS_WARNING_ON_ANIMATOR_PARAMETER, false);
    }

}
