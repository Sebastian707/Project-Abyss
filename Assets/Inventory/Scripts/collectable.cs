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
            Ammo ammo = itemTetris as Ammo;
            if (ammo != null) // If this IS a battery
            {
                foreach (RechargeableSystem rs in FindObjectsOfType<RechargeableSystem>())
                {
                    if (rs.currentPower <= 0f && ammo.AmmoID == rs.batteryItemID)
                    {
                        rs.TryConsumeBatteryFromInventory();
                    }
                }
            }

            Destroy(gameObject);
        }
    }
}

