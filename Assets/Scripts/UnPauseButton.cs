using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnpauseButtonScript : MonoBehaviour
{
    public PlayerController playerController;
    public KeyCode unpausepauseKey = KeyCode.Escape;
    GameObject pauseScreen;
    private void Awake()
    {
        pauseScreen = GameObject.Find("PauseScreen");
    }

    private void Update()
    {
        if (pauseScreen.activeSelf && Input.GetKeyDown(unpausepauseKey))
        {
            Unpause();
        }
    }
    public void Unpause()
    {
        Time.timeScale = 1;
        pauseScreen.SetActive(false);
        playerController.EnableMovement();
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;

    }

}
