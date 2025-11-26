using System.Collections;
using UnityEngine;
using TMPro;

public class TypewriterPerLine : MonoBehaviour
{
    public TMP_Text textComponent;
    public float lineDelay = 0.5f;     // Delay between lines
    public AudioSource audioSource;

    private string[] lines;

    void Awake()
    {
        if (textComponent == null)
            textComponent = GetComponent<TMP_Text>();
    }

    void OnEnable()
    {
        // Normalize line endings and split into lines
        string rawText = textComponent.text.Replace("\r\n", "\n").Replace("\r", "\n");
        lines = rawText.Split('\n');

        // Clear visible text
        textComponent.text = "";

        // Start typing
        StartCoroutine(TypeLines());
    }

    IEnumerator TypeLines()
    {
        foreach (string line in lines)
        {
            if (!string.IsNullOrEmpty(line)) // Skip empty lines
            {
                if (!string.IsNullOrEmpty(textComponent.text))
                    textComponent.text += "\n"; // Only add \n after first line

                textComponent.text += line;

                if (audioSource != null)
                    audioSource.Play();

                yield return new WaitForSeconds(lineDelay);
            }
        }
    }
}
