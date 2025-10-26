using UnityEngine;
using UnityEngine.Video;

public class UnpauseButtonScript : MonoBehaviour
{
    public PlayerController playerController;
    public KeyCode unpauseKey = KeyCode.Escape;
    GameObject pauseScreen;
    private VideoPlayer[] allVideoPlayers;
    private Rigidbody playerRigidbody;

    private void Awake()
    {
        pauseScreen = GameObject.Find("PauseScreen");
        allVideoPlayers = FindObjectsOfType<VideoPlayer>();
            playerRigidbody = playerController.GetComponent<Rigidbody>();

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
        playerRigidbody.isKinematic = false;
        pauseScreen.SetActive(false);
        playerController.EnableMovement();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        foreach (VideoPlayer vp in allVideoPlayers)
        {
            if (vp.isPaused) vp.Play();
        }
    }
}
