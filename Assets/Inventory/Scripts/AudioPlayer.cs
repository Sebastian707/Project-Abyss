using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Tetris/AudioItem")]
public class AudioPlayer : TetrisItem
{
    public AudioClip audioClip;
    private AudioSource audioSource;
    public Sprite speakerSprite;

    public override void Use()
    {
        if (audioSource == null)
            audioSource = AudioManager.Source;  

        if (audioSource == null)
        {
            Debug.LogWarning("No AudioSource found in the scene.");
            return;
        }

        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
        else
        {
            audioSource.clip = audioClip;
            audioSource.Play();
        }
        AudioManager.Instance.SetSpeakerIcon(speakerSprite);
    }
}
