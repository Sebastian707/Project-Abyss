using UnityEngine;


[CreateAssetMenu(fileName = "New Tag", menuName = "Merchant/Merchant Tag")]
public class MerchantTag : ScriptableObject
{
    public string tagName;         

    [TextArea]
    public string description;     

    [Range(0.0f, 5f)]
    public float modifier = 1f;

    [Tooltip("Leave empty to affect all items. Set to a category to only affect matching items.")]
    public string affectedCategory;
}
