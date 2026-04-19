using UnityEngine;

public class MerchantInteractable : Interactable
{
    [Header("Merchant")]
    [Tooltip("Unique ScriptableObject per merchant — set their stock here.")]
    public MerchantData merchantData;

    [Tooltip("The single shared MerchantUI panel in the scene.")]
    public MerchantUI merchantUI;

    protected override void Interact()
    {
        if (merchantData == null)
        {
            Debug.LogWarning($"[MerchantInteractable] No MerchantData assigned on {gameObject.name}");
            return;
        }

        if (merchantUI == null)
        {
            Debug.LogWarning($"[MerchantInteractable] No MerchantUI assigned on {gameObject.name}");
            return;
        }
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0;
        merchantUI.OpenWith(merchantData);
    }
}