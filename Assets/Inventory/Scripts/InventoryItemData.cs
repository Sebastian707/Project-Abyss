using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventorySaveData
{
    public List<InventoryItemData> items;
    public List<string> pickedPickups;

    // New fields
    public string sceneName;
    public Vector3 playerPosition;
}

[System.Serializable]
public class InventoryItemData
{
    public string itemID;
    public Vector2 position;
    public int stackCount;
}


[System.Serializable]
public class SceneSaveData
{
    public string sceneName;
    public Vector3 playerPosition;
}
