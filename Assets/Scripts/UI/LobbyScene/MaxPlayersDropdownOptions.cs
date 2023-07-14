using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MaxPlayersDropdownOptions : MonoBehaviour
{

    private TMP_Dropdown dropdown;
    [SerializeField] private Transform optionsTransform;
    [SerializeField] private Transform optionsTemplate;

    private void Start() {
        dropdown = GetComponent<TMP_Dropdown>();
        foreach (int numOfPlayers in MultiplayerManager.Instance.GetMaxPlayersOptions()) {
            dropdown.options.Add(new TMP_Dropdown.OptionData(numOfPlayers.ToString()));
            if (dropdown.options.Count == 1) {
                dropdown.SetValueWithoutNotify(0);
                dropdown.value = numOfPlayers;
            }
            /* Transform optionTransform = Instantiate(optionsTemplate, optionsTransform);
            TextMeshProUGUI optionText = optionTransform.GetComponent<TextMeshProUGUI>();
            optionText.text = numOfPlayers.ToString();
            optionTransform.gameObject.SetActive(true); */
        }
    }

}
