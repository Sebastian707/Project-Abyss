using System.Collections;
using UnityEngine;
using TMPro;

public class TypewriterWithStableCursor : MonoBehaviour
{
    public TMP_Text textComponent;
    public float typingDelay = 0.05f;
    public float cursorBlinkRate = 0.5f;
    public AudioSource audioSource;

    private string fullText;
    private string typedText = "";
    private bool cursorVisible = true;

    void Awake()
    {
        if (textComponent == null)
            textComponent = GetComponent<TMP_Text>();
    }

    void OnEnable()
    {
        fullText = textComponent.text;
        typedText = "";
        textComponent.text = "";
        StartCoroutine(Type());
        StartCoroutine(CursorBlink());
    }

    IEnumerator Type()
    {
        foreach (char c in fullText)
        {
            typedText += c;

            if (audioSource != null)
                audioSource.Play();

            UpdateVisual();
            yield return new WaitForSeconds(typingDelay);
        }
    }

    IEnumerator CursorBlink()
    {
        while (true)
        {
            cursorVisible = !cursorVisible;
            UpdateVisual();
            yield return new WaitForSeconds(cursorBlinkRate);
        }
    }

    void UpdateVisual()
    {
        // Only builds text in ONE place — no replacement bugs
        string cursor = cursorVisible ? "<alpha=#FF>│"
                                      : "<alpha=#00>│";

        textComponent.text = typedText + cursor;
    }
}
