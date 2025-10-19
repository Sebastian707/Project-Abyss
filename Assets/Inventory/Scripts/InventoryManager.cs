using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    private void Start()
    {
        Instance = this;
        LoadInventory();
    }


    [Header("References")]
    public TetrisListItens listItens;
    public TetrisSlot slotGrid;

    [Header("Save Settings")]
    public string saveFileName = "save.json";

    [HideInInspector]
    public List<string> pickedPickups = new List<string>(); // new: store collected objects

    #region Save/Load Inventory
    public void SaveInventory()
    {
        InventorySaveData saveData = new InventorySaveData
        {
            items = new List<InventoryItemData>(),
            pickedPickups = pickedPickups // save collected pickups
        };

        TetrisItemSlot[] slots = FindObjectsOfType<TetrisItemSlot>();
        foreach (var slot in slots)
        {
            if (slot.item != null)
            {
                saveData.items.Add(new InventoryItemData
                {
                    itemID = slot.item.itemID,
                    position = slot.startPosition,
                    stackCount = slot.currentStack
                });
            }
        }

        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(Path.Combine(Application.persistentDataPath, saveFileName), json);
        Debug.Log("Inventory saved at: " + Path.Combine(Application.persistentDataPath, saveFileName));
    }

    public void LoadInventory()
    {
        string path = Path.Combine(Application.persistentDataPath, saveFileName);
        if (!File.Exists(path)) return;

        string json = File.ReadAllText(path);
        InventorySaveData saveData = JsonUtility.FromJson<InventorySaveData>(json);

        // Restore inventory
        pickedPickups = saveData.pickedPickups ?? new List<string>();

        slotGrid.itensInBag.Clear();
        slotGrid.grid = new int[slotGrid.maxGridX, slotGrid.maxGridY];

        TetrisItemSlot[] currentSlots = FindObjectsOfType<TetrisItemSlot>();
        foreach (var s in currentSlots)
            Destroy(s.gameObject);

        foreach (var itemData in saveData.items)
        {
            TetrisItem itemSO = GetItemSOFromID(itemData.itemID);
            if (itemSO == null) continue;

            // Instantiate slot
            TetrisItemSlot newSlot = Instantiate(slotGrid.prefabSlot, slotGrid.transform);
            newSlot.item = itemSO;
            newSlot.startPosition = itemData.position;
            newSlot.currentStack = itemData.stackCount;
            newSlot.icon.sprite = itemSO.itemIcon;

            // Set rect transform
            RectTransform rt = newSlot.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0f, 1f);
            rt.anchorMax = new Vector2(0f, 1f);
            rt.localScale = Vector3.one;
            rt.anchoredPosition = new Vector2(newSlot.startPosition.x * 34f, -newSlot.startPosition.y * 34f);

            // Update UI
            newSlot.UpdateStackUI();

            // Mark occupied slots in the grid
            List<Vector2> occupiedPositions = new List<Vector2>();
            for (int y = 0; y < newSlot.item.itemSize.y; y++)
            {
                for (int x = 0; x < newSlot.item.itemSize.x; x++)
                {
                    Vector2 pos = new Vector2(newSlot.startPosition.x + x, newSlot.startPosition.y + y);
                    occupiedPositions.Add(pos);
                    slotGrid.grid[(int)pos.x, (int)pos.y] = 1;
                }
            }

            slotGrid.itensInBag.Add(newSlot);
        }

        // Remove collected pickups from scene
        RemovePickedUpObjects();
    }
    #endregion

    #region Helpers
    public void AddPickedUp(string id)
    {
        if (!pickedPickups.Contains(id))
            pickedPickups.Add(id);
    }

    private void RemovePickedUpObjects()
    {
        collectable[] pickups = FindObjectsOfType<collectable>();
        foreach (var pickup in pickups)
        {
            if (pickedPickups.Contains(pickup.pickupID))
                Destroy(pickup.gameObject);
        }
    }

    private TetrisItem GetItemSOFromID(string id)
    {
        foreach (var item in listItens.itens)
        {
            if (item != null && item.itemID == id)
                return item;
        }
        return null;
    }
    #endregion
}
