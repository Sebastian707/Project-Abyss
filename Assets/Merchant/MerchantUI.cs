using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MerchantUI : MonoBehaviour
{
    [Header("UI Panel")]
    public GameObject merchantPanel;
    public TMP_Text merchantNameText;

    [Header("Scroll View")]
    public Transform scrollContent;
    public GameObject slotPrefab;

    [Header("Quantity Popup")]
    public MerchantQuantityPopup quantityPopup;

    private MerchantData _currentData;

    private void Start()
    {
        merchantPanel.SetActive(false);
    }

    public void OpenWith(MerchantData data)
    {
        if (_currentData != data)
        {
            _currentData = data;
            PopulateList(data);
        }

        if (merchantNameText != null)
            merchantNameText.text = data.merchantName;

        merchantPanel.SetActive(true);
    }

    public void Close()
    {
        merchantPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1;
    }

    public void Toggle()
    {
        if (merchantPanel.activeSelf) Close();
        else if (_currentData != null) OpenWith(_currentData);
    }

    private void PopulateList(MerchantData data)
    {
        foreach (Transform child in scrollContent)
            Destroy(child.gameObject);

        foreach (MerchantStock entry in data.itemsForSale)
        {
            if (entry.item == null) continue;

            GameObject row = Instantiate(slotPrefab, scrollContent);
            MerchantSlotUI slot = row.GetComponent<MerchantSlotUI>();

            if (slot != null)
                slot.Setup(entry.item, entry.stock, quantityPopup, data);
            else
                Debug.LogError("[MerchantUI] slotPrefab is missing a MerchantSlotUI component.");
        }
    }
}