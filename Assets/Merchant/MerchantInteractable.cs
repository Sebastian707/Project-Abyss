using UnityEngine;

public class MerchantInteractable : Interactable
{
    [Header("Merchant")]
    public MerchantData merchantData;
    public MerchantUI merchantUI;
    public SellUI sellUI;

    protected override void Interact()
    {
        if (merchantData == null)
        {
            Debug.LogWarning($"[MerchantInteractable] No MerchantData assigned on {gameObject.name}.");
            return;
        }

        if (merchantUI == null || sellUI == null)
        {
            Debug.LogWarning($"[MerchantInteractable] MerchantUI or SellUI not assigned on {gameObject.name}.");
            return;
        }

        if (merchantUI.IsOpen())
        {
            merchantUI.Close();
            sellUI.Close();
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0;
            merchantUI.OpenWith(merchantData);
            sellUI.Open(merchantData);
        }
    }
}