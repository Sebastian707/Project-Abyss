using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class PlayerStats : MonoBehaviour
{
    public float CurrentHealth;
    public float MaxHealth = 100;
    public TextMeshProUGUI uiText;

public void Awake()
    {

        CurrentHealth = MaxHealth;
        UpdateUI();
    }

    public void Update()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (uiText)
        {
            float health = Mathf.RoundToInt((CurrentHealth / MaxHealth) * 100f);
            uiText.text = health.ToString();

        }
    }

}