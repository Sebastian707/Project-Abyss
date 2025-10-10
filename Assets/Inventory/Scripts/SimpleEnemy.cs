using UnityEngine;
using UnityEngine.UI;

public class SimpleEnemy : MonoBehaviour, IDamageable
{
    [Header("Health")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("UI")]
    public Image healthBar; // Assign a UI Image (fill type) in inspector

    void Awake()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    public void ApplyDamage(float amount)
    {
        currentHealth -= amount;
        UpdateHealthUI();

        if (currentHealth <= 0)
            Die();
    }

    void UpdateHealthUI()
    {
        if (healthBar != null)
            healthBar.fillAmount = Mathf.Clamp01(currentHealth / maxHealth);
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
