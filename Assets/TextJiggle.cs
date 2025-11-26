using UnityEngine;
using TMPro;

public class TextJiggle : MonoBehaviour
{
    public TMP_Text textComponent;       // Auto-assign
    public float intensity = 1f;         // How strong the shake is
    public float speed = 10f;            // How fast it wiggles

    Vector3 originalPos;

    void Awake()
    {
        if (textComponent == null)
            textComponent = GetComponent<TMP_Text>();

        originalPos = transform.localPosition;
    }

    void Update()
    {
        float offsetX = Mathf.Sin(Time.time * speed) * intensity;
        float offsetY = Mathf.Cos(Time.time * speed * 1.3f) * intensity;

        transform.localPosition = originalPos + new Vector3(offsetX, offsetY, 0);
    }
}
