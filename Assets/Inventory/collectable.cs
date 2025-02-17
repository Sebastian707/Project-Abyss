using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collectable : Interactable
{
    public TetrisItem itemTetris;

    protected override void Interact()
    {
        bool wasPickedUpTetris = TetrisSlot.instanceSlot.addInFirstSpace(itemTetris);
        if (wasPickedUpTetris)
        {
            Destroy(gameObject);
        }
    }
}
