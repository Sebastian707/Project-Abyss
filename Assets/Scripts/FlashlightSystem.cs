using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlashlightSystem : RechargeableSystem
{
    [Header("Flashlight")]
    public Light flashlight;
    public KeyCode toggleKey = KeyCode.F;
    public float batteryDrainRate = 5f;
    public float flickerThresholdPercent = 15f;
    public GameObject flashlightObject;

    private bool isOnRequested = false;
    private float flickerTimer = 0f;
    private float baseIntensity;

    protected override void Awake()
    {
        base.Awake();
        baseIntensity = flashlight.intensity;
        flashlight.enabled = false;
    }
    void Update()
    {
        HandleInput();
        HandleDrain();
        HandleAutoDisable();
        HandleVisuals();
    }

    void HandleAutoDisable()
    {
        void HandleAutoDisable()
        {
            if (!flashlightObject.activeInHierarchy && isOnRequested)
            {
                isOnRequested = false;
                flashlight.enabled = false;

                if (uiText != null)
                    uiText.enabled = false;
            }
        }

    }

    void HandleInput()
    {
        if (flashlightObject.activeInHierarchy && Input.GetKeyDown(toggleKey))
        {
            if (currentPower > 0f || TryConsumeBatteryFromInventory())
                isOnRequested = !isOnRequested;
        }
    }

    void HandleDrain()
    {
        if (isOnRequested && currentPower > 0f)
        {
            currentPower -= batteryDrainRate * Time.deltaTime;
            currentPower = Mathf.Max(0, currentPower);
            if (currentPower == 0f) isOnRequested = false;
            UpdateUI();
        }
    }

    void HandleVisuals()
    {
        flashlight.enabled = isOnRequested;
        if (uiText != null)
            uiText.enabled = flashlightObject.activeInHierarchy;

        if (!isOnRequested) return;

        float threshold = maxPower * (flickerThresholdPercent / 100f);
        if (currentPower <= threshold)
        {
            flickerTimer += Time.deltaTime;
            if (flickerTimer >= Random.Range(0.03f, 0.18f))
            {
                flashlight.intensity = Mathf.Lerp(baseIntensity * 0.2f, baseIntensity, Random.value);
                flickerTimer = 0f;
            }
        }
        else
        {
            flashlight.intensity = baseIntensity;
        }
    }
}
