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
    /// <summary>
    /// Derived automatically from TetrisSlot's grid dimensions (maxGridX * maxGridY).
    /// Do not set this manually — configure the grid size in TetrisSlot instead.
    /// </summary>
    public int numberSlots
    {
        get
        {
            if (TetrisSlot.instanceSlot != null)
                return TetrisSlot.instanceSlot.maxGridX * TetrisSlot.instanceSlot.maxGridY - 1;
            Debug.LogWarning("TetrisSlot instance not found. Returning 0 for NumberSlots.");
            return 0;
        }
    }
}