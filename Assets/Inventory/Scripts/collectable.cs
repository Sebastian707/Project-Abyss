using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collectable : Interactable
{
    [Header("Item Data")]
    public TetrisItem itemTetris;

    [Header("Persistence")]
    public string pickupID; // unique per object in the scene

    protected override void Interact()
    {
        bool wasPickedUpTetris = TetrisSlot.instanceSlot.addInFirstSpace(itemTetris);
        if (wasPickedUpTetris)
        {
            // Handle battery/rechargeable logic
            Ammo ammo = itemTetris as Ammo;
            if (ammo != null)
            {
                foreach (RechargeableSystem rs in FindObjectsOfType<RechargeableSystem>())
                {
                    if (rs.currentPower <= 0f && ammo.AmmoID == rs.batteryItemID)
                    {
                        rs.TryConsumeBatteryFromInventory();
                    }
                }
            }

            // Record as collected in save for persistence
            InventoryManager.Instance.AddPickedUp(pickupID);

            // Remove object from scene
            Destroy(gameObject);
        }
    }
}
