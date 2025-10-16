using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public static AudioSource Source;

    public GameObject PauseScreen;      // Assign in Inspector
    public GameObject audioPanel;       // Assign in Inspector
    public Image speakerIcon;           // UI speaker image in your canvas

    private bool wasPlayingBeforePause = false;

    private void Awake()
    {
        Instance = this;

        Source = GetComponent<AudioSource>();
        if (Source == null)
            Source = gameObject.AddComponent<AudioSource>();

        if (audioPanel != null)
            audioPanel.SetActive(false);
    }

    private void Update()
    {
        // If Paper UI is open, ignore
        if (PaperUIManager.PaperIsOpen) return;

        // Pause handling
        if (PauseScreen != null && PauseScreen.activeSelf)
        {
            if (Source != null && Source.isPlaying)
            {
                Source.Pause(); // Pause the audio
                wasPlayingBeforePause = true;
            }

      

            return; // Skip normal panel/audio logic while paused
        }
        else
        {
            // Resume if it was playing before pause
            if (wasPlayingBeforePause && Source != null)
            {
                Source.UnPause();
                wasPlayingBeforePause = false;
            }
        }

        // Normal audio panel logic
        if (Source != null && Source.isPlaying)
        {
            if (!audioPanel.activeSelf)
                audioPanel.SetActive(true);
        }
        else
        {
            if (audioPanel.activeSelf)
                audioPanel.SetActive(false);
        }
    }

    public void SetSpeakerIcon(Sprite icon)
    {
        if (speakerIcon != null && icon != null)
            speakerIcon.sprite = icon;
    }
}
