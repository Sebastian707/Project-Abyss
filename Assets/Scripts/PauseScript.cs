using UnityEngine;
using UnityEngine.Video; // required for VideoPlayer

public class PauseScript : MonoBehaviour
{
    public GameObject pauseScreen;
    public PlayerController playerController;
    public KeyCode pauseKey = KeyCode.Escape;
    private Rigidbody playerRigidbody;


    private VideoPlayer[] allVideoPlayers;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // find all VideoPlayers in the scene (or cache specific ones if you want)
        allVideoPlayers = FindObjectsOfType<VideoPlayer>();
        playerRigidbody = playerController.GetComponent<Rigidbody>();
    }

    private void Awake()
    {
        pauseScreen.SetActive(false);
    }

    void Update()
    {
        if (PaperUIManager.PaperIsOpen) return;
        if (Time.timeScale == 0) return;
        if (Input.GetKeyDown(pauseKey))
        {
            playerRigidbody.isKinematic = true;
            Time.timeScale = 0;
            pauseScreen.SetActive(true);
            playerController.DisableMovement();

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // pause all video players
            foreach (VideoPlayer vp in allVideoPlayers)
            {
                if (vp.isPlaying) vp.Pause();
            }
        }
    }
}
