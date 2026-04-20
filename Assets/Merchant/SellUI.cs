using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Attach to your sell panel. Reads from TetrisSlot.instanceSlot.itensInBag
/// and populates a scroll view with items this merchant will actually buy.
/// Refreshes every time it opens so newly acquired items always appear.
/// </summary>
public class SellUI : MonoBehaviour
{
    [Header("UI Panel")]
    public GameObject sellPanel;

    [Header("Scroll View")]
    public Transform scrollContent;
    public GameObject sellSlotPrefab;

    private MerchantData _currentMerchant;

    // ------------------------------------------------------------------ //

    private void Start()
    {
        sellPanel.SetActive(false);
    }

    public void Open(MerchantData merchantData)
    {
        _currentMerchant = merchantData;
        PopulateList();
        sellPanel.SetActive(true);
    }

    public void Close()
    {
        sellPanel.SetActive(false);
    }

    public bool IsOpen() => sellPanel.activeSelf;

    // ------------------------------------------------------------------ //

    private void PopulateList()
    {
        foreach (Transform child in scrollContent)
            Destroy(child.gameObject);

        if (TetrisSlot.instanceSlot == null)
        {
            Debug.LogWarning("[SellUI] No TetrisSlot instance found.");
            return;
        }

        foreach (TetrisItemSlot inventorySlot in TetrisSlot.instanceSlot.itensInBag)
        {
            if (inventorySlot == null || inventorySlot.item == null) continue;

            // Skip items with no value
            if (inventorySlot.item.basePrice <= 0) continue;

            // Skip items this merchant doesn't accept
            if (!_currentMerchant.WillBuyItem(inventorySlot.item)) continue;

            GameObject row = Instantiate(sellSlotPrefab, scrollContent);
            SellSlotUI slot = row.GetComponent<SellSlotUI>();

            if (slot != null)
                slot.Setup(inventorySlot, _currentMerchant);
            else
                Debug.LogError("[SellUI] sellSlotPrefab is missing a SellSlotUI component.");
        }

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(scrollContent.GetComponent<RectTransform>());
    }
}