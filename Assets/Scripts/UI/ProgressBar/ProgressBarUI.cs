using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarUI : MonoBehaviour
{

    [SerializeField] private GameObject progressCounterGameObject;
    [SerializeField] private Image progressBar;

    private IHasProgress progressCounter;

    private void Start() {
        progressCounter = progressCounterGameObject.GetComponent<IHasProgress>();
        if (progressCounter == null) {
            Debug.LogError("GameObject doesn't have IHasProgress script!");
        }

        progressCounter.OnProgressUpdate += ProgressCounter_OnProgressUpdate;
        progressBar.fillAmount = 0f;
        Hide();
    }

    private void ProgressCounter_OnProgressUpdate(object sender, IHasProgress.OnProgressUpdateEventArgs e) {
        progressBar.fillAmount = e.progress;
        if (progressBar.fillAmount == 0f || progressBar.fillAmount == 1f) {
            Hide();
        } else {
            Show();
        }
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }

}
