using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Tetris/Item")]
public class TetrisItem : ScriptableObject
{
    public string itemID;
    public Sprite itemIcon;
    public string itemName;
    public string itemDescription;
    public bool usable;
    public bool equipable;
    public int MaxStackSize;
    public int amountOnPickup = 1;
    public Vector2 itemSize;
    public string rarity;

    [SerializeField]
    protected int att1;
    [SerializeField]
    protected Sprite att1_icon;

    public int getAtt1() => att1;
    public Sprite getAtt1Icon() => att1_icon;

    public virtual void Use()
    {
        Debug.Log("Using 1 " + itemName);
    }
}
