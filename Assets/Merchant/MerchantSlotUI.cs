using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MerchantSlotUI : MonoBehaviour
{
    [Header("UI References")]
    public Image itemIcon;
    public TMP_Text itemNameText;
    public TMP_Text itemDescriptionText;  
    public TMP_Text stackCountText;      
    public TMP_Text priceText;         
    public Button buyButton;

    [HideInInspector] public MerchantQuantityPopup quantityPopup;

    private TetrisItem _item;
    private int _stock;
    private int _unitPrice; 

    public void Setup(TetrisItem item, int stock, MerchantQuantityPopup popup, MerchantData merchantData)
    {
        _item = item;
        _stock = stock;
        quantityPopup = popup;
        _unitPrice = merchantData.GetPrice(item);

        if (itemIcon != null && item.itemIcon != null)
            itemIcon.sprite = item.itemIcon;

        if (itemNameText != null)
            itemNameText.text = item.itemName;

        if (itemDescriptionText != null)
            itemDescriptionText.text = item.itemDescription;

        if (stackCountText != null)
        {
            stackCountText.gameObject.SetActive(stock > 1);
            if (stock > 1)
                stackCountText.text = $"x{stock}";
        }

        UpdatePriceLabel(1);

        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(OnBuyClicked);
    }

    private void UpdatePriceLabel(int quantity)
    {
        if (priceText != null)
            priceText.text = $"{_unitPrice * quantity}g";
    }

    private void OnBuyClicked()
    {
        if (_item == null) return;

        if (_stock > 1)
        {
            quantityPopup.Open(_item, _stock, _unitPrice, (amount) =>
            {
                PurchaseAmount(amount);
            });
        }
        else
        {
            PurchaseAmount(1);
        }
    }

    private void PurchaseAmount(int amount)
    {
        int totalCost = _unitPrice * amount;

        if (!PlayerWealth.Instance.CanAfford(totalCost))
        {
            Debug.LogWarning($"[Merchant] Can't afford {amount}x {_item.itemName} ({totalCost}g).");
            return;
        }

        int successCount = 0;
        for (int i = 0; i < amount; i++)
        {
            bool added = TetrisSlot.instanceSlot.addInFirstSpace(_item);
            if (added) successCount++;
            else break;
        }

        if (successCount > 0)
        {
            PlayerWealth.Instance.TrySpend(_unitPrice * successCount);
            Debug.Log($"[Merchant] Bought {successCount}x {_item.itemName} for {_unitPrice * successCount}g.");

            _stock -= successCount;

            Ammo ammo = _item as Ammo;
            if (ammo != null)
            {
                foreach (RechargeableSystem rs in FindObjectsOfType<RechargeableSystem>())
                {
                    if (rs.currentPower <= 0f && ammo.AmmoID == rs.batteryItemID)
                        rs.TryConsumeBatteryFromInventory();
                }
            }

            if (_stock <= 0)
            {
                Destroy(gameObject);
            }
            else
            {
                if (stackCountText != null)
                    stackCountText.text = $"x{_stock}";

                UpdatePriceLabel(1);
            }
        }
        else
        {
            Debug.LogWarning($"[Merchant] Inventory full — could not add {_item.itemName}.");
        }
    }
}