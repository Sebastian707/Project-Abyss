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

    public void ExpandGrid(int addX, int addY)
    {
        if (addX < 0 || addY < 0)
        {
            Debug.LogWarning("ExpandGrid: addX and addY must be >= 0.");
            return;
        }

        if (addX == 0 && addY == 0)
            return;

        int oldMaxX = maxGridX;
        int oldMaxY = maxGridY;
        int newMaxX = maxGridX + addX;
        int newMaxY = maxGridY + addY;

        int[,] newGrid = new int[newMaxX, newMaxY];
        for (int x = 0; x < oldMaxX; x++)
            for (int y = 0; y < oldMaxY; y++)
                newGrid[x, y] = grid[x, y];

        grid = newGrid;
        maxGridX = newMaxX;
        maxGridY = newMaxY;

        if (TetrisUI.instanceUI != null)
            TetrisUI.instanceUI.AddSlots(oldMaxX, oldMaxY, newMaxX, newMaxY);
        else
            Debug.LogWarning("ExpandGrid: No TetrisUI instance found.");

        Debug.Log($"Inventory expanded to {maxGridX}x{maxGridY}.");
    }

    public bool addInFirstSpace(TetrisItem item)
    {
        int amountToAdd = Mathf.Max(1, item.amountOnPickup);

        // 1. Try stacking first
        foreach (TetrisItemSlot existingSlot in itensInBag)
        {
            if (existingSlot.CanStackWith(item))
            {
                int spaceLeft = item.MaxStackSize - existingSlot.currentStack;
                int amountUsed = Mathf.Min(amountToAdd, spaceLeft);
                existingSlot.AddToStack(amountUsed);
                amountToAdd -= amountUsed;

                if (amountToAdd <= 0)
                {
                    Debug.Log($"Stacked {item.itemName}. New count: {existingSlot.currentStack}");
                    return true;
                }
            }
        }

        // 2. Place remaining into new grid slots
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

            TetrisItemSlot myItem = Instantiate(prefabSlot);
            myItem.startPosition = new Vector2(posItemNaBag[0].x, posItemNaBag[0].y);
            myItem.item = item;
            myItem.icon.sprite = item.itemIcon;

            myItem.transform.SetParent(this.GetComponent<RectTransform>(), false);

            RectTransform rt = myItem.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0f, 1f);
            rt.anchorMax = new Vector2(0f, 1f);
            rt.pivot = new Vector2(0f, 1f);
            rt.anchoredPosition = new Vector2(
                myItem.startPosition.x * cellSize.x,
               -myItem.startPosition.y * cellSize.y
            );
            rt.localScale = Vector3.one;

            int amountThisStack = Mathf.Min(amountToAdd, item.MaxStackSize);
            myItem.currentStack = amountThisStack;
            amountToAdd -= amountThisStack;

            myItem.UpdateStackUI();
            itensInBag.Add(myItem);

            // FIX: mark grid BEFORE clearing the list
            foreach (Vector2 pos in posItemNaBag)
            {
                grid[(int)pos.x, (int)pos.y] = 1;
            }


            posItemNaBag.Clear();
        }

        return true;
    }
}