using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public abstract class RechargeableSystem : MonoBehaviour
{
    [Header("Recharge Settings")]
    public float maxPower = 100f;
    public float currentPower;
    public string batteryItemID = "FlashlightBattery"; // Can be overridden per item
    public TextMeshProUGUI uiText;

    protected TetrisSlot playerSlot;

    protected virtual void Awake()
    {
        playerSlot = TetrisSlot.instanceSlot ?? FindObjectOfType<TetrisSlot>();
        currentPower = maxPower;
        UpdateUI();
    }

    public bool TryConsumeBatteryFromInventory()
    {
        if (playerSlot == null || playerSlot.itensInBag == null) return false;

        List<TetrisItemSlot> bag = playerSlot.itensInBag;

        for (int i = bag.Count - 1; i >= 0; i--)
        {
            TetrisItemSlot slot = bag[i];
            if (slot == null || slot.item == null) continue;

            Ammo ammo = slot.item as Ammo;
            if (ammo != null && ammo.AmmoID == batteryItemID)
            {
                ClearGridCellsForSlot(slot);
                bag.RemoveAt(i);
                Destroy(slot.gameObject);

                currentPower = maxPower;
                UpdateUI();
                return true;
            }
        }
        return false;
    }

    void ClearGridCellsForSlot(TetrisItemSlot slot)
    {
        if (playerSlot == null || playerSlot.grid == null) return;
        if (slot == null || slot.item == null) return;

        Vector2 start = slot.startPosition;
        Vector2 size = slot.item.itemSize;

        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                int gx = (int)(start.x + x);
                int gy = (int)(start.y + y);
                if (gx >= 0 && gx < playerSlot.maxGridX && gy >= 0 && gy < playerSlot.maxGridY)
                    playerSlot.grid[gx, gy] = 0;
            }
        }
    }

    public void UpdateUI()
    {
        if (uiText)
        {
            int pct = Mathf.RoundToInt((currentPower / maxPower) * 100f);
            uiText.text = pct + "%";
        }
    }
}
