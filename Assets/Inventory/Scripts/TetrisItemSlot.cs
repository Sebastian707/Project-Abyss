using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class TetrisItemSlot : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler,
    IPointerEnterHandler, IPointerExitHandler, IDropHandler
{
    public Vector2 size = new Vector2(34f, 34f); // grid cell base size
    public TetrisItem item;

    public Vector2 startPosition;
    public Vector2 oldPosition;
    public Image icon;
    private bool isDragging = false;


    [Header("Stack Settings")]
    public TMP_Text stackCountText; // assigned in prefab
    public int currentStack = 1;

    private TetrisSlot slots;

    void Awake()
    {
        if (stackCountText == null)
            stackCountText = GetComponentInChildren<TMP_Text>();
    }

    void Start()
    {
        // --- Rescale item visuals ---
        RescaleItem();

        slots = FindObjectOfType<TetrisSlot>();

        // Mark occupied grid cells based on item size
        for (int y = 0; y < item.itemSize.y; y++)
        {
            for (int x = 0; x < item.itemSize.x; x++)
            {
                int gx = (int)(startPosition.x + x);
                int gy = (int)(startPosition.y + y);
                if (gx >= 0 && gx < slots.maxGridX && gy >= 0 && gy < slots.maxGridY)
                    slots.grid[gx, gy] = 1;
            }
        }

        currentStack = Mathf.Max(1, currentStack);
        UpdateStackUI();
    }

    void RescaleItem()
    {
        RectTransform rt = GetComponent<RectTransform>();
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, item.itemSize.y * size.y);
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, item.itemSize.x * size.x);

        foreach (RectTransform child in transform)
        {
            child.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, item.itemSize.y * child.rect.height);
            child.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, item.itemSize.x * child.rect.width);

            foreach (RectTransform iconChild in child)
            {
                iconChild.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, item.itemSize.y * iconChild.rect.height);
                iconChild.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, item.itemSize.x * iconChild.rect.width);
                iconChild.localPosition = new Vector2(
                    child.localPosition.x + child.rect.width / 2,
                    child.localPosition.y - child.rect.height / 2);
            }
        }
    }

    public void UpdateStackUI()
    {
        if (stackCountText != null)
        {
            if (currentStack > 1)
            {
                stackCountText.gameObject.SetActive(true);
                stackCountText.text = currentStack.ToString();
            }
            else
            {
                stackCountText.gameObject.SetActive(false);
                stackCountText.text = "";
            }
        }
    }

    public bool CanStackWith(TetrisItem other)
    {
        return other != null && other.itemID == item.itemID && currentStack < item.MaxStackSize;
    }

    public void AddToStack(int amount)
    {
        currentStack += amount;
        if (currentStack > item.MaxStackSize)
            currentStack = item.MaxStackSize;
        UpdateStackUI();
    }

    #region Pointer Hover
    public void OnPointerEnter(PointerEventData eventData)
    {
        Functionalities descript = FindObjectOfType<Functionalities>();
        descript.changeDescription(item.itemName, item.itemDescription, item.getAtt1(), item.rarity, item.getAtt1Icon());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Functionalities descript = FindObjectOfType<Functionalities>();
        descript.changeDescription("", "", 0, "");
    }
    #endregion

    #region Drag Handlers
    public void OnBeginDrag(PointerEventData eventData)
    {
        oldPosition = GetComponent<RectTransform>().anchoredPosition;
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
        // Temporarily clear occupied grid
        for (int y = 0; y < item.itemSize.y; y++)
            for (int x = 0; x < item.itemSize.x; x++)
                slots.grid[(int)startPosition.x + x, (int)startPosition.y + y] = 0;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        GetComponent<CanvasGroup>().blocksRaycasts = true;

        if (EventSystem.current.IsPointerOverGameObject())
        {
            Vector2 finalPos = GetComponent<RectTransform>().anchoredPosition;
            Vector2 finalSlot;
            finalSlot.x = Mathf.Floor(finalPos.x / size.x);
            finalSlot.y = Mathf.Floor(-finalPos.y / size.y);

            if (((int)finalSlot.x + (int)item.itemSize.x - 1) < slots.maxGridX &&
                ((int)finalSlot.y + (int)item.itemSize.y - 1) < slots.maxGridY &&
                (finalSlot.x >= 0 && finalSlot.y >= 0))
            {
                List<Vector2> newPosItem = new List<Vector2>();
                bool fit = false;

                for (int y = 0; y < item.itemSize.y; y++)
                {
                    for (int x = 0; x < item.itemSize.x; x++)
                    {
                        int gridX = (int)finalSlot.x + x;
                        int gridY = (int)finalSlot.y + y;

                        if (slots.grid[gridX, gridY] != 1)
                        {
                            newPosItem.Add(new Vector2(gridX, gridY));
                            fit = true;
                        }
                        else
                        {
                            fit = false;
                            transform.GetComponent<RectTransform>().anchoredPosition = oldPosition;
                            newPosItem.Clear();
                            x = (int)item.itemSize.x;
                            y = (int)item.itemSize.y;
                        }
                    }
                }

                if (fit)
                {
                    // Clear old grid
                    for (int y = 0; y < item.itemSize.y; y++)
                        for (int x = 0; x < item.itemSize.x; x++)
                            slots.grid[(int)startPosition.x + x, (int)startPosition.y + y] = 0;

                    // Mark new grid
                    foreach (Vector2 pos in newPosItem)
                        slots.grid[(int)pos.x, (int)pos.y] = 1;

                    startPosition = newPosItem[0];
                    transform.GetComponent<RectTransform>().anchoredPosition =
                        new Vector2(startPosition.x * size.x, -startPosition.y * size.y);
                }
                else
                {
                    // Re-mark old grid (fixed bug here)
                    for (int y = 0; y < item.itemSize.y; y++)
                        for (int x = 0; x < item.itemSize.x; x++)
                            slots.grid[(int)startPosition.x + x, (int)startPosition.y + y] = 1;
                }
            }
            else
            {
                transform.GetComponent<RectTransform>().anchoredPosition = oldPosition;
            }
        }
        else
        {
            DropOutsideInventory();
        }
    }

    void DropOutsideInventory()
    {
        PlayerController player = FindObjectOfType<PlayerController>();
        TetrisListItens itenInGame = FindObjectOfType<TetrisListItens>();

        for (int t = 0; t < itenInGame.prefabs.Length; t++)
        {
            if (itenInGame.itens[t].itemName == item.itemName)
            {
                Instantiate(itenInGame.prefabs[t].gameObject,
                    new Vector2(player.transform.position.x + Random.Range(-1.5f, 1.5f),
                                player.transform.position.y + Random.Range(-1.5f, 1.5f)),
                    Quaternion.identity);
                Destroy(this.gameObject);
                break;
            }
        }
    }
    #endregion

    #region Stack Merge
    public void OnDrop(PointerEventData eventData)
    {
        TetrisItemSlot droppedSlot = eventData.pointerDrag?.GetComponent<TetrisItemSlot>();
        if (droppedSlot == null || droppedSlot == this) return;

        // Merge if same item type and stackable
        if (CanStackWith(droppedSlot.item))
        {
            int total = currentStack + droppedSlot.currentStack;
            int overflow = total - item.MaxStackSize;

            currentStack = Mathf.Min(total, item.MaxStackSize);
            UpdateStackUI();

            if (overflow > 0)
            {
                droppedSlot.currentStack = overflow;
                droppedSlot.UpdateStackUI();
            }
            else
            {
                // fully merged, destroy dropped slot
                Destroy(droppedSlot.gameObject);
            }
        }
    }
    #endregion

    #region Click Logic
    public void clicked()
    {
        if (item.usable)
        {
            item.Use();
            currentStack--;
            UpdateStackUI();

            if (currentStack <= 0)
            {
                for (int y = 0; y < item.itemSize.y; y++)
                    for (int x = 0; x < item.itemSize.x; x++)
                        slots.grid[(int)startPosition.x + x, (int)startPosition.y + y] = 0;

                Destroy(this.gameObject);
            }

            Functionalities descript = FindObjectOfType<Functionalities>();
            descript.changeDescription("", "", 0, "");
        }

        if (item.equipable)
        {
            item.Use();
            Functionalities descript = FindObjectOfType<Functionalities>();
            descript.changeDescription("", "", 0, "");
        }
    }
    #endregion


}
