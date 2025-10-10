using UnityEngine;

public class InventoryItemActivator : MonoBehaviour
{
    [Header("Target Settings")]
    [Tooltip("The ScriptableObject item to check for in the player's inventory.")]
    public TetrisItem targetItem;

    [Tooltip("The GameObject to activate when the target item is in the inventory.")]
    public GameObject objectToActivate;

    [Header("Performance Settings")]
    [Tooltip("How often to check the inventory (in seconds). Lower = more responsive, higher = less CPU use.")]
    public float checkInterval = 0.25f;

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= checkInterval)
        {
            timer = 0f;
            CheckInventory();
        }
    }

    private void CheckInventory()
    {
        // Safety checks
        if (objectToActivate == null || targetItem == null)
            return;

        TetrisSlot playerSlot = TetrisSlot.instanceSlot;
        if (playerSlot == null)
            return;

        bool itemFound = false;

        // Loop through all slots to find the target item
        foreach (TetrisItemSlot slot in playerSlot.itensInBag)
        {
            if (slot != null && slot.item == targetItem)
            {
                itemFound = true;
                break;
            }
        }

        // Activate or deactivate based on whether item is found
        if (objectToActivate.activeSelf != itemFound)
        {
            objectToActivate.SetActive(itemFound);
        }
    }
}
