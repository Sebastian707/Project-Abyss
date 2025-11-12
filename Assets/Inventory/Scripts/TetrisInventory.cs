using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisInventory : MonoBehaviour
{
    // Responsible for having just one inventory in the scene.
    #region Singleton
    public static TetrisInventory instanceTetris;

    void Awake()
    {
        if (instanceTetris != null)
        {
            Debug.LogWarning("More than one Tetris inventory");
            return;
        }
        instanceTetris = this;
    }
    #endregion

    [SerializeField]
    public int numberSlots = 1; // Default value

    // Property to handle changes
    public int NumberSlots
    {
        get => numberSlots;
        set
        {
            if (numberSlots != value)
            {
                numberSlots = Mathf.Max(1, value); // Ensure at least 1 slot
                UpdateInventorySlots();
            }
        }
    }

    // This method runs when number of slots changes
    private void UpdateInventorySlots()
    {
        Debug.Log("Inventory slots updated to: " + numberSlots);
        // TODO: Add your logic here to handle UI or other slot-related changes
    }

    // Optional: updates when changing in the inspector
    private void OnValidate()
    {
        NumberSlots = numberSlots;
    }
}