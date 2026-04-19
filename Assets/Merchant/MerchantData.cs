using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Merchant", menuName = "Merchant/Merchant Data")]
public class MerchantData : ScriptableObject
{
    public string merchantName;

    [Header("Pricing")]
    [Tooltip("Global price multiplier for this merchant. 1.0 = normal, 1.2 = 20% more expensive.")]
    public float priceMultiplier = 1f;

    [Tooltip("Tags that further adjust prices per item category.")]
    public List<MerchantTag> tags = new List<MerchantTag>();

    [Header("Stock")]
    public List<MerchantStock> itemsForSale = new List<MerchantStock>();

    public int GetPrice(TetrisItem item)
    {
        float multiplier = priceMultiplier;

        foreach (MerchantTag tag in tags)
        {
            if (tag == null) continue;

            bool affectsAll = string.IsNullOrEmpty(tag.affectedCategory);
            bool affectsThisItem = item.categories.Contains(tag.affectedCategory);

            if (affectsAll || affectsThisItem)
                multiplier *= tag.modifier;
        }

        return Mathf.Max(0, Mathf.RoundToInt(item.basePrice * multiplier));
    }
}