using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [Header("References")]
    public TetrisListItens listItens;
    public TetrisSlot slotGrid;
    public Transform playerTransform; // Reference to player

    [Header("Save Settings")]
    public string saveFileName = "save.json";

    [HideInInspector]
    public List<string> pickedPickups = new List<string>();

    private string SavePath => Path.Combine(Application.persistentDataPath, saveFileName);

    private void Awake()
    {
        LoadGame();
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Optional: load automatically when entering game scene
        // LoadGame();
    }

    #region === SAVE / LOAD SYSTEM ===

    public void SaveGame()
    {
        InventorySaveData saveData = new InventorySaveData
        {
            items = GetAllItemsData(),
            pickedPickups = pickedPickups,
            sceneName = SceneManager.GetActiveScene().name,
            playerPosition = playerTransform != null ? playerTransform.position : Vector3.zero
        };

        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(SavePath, json);

        Debug.Log($"✅ Game saved to {SavePath}");
    }

    public void LoadGame()
    {
        if (!File.Exists(SavePath))
        {
            Debug.LogWarning("⚠️ No save file found!");
            return;
        }

        string json = File.ReadAllText(SavePath);
        InventorySaveData saveData = JsonUtility.FromJson<InventorySaveData>(json);

        // If we’re not already in the correct scene, load it first
        if (SceneManager.GetActiveScene().name != saveData.sceneName)
        {
            StartCoroutine(LoadSceneAndRestore(saveData));
            return;
        }

        RestoreGameState(saveData);
    }

    private System.Collections.IEnumerator LoadSceneAndRestore(InventorySaveData saveData)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(saveData.sceneName);
        while (!asyncLoad.isDone)
            yield return null;

        yield return null; // wait one frame for scene init

        RestoreGameState(saveData);
    }

    private void RestoreGameState(InventorySaveData saveData)
    {
        if (playerTransform != null)
            playerTransform.position = saveData.playerPosition;

        LoadInventoryData(saveData);

        Debug.Log($"✅ Game loaded. Scene: {saveData.sceneName}, Position: {saveData.playerPosition}");
    }

    public bool HasSave()
    {
        return File.Exists(SavePath);
    }

    public string GetSavedScene()
    {
        if (!HasSave()) return null;

        string json = File.ReadAllText(SavePath);
        InventorySaveData data = JsonUtility.FromJson<InventorySaveData>(json);
        return data.sceneName;
    }

    #endregion

    #region === INVENTORY ===

    public List<InventoryItemData> GetAllItemsData()
    {
        List<InventoryItemData> items = new List<InventoryItemData>();
        TetrisItemSlot[] slots = FindObjectsOfType<TetrisItemSlot>();

        foreach (var slot in slots)
        {
            if (slot.item != null)
            {
                items.Add(new InventoryItemData
                {
                    itemID = slot.item.itemID,
                    position = slot.startPosition,
                    stackCount = slot.currentStack
                });
            }
        }

        return items;
    }

    public void LoadInventoryData(InventorySaveData saveData)
    {
        pickedPickups = saveData.pickedPickups ?? new List<string>();

        slotGrid.itensInBag.Clear();
        slotGrid.grid = new int[slotGrid.maxGridX, slotGrid.maxGridY];

        // Destroy any existing item slots
        TetrisItemSlot[] currentSlots = FindObjectsOfType<TetrisItemSlot>();
        foreach (var s in currentSlots)
            Destroy(s.gameObject);

        foreach (var itemData in saveData.items)
        {
            TetrisItem itemSO = GetItemSOFromID(itemData.itemID);
            if (itemSO == null) continue;

            TetrisItemSlot newSlot = Instantiate(slotGrid.prefabSlot, slotGrid.transform);
            newSlot.item = itemSO;
            newSlot.startPosition = itemData.position;
            newSlot.currentStack = itemData.stackCount;
            newSlot.icon.sprite = itemSO.itemIcon;

            RectTransform rt = newSlot.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0f, 1f);
            rt.anchorMax = new Vector2(0f, 1f);
            rt.localScale = Vector3.one;
            rt.anchoredPosition = new Vector2(newSlot.startPosition.x * 34f, -newSlot.startPosition.y * 34f);

            newSlot.UpdateStackUI();
            slotGrid.itensInBag.Add(newSlot);
        }

        RemovePickedUpObjects();
    }

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
