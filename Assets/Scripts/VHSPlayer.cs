using UnityEngine;
using UnityEngine.Video;

public class VHSPlayer : Interactable
{
    [Header("TV Components")]
    public VideoPlayer videoPlayer;   // TV's VideoPlayer component

    private TetrisSlot playerSlot;    // Inventory reference

    void Start()
    {
        playerSlot = FindObjectOfType<TetrisSlot>();
        if (playerSlot == null)
            Debug.LogError("No TetrisSlot found in scene!");

        if (videoPlayer == null)
            Debug.LogError("No VideoPlayer assigned to VHSPlayer!");
    }

    protected override void Interact()
    {
        if (playerSlot == null) return;

        // Look through inventory for a VHS
        foreach (TetrisItemSlot slot in playerSlot.itensInBag)
        {
            if (slot != null && slot.item != null)
            {
                VHS tape = slot.item as VHS;
                if (tape != null)
                {
                    PlayTape(tape);
                    return;
                }
            }
        }

        UITVManager.Instance.ShowMessage("You don't have a VHS tape!");
    }

    private void PlayTape(VHS tape)
    {
        if (videoPlayer == null || tape.videoClip == null)
        {
            Debug.LogWarning("Missing VideoPlayer or VideoClip!");
            return;
        }

        UITVManager.Instance.ShowMessage("Playing VHS: " + tape.tapeID);

        videoPlayer.clip = tape.videoClip;
        videoPlayer.Play();
    }
}
