using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// One row in the sell scroll view.
/// Prefab needs: Image (icon), TMP_Text (name), TMP_Text (stack), TMP_Text (price), Button (sell).
/// </summary>
public class SellSlotUI : MonoBehaviour
{
    [Header("UI References")]
    public Image itemIcon;
    public TMP_Text itemNameText;
    public TMP_Text stackCountText;
    public TMP_Text priceText;
    public Button sellButton;

    private TetrisItemSlot _inventorySlot;
    private int _sellPrice; // per single item, calculated from merchant tags

    // ------------------------------------------------------------------ //

    public void Setup(TetrisItemSlot inventorySlot, MerchantData merchantData)
    {
        _inventorySlot = inventorySlot;

        // Sell price uses same tag modifiers as buying, scaled by sellMultiplier
        _sellPrice = merchantData.GetSellPrice(inventorySlot.item);

        if (itemIcon != null && inventorySlot.item.itemIcon != null)
            itemIcon.sprite = inventorySlot.item.itemIcon;

        if (itemNameText != null)
            itemNameText.text = inventorySlot.item.itemName;

        RefreshUI();

        sellButton.onClick.RemoveAllListeners();
        sellButton.onClick.AddListener(OnSellClicked);
    }

    // ------------------------------------------------------------------ //

    private void RefreshUI()
    {
        if (_inventorySlot == null) return;

        if (stackCountText != null)
        {
            bool showStack = _inventorySlot.currentStack > 1;
            stackCountText.gameObject.SetActive(showStack);
            if (showStack)
                stackCountText.text = $"x{_inventorySlot.currentStack}";
        }

        if (priceText != null)
            priceText.text = $"{_sellPrice * _inventorySlot.currentStack}g";
    }

    private void OnSellClicked()
    {
        if (_inventorySlot == null) return;

        int totalEarned = _sellPrice * _inventorySlot.currentStack;

        TetrisSlot inventory = TetrisSlot.instanceSlot;

        int contX = (int)_inventorySlot.item.itemSize.x;
        int contY = (int)_inventorySlot.item.itemSize.y;
        int startX = (int)_inventorySlot.startPosition.x;
        int startY = (int)_inventorySlot.startPosition.y;

        // Bounds-checked grid clear
        for (int x = 0; x < contX; x++)
        {
            for (int y = 0; y < contY; y++)
            {
                int gx = startX + x;
                int gy = startY + y;

                if (gx >= 0 && gx < inventory.maxGridX && gy >= 0 && gy < inventory.maxGridY)
                    inventory.grid[gx, gy] = 0;
                else
                    Debug.LogWarning($"[SellUI] Grid position ({gx},{gy}) out of bounds — skipping.");
            }
        }

        inventory.itensInBag.Remove(_inventorySlot);
        Destroy(_inventorySlot.gameObject);

        PlayerWealth.Instance.Earn(totalEarned);
        Debug.Log($"[Sell] Sold {_inventorySlot.item.itemName} for {totalEarned}g.");

        Destroy(gameObject);
    }
}