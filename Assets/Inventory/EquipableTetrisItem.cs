using UnityEngine;

[CreateAssetMenu(fileName = "New Equipable Item", menuName = "Inventory/Tetris/EquipableItem")]
public class EquipableTetrisItem : TetrisItem
{
    [Header("Weapon Link")]
    [Tooltip("Drag and drop the weapon GameObject this item should equip.")]
    public GameObject weaponPrefab;

    public override void Use()
    {
        if (!equipable)
        {
            Debug.Log($"{itemName} is not equipable.");
            return;
        }

        EquipmentManager.Instance.ToggleEquip(this);
    }
}
