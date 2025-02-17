using UnityEngine;

public class PlayerRewindController : MonoBehaviour
{
    private RewindManager rewindManager;

    void Start()
    {
        // Find the RewindManager in the scene
        rewindManager = FindObjectOfType<RewindManager>();
    }

    void Update()
    {
        // Trigger rewind with the "R" key (or any input you prefer)
        if (Input.GetKeyDown(KeyCode.R))
        {
            rewindManager.StartRewind();
        }
    }
}
