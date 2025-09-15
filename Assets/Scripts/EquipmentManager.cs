using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    public static EquipmentManager Instance;

    [System.Serializable]
    public struct ItemWeaponPair
    {
        [Tooltip("Drag the TetrisItem ScriptableObject asset here")]
        public TetrisItem item;
        [Tooltip("Drag the corresponding weapon GameObject from the scene here (should be deactivated initially)")]
        public GameObject weapon;
    }

    [Header("Map each inventory item to its scene weapon (drag & drop)")]
    public ItemWeaponPair[] itemWeaponPairs;

    private TetrisItem currentlyEquippedItem;
    private GameObject currentlyActiveWeapon;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("More than one EquipmentManager in the scene!");
            return;
        }

        Instance = this;

        // Ensure ALL mapped weapons are OFF at game load
        DeactivateAllMappedWeapons();
    }

    /// <summary>
    /// Called by inventory items (EquipableTetrisItem.Use) to equip/unequip.
    /// </summary>
    public void ToggleEquip(TetrisItem item)
    {
        if (item == null)
        {
            Debug.LogWarning("ToggleEquip called with null item.");
            return;
        }

        // If clicking the already equipped item -> unequip
        if (currentlyEquippedItem == item)
        {
            Debug.Log($"Unequipped: {item.itemName}");
            currentlyEquippedItem = null;
            DeactivateCurrentWeapon();
            return;
        }

        // Equip new item
        if (currentlyEquippedItem != null)
            Debug.Log($"Replaced: {currentlyEquippedItem.itemName} with {item.itemName}");
        else
            Debug.Log($"Equipped: {item.itemName}");

        DeactivateCurrentWeapon();
        currentlyEquippedItem = item;

        GameObject mappedWeapon = FindWeaponByItem(item);
        if (mappedWeapon != null)
        {
            currentlyActiveWeapon = mappedWeapon;
            currentlyActiveWeapon.SetActive(true);

            Weapon w = currentlyActiveWeapon.GetComponent<Weapon>();
         
        }
        else
        {
            Debug.LogWarning($"No scene weapon mapped for item: {item.itemName}. Player will be empty-handed.");
            currentlyActiveWeapon = null;
        }
    }

    public TetrisItem GetEquippedItem() => currentlyEquippedItem;

    private GameObject FindWeaponByItem(TetrisItem item)
    {
        for (int i = 0; i < itemWeaponPairs.Length; i++)
        {
            if (itemWeaponPairs[i].item == item)
                return itemWeaponPairs[i].weapon;
        }
        return null;
    }

    private void DeactivateCurrentWeapon()
    {
        if (currentlyActiveWeapon != null)
        {
            Weapon w = currentlyActiveWeapon.GetComponent<Weapon>();
        

            currentlyActiveWeapon.SetActive(false);
            currentlyActiveWeapon = null;
        }
    }

    private void DeactivateAllMappedWeapons()
    {
        for (int i = 0; i < itemWeaponPairs.Length; i++)
        {
            if (itemWeaponPairs[i].weapon != null)
            {
                Weapon w = itemWeaponPairs[i].weapon.GetComponent<Weapon>();
    

                itemWeaponPairs[i].weapon.SetActive(false);
            }
        }
        currentlyActiveWeapon = null;
    }
}
