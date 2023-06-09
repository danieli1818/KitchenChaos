using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetStaticDataManager : MonoBehaviour
{

    private void Awake() {
        CutterCounter.ResetStaticData();
        DeliveryCounter.ResetStaticData();
        PlayerSoundManager.ResetStaticData();
        BaseCounter.ResetStaticData();
        PlayerController.ResetStaticData();
        StoveCounterSoundManager.ResetStaticData();
        TrashCounter.ResetStaticData();
    }

}
