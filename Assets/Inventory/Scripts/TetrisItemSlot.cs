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

    [Header("Rotation Settings")]
    private bool isRotated = false;
    private Vector2 currentItemSize; // Local copy of item size

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
        // Store a local copy of the item size (don't modify ScriptableObject!)
        currentItemSize = item.itemSize;

        // --- Rescale item visuals ---
        RescaleItem();

        slots = FindObjectOfType<TetrisSlot>();

        // Mark occupied grid cells based on item size
        for (int y = 0; y < currentItemSize.y; y++)
        {
            for (int x = 0; x < currentItemSize.x; x++)
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

    void Update()
    {
        // Check for R key press during drag
        if (isDragging && Input.GetKeyDown(KeyCode.R))
        {
            RotateItem();
        }
    }

    void RotateItem()
    {
        // Swap X and Y dimensions locally
        currentItemSize = new Vector2(currentItemSize.y, currentItemSize.x);
        isRotated = !isRotated;

        // Just call RescaleItem, which now handles everything
        RescaleItem();
    }

    void RescaleItem()
    {
        RectTransform rt = GetComponent<RectTransform>();
        float width = currentItemSize.x * size.x;
        float height = currentItemSize.y * size.y;

        // Resize the slot container
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);

        // Resize all children except the icon (we handle separately)
        foreach (RectTransform child in transform)
        {
            if (icon != null && child == icon.rectTransform)
                continue;

            child.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            child.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        }

        // --- ICON HANDLING ---
        if (icon != null)
        {
            RectTransform iconRT = icon.GetComponent<RectTransform>();

            // Always use a centered pivot and no anchors (so it rotates correctly)
            iconRT.anchorMin = new Vector2(0.5f, 0.5f);
            iconRT.anchorMax = new Vector2(0.5f, 0.5f);
            iconRT.pivot = new Vector2(0.5f, 0.5f);
            iconRT.anchoredPosition = Vector2.zero;

            // Apply correct size and rotation based on state
            if (isRotated)
            {
                iconRT.sizeDelta = new Vector2(rt.rect.height, rt.rect.width);
                iconRT.localRotation = Quaternion.Euler(0, 0, 90f);
            }
            else
            {
                iconRT.sizeDelta = new Vector2(rt.rect.width, rt.rect.height);
                iconRT.localRotation = Quaternion.identity;
            }

            iconRT.localScale = Vector3.one;
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
        isDragging = true;
        oldPosition = GetComponent<RectTransform>().anchoredPosition;
        GetComponent<CanvasGroup>().blocksRaycasts = false;

        // 🔥 Move to top of hierarchy (renders above all other UI)
        transform.SetAsLastSibling();

        // Clear grid when starting drag
        for (int y = 0; y < currentItemSize.y; y++)
            for (int x = 0; x < currentItemSize.x; x++)
            {
                int gx = (int)startPosition.x + x;
                int gy = (int)startPosition.y + y;
                if (gx >= 0 && gx < slots.maxGridX && gy >= 0 && gy < slots.maxGridY)
                    slots.grid[gx, gy] = 0;
            }
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        GetComponent<CanvasGroup>().blocksRaycasts = true;

        if (EventSystem.current.IsPointerOverGameObject())
        {
            Vector2 finalPos = GetComponent<RectTransform>().anchoredPosition;
            Vector2 finalSlot;
            finalSlot.x = Mathf.Floor(finalPos.x / size.x);
            finalSlot.y = Mathf.Floor(-finalPos.y / size.y);

            if (((int)finalSlot.x + (int)currentItemSize.x - 1) < slots.maxGridX &&
                ((int)finalSlot.y + (int)currentItemSize.y - 1) < slots.maxGridY &&
                (finalSlot.x >= 0 && finalSlot.y >= 0))
            {
                List<Vector2> newPosItem = new List<Vector2>();
                bool fit = false;

                for (int y = 0; y < currentItemSize.y; y++)
                {
                    for (int x = 0; x < currentItemSize.x; x++)
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
                            RevertRotationAndPosition();
                            newPosItem.Clear();
                            x = (int)currentItemSize.x;
                            y = (int)currentItemSize.y;
                        }
                    }
                }

                if (fit)
                {
                    // Mark new grid
                    foreach (Vector2 pos in newPosItem)
                        slots.grid[(int)pos.x, (int)pos.y] = 1;

                    startPosition = newPosItem[0];
                    transform.GetComponent<RectTransform>().anchoredPosition =
                        new Vector2(startPosition.x * size.x, -startPosition.y * size.y);
                }
                else
                {
                    // Re-mark old grid
                    for (int y = 0; y < currentItemSize.y; y++)
                        for (int x = 0; x < currentItemSize.x; x++)
                        {
                            int gx = (int)startPosition.x + x;
                            int gy = (int)startPosition.y + y;
                            if (gx >= 0 && gx < slots.maxGridX && gy >= 0 && gy < slots.maxGridY)
                                slots.grid[gx, gy] = 1;
                        }
                }
            }
            else
            {
                RevertRotationAndPosition();
            }
        }
        else
        {
            // Reset rotation before dropping outside
            currentItemSize = item.itemSize;
            isRotated = false;
            if (icon != null)
            {
                RectTransform iconRT = icon.GetComponent<RectTransform>();
                iconRT.localRotation = Quaternion.identity;
            }

            DropOutsideInventory();
        }
    }

    void RevertRotationAndPosition()
    {
        // Revert to original rotation if placement failed
        if (isRotated)
        {
            currentItemSize = item.itemSize;
            isRotated = false;
            if (icon != null)
            {
                RectTransform iconRT = icon.GetComponent<RectTransform>();
                iconRT.localRotation = Quaternion.identity;
            }
            RescaleItem();
        }

        transform.GetComponent<RectTransform>().anchoredPosition = oldPosition;

        // Re-mark original grid position
        for (int y = 0; y < currentItemSize.y; y++)
            for (int x = 0; x < currentItemSize.x; x++)
            {
                int gx = (int)startPosition.x + x;
                int gy = (int)startPosition.y + y;
                if (gx >= 0 && gx < slots.maxGridX && gy >= 0 && gy < slots.maxGridY)
                    slots.grid[gx, gy] = 1;
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

                // Clear grid before destroying
                for (int y = 0; y < currentItemSize.y; y++)
                    for (int x = 0; x < currentItemSize.x; x++)
                    {
                        int gx = (int)startPosition.x + x;
                        int gy = (int)startPosition.y + y;
                        if (gx >= 0 && gx < slots.maxGridX && gy >= 0 && gy < slots.maxGridY)
                            slots.grid[gx, gy] = 0;
                    }

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
                // Clear grid before destroying
                for (int y = 0; y < droppedSlot.currentItemSize.y; y++)
                    for (int x = 0; x < droppedSlot.currentItemSize.x; x++)
                    {
                        int gx = (int)droppedSlot.startPosition.x + x;
                        int gy = (int)droppedSlot.startPosition.y + y;
                        if (gx >= 0 && gx < slots.maxGridX && gy >= 0 && gy < slots.maxGridY)
                            slots.grid[gx, gy] = 0;
                    }

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
                for (int y = 0; y < currentItemSize.y; y++)
                    for (int x = 0; x < currentItemSize.x; x++)
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