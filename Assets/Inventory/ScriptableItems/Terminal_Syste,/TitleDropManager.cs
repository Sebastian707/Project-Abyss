using UnityEngine;
using TMPro;
using System.Collections;

public class TitleDropManager : MonoBehaviour
{
    public static TitleDropManager Instance;

    public TextMeshProUGUI titleText;
    public float displayTime = 3f;

    public AudioSource audioSource;

    private Coroutine currentRoutine;

    void Awake()
    {
        Instance = this;
        titleText.text = "";
    }

    public void ShowTitle(string newTitle, AudioClip sound = null)
    {
        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
        }

        if (sound != null && audioSource != null)
        {
            audioSource.PlayOneShot(sound);
        }

        currentRoutine = StartCoroutine(ShowTitleRoutine(newTitle));
    }

    IEnumerator ShowTitleRoutine(string newTitle)
    {
        titleText.text = newTitle;
        titleText.alpha = 1;

        yield return new WaitForSeconds(displayTime);

        titleText.text = "";
    }
}