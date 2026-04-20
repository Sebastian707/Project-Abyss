using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Merchant", menuName = "Merchant/Merchant Data")]
public class MerchantData : ScriptableObject
{
    public string merchantName;

    [Header("Pricing")]
    [Tooltip("Global buy price multiplier. 1.0 = normal, 1.2 = 20% more expensive.")]
    public float priceMultiplier = 1f;

    [Tooltip("Global sell price multiplier applied on top of tag modifiers. 0.5 = player gets 50% back.")]
    [Range(0f, 1f)]
    public float sellMultiplier = 0.5f;

    [Tooltip("Tags that adjust prices per item category for both buying and selling.")]
    public List<MerchantTag> tags = new List<MerchantTag>();

    [Header("Accepted Sell Categories")]
    [Tooltip("Categories this merchant will buy from the player. Leave empty to accept everything.")]
    public List<string> acceptedSellCategories = new List<string>();

    [Header("Stock")]
    public List<MerchantStock> itemsForSale = new List<MerchantStock>();

    // ------------------------------------------------------------------ //

    /// <summary>
    /// Returns true if this merchant will buy this item from the player.
    /// If acceptedSellCategories is empty, the merchant buys anything.
    /// Otherwise the item must have at least one matching category.
    /// </summary>
    public bool WillBuyItem(TetrisItem item)
    {
        if (acceptedSellCategories == null || acceptedSellCategories.Count == 0)
            return true;

        foreach (string accepted in acceptedSellCategories)
            if (item.categories.Contains(accepted))
                return true;

        return false;
    }

    /// <summary>Final buy price after all tag modifiers.</summary>
    public int GetPrice(TetrisItem item)
    {
        float multiplier = priceMultiplier;

        foreach (MerchantTag tag in tags)
        {
            if (tag == null) continue;
            if (string.IsNullOrEmpty(tag.affectedCategory) || item.categories.Contains(tag.affectedCategory))
                multiplier *= tag.modifier;
        }

        return Mathf.Max(0, Mathf.RoundToInt(item.basePrice * multiplier));
    }

    /// <summary>Final sell price — same tag modifiers as buying, scaled by sellMultiplier.</summary>
    public int GetSellPrice(TetrisItem item)
    {
        float multiplier = priceMultiplier * sellMultiplier;

        foreach (MerchantTag tag in tags)
        {
            if (tag == null) continue;
            if (string.IsNullOrEmpty(tag.affectedCategory) || item.categories.Contains(tag.affectedCategory))
                multiplier *= tag.modifier;
        }

        return Mathf.Max(0, Mathf.RoundToInt(item.basePrice * multiplier));
    }
}