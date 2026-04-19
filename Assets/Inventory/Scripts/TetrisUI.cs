using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TetrisUI : MonoBehaviour
{
    public static TetrisUI instanceUI;

    [SerializeField] GameObject slotPrefab;

    readonly Vector2 cellSize = new Vector2(34f, 34f);
    void Awake()
    {
        instanceUI = this;
        GridLayoutGroup glg = GetComponent<GridLayoutGroup>();
        if (glg != null)
            Destroy(glg);
    }
    void Start()
    {
        DrawGrid(TetrisSlot.instanceSlot.maxGridX, TetrisSlot.instanceSlot.maxGridY);
    }
    void DrawGrid(int sizeX, int sizeY)
    {
        for (int y = 0; y < sizeY; y++)
            for (int x = 0; x < sizeX; x++)
                SpawnCell(x, y);
    }
    public void AddSlots(int oldSizeX, int oldSizeY, int newSizeX, int newSizeY)
    {
        for (int y = 0; y < oldSizeY; y++)
            for (int x = oldSizeX; x < newSizeX; x++)
                SpawnCell(x, y);
        for (int y = oldSizeY; y < newSizeY; y++)
            for (int x = 0; x < newSizeX; x++)
                SpawnCell(x, y);
    }
    void SpawnCell(int x, int y)
    {
        GameObject cell = Instantiate(slotPrefab, transform);
        RectTransform rt = cell.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0f, 1f);
        rt.anchorMax = new Vector2(0f, 1f);
        rt.pivot = new Vector2(0f, 1f);
        rt.sizeDelta = cellSize;
        rt.anchoredPosition = new Vector2(x * cellSize.x, -y * cellSize.y);
    }
}
