using UnityEngine;
using UnityEngine.Video;

public class UnpauseButtonScript : MonoBehaviour
{
    public PlayerController playerController;
    public KeyCode unpauseKey = KeyCode.Escape;
    GameObject pauseScreen;
    private VideoPlayer[] allVideoPlayers;

    private void Awake()
    {
        pauseScreen = GameObject.Find("PauseScreen");
        allVideoPlayers = FindObjectsOfType<VideoPlayer>();
    }

    private void Update()
    {
        if (pauseScreen.activeSelf && Input.GetKeyDown(unpauseKey))
        {
            Unpause();
        }
    }

    public void Unpause()
    {
        Time.timeScale = 1;
        pauseScreen.SetActive(false);
        playerController.EnableMovement();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // resume all video players
        foreach (VideoPlayer vp in allVideoPlayers)
        {
            // only resume if paused
            if (vp.isPaused) vp.Play();
        }
    }
}
