using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisSlot : MonoBehaviour
{
    #region Singleton
    public static TetrisSlot instanceSlot;
    void Awake()
    {
        if (instanceSlot != null)
        {
            Debug.LogWarning("More than one Tetris inventory");
            return;
        }
        instanceSlot = this;
        grid = new int[maxGridX, maxGridY];
    }
    #endregion

    public int[,] grid;
    public TetrisInventory playerInventory;
    public List<TetrisItemSlot> itensInBag = new List<TetrisItemSlot>();
    public int maxGridX;
    public int maxGridY;
    public TetrisItemSlot prefabSlot;
    Vector2 cellSize = new Vector2(34f, 34f);
    List<Vector2> posItemNaBag = new List<Vector2>();

    void Start()
    {
        playerInventory = FindObjectOfType<TetrisInventory>();
    }

    /// <summary>
    /// Adds a new item into the inventory grid or stacks with existing if possible.
    /// Now supports items that give multiple stack counts when picked up (e.g. a box of bullets adds 17 units).
    /// </summary>
    public bool addInFirstSpace(TetrisItem item)
    {
        // 🆕 Determine how many units this pickup represents
        int amountToAdd = Mathf.Max(1, item.amountOnPickup); // default 1 if not set

        // 1️⃣ Try stacking first
        foreach (TetrisItemSlot existingSlot in itensInBag)
        {
            if (existingSlot.CanStackWith(item))
            {
                int spaceLeft = item.MaxStackSize - existingSlot.currentStack;

                // Fill as much as possible into this existing stack
                int amountUsed = Mathf.Min(amountToAdd, spaceLeft);
                existingSlot.AddToStack(amountUsed);
                amountToAdd -= amountUsed;

                // If we fully used up the pickup, stop here
                if (amountToAdd <= 0)
                {
                    Debug.Log($"Stacked {item.itemName}. New count: {existingSlot.currentStack}");
                    return true;
                }
            }
        }

        // 2️⃣ If there’s still some leftover (like overflow bullets), create new stack(s)
        while (amountToAdd > 0)
        {
            int contX = (int)item.itemSize.x;
            int contY = (int)item.itemSize.y;
            posItemNaBag.Clear();

            bool foundSpace = false;

            for (int i = 0; i < maxGridX && !foundSpace; i++)
            {
                for (int j = 0; j < maxGridY && !foundSpace; j++)
                {
                    List<Vector2> tempPos = new List<Vector2>();
                    bool fits = true;

                    for (int y = 0; y < contY && fits; y++)
                    {
                        for (int x = 0; x < contX && fits; x++)
                        {
                            if ((i + x) < maxGridX && (j + y) < maxGridY && grid[i + x, j + y] != 1)
                                tempPos.Add(new Vector2(i + x, j + y));
                            else
                                fits = false;
                        }
                    }

                    if (fits && tempPos.Count == (contX * contY))
                    {
                        posItemNaBag = tempPos;
                        foundSpace = true;
                    }
                }
            }

            if (!foundSpace)
            {
                Debug.LogWarning($"No space left for {item.itemName}");
                return false;
            }

            // Create a new slot
            TetrisItemSlot myItem = Instantiate(prefabSlot);
            myItem.startPosition = new Vector2(posItemNaBag[0].x, posItemNaBag[0].y);
            myItem.item = item;
            myItem.icon.sprite = item.itemIcon;

            myItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            myItem.GetComponent<RectTransform>().anchorMax = new Vector2(0f, 1f);
            myItem.GetComponent<RectTransform>().anchorMin = new Vector2(0f, 1f);
            myItem.transform.SetParent(this.GetComponent<RectTransform>(), false);
            myItem.gameObject.transform.localScale = Vector3.one;
            myItem.GetComponent<RectTransform>().anchoredPosition =
                new Vector2(myItem.startPosition.x * cellSize.x, -myItem.startPosition.y * cellSize.y);

            // Assign stack size to fill as much as possible
            int amountThisStack = Mathf.Min(amountToAdd, item.MaxStackSize);
            myItem.currentStack = amountThisStack;
            amountToAdd -= amountThisStack;

            myItem.UpdateStackUI();
            itensInBag.Add(myItem);

            foreach (Vector2 pos in posItemNaBag)
                grid[(int)pos.x, (int)pos.y] = 1;

            posItemNaBag.Clear();
        }

        return true;
    }
}
