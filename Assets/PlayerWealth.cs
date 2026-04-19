using UnityEngine;

/// <summary>
/// Holds the player's currency. Set the starting amount directly in the Inspector.
/// </summary>
public class PlayerWealth : MonoBehaviour
{
    public static PlayerWealth Instance { get; private set; }

    [Header("Currency")]
    public int gold = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public bool CanAfford(int amount) => gold >= amount;

    public bool TrySpend(int amount)
    {
        if (!CanAfford(amount)) return false;
        gold -= amount;
        Debug.Log($"[PlayerWealth] Spent {amount}. Remaining: {gold}");
        return true;
    }

    public void Earn(int amount)
    {
        gold += amount;
        Debug.Log($"[PlayerWealth] Earned {amount}. Total: {gold}");
    }
}
