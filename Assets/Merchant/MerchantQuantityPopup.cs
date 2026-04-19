using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MerchantQuantityPopup : MonoBehaviour
{
    [Header("UI References")]
    public Slider quantitySlider;
    public TMP_Text quantityLabel;   
    public TMP_Text totalPriceLabel; 
    public TMP_Text itemNameLabel;  
    public Button confirmButton;
    public Button cancelButton;

    private Action<int> _onConfirm;
    private int _unitPrice;

    private void Awake()
    {
        confirmButton.onClick.AddListener(OnConfirm);
        cancelButton.onClick.AddListener(OnCancel);
        gameObject.SetActive(false);
    }

    public void Open(TetrisItem item, int stock, int unitPrice, Action<int> onConfirm)
    {
        _onConfirm = onConfirm;
        _unitPrice = unitPrice;

        quantitySlider.minValue = 1;
        quantitySlider.maxValue = stock;
        quantitySlider.wholeNumbers = true;
        quantitySlider.value = 1;

        if (itemNameLabel != null)
            itemNameLabel.text = item.itemName;

        UpdateLabels();

        quantitySlider.onValueChanged.RemoveAllListeners();
        quantitySlider.onValueChanged.AddListener((_) => UpdateLabels());

        gameObject.SetActive(true);
    }

    private void UpdateLabels()
    {
        int amount = (int)quantitySlider.value;

        if (quantityLabel != null)
            quantityLabel.text = $"Buy: {amount}";

        if (totalPriceLabel != null)
            totalPriceLabel.text = $"Total: {_unitPrice * amount}g";
    }

    private void OnConfirm()
    {
        int amount = (int)quantitySlider.value;
        gameObject.SetActive(false);
        _onConfirm?.Invoke(amount);
    }

    private void OnCancel()
    {
        gameObject.SetActive(false);
    }
}