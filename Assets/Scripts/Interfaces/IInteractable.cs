using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable: ISelectable
{

    public void Interact(PlayerController player);
    public void InteractAlternate(PlayerController player);

}
