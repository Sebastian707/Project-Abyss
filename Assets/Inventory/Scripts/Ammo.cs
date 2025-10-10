using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ammo Item", menuName = "Inventory/Tetris/AmmoItem")]
public class Ammo : TetrisItem
{
    [Header("Ammo Settings")]
    public string AmmoID;


    public override void Use()
    {
        if (!usable) return; 

        FlashlightSystem flashlight = Object.FindObjectOfType<FlashlightSystem>();
        if (flashlight == null)
        {
            Debug.LogWarning("No FlashlightSystem found in scene!");
            return;
        }

        if (flashlight.currentPower >= flashlight.maxPower)
        {
            Debug.Log("Flashlight already fully charged.");
            return;
        }

        flashlight.currentPower = flashlight.maxPower;
        flashlight.UpdateUI();

        TetrisSlot playerSlot = TetrisSlot.instanceSlot;
        if (playerSlot != null)
        {
            for (int i = playerSlot.itensInBag.Count - 1; i >= 0; i--)
            {
                TetrisItemSlot slot = playerSlot.itensInBag[i];
                if (slot != null && slot.item == this)
                {
                    flashlight.TryConsumeBatteryFromInventory();
                    break;
                }
            }
        }

        Debug.Log("Used Battery: Flashlight recharged.");
    }
}
