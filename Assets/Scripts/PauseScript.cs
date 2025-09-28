using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseScript : MonoBehaviour
{
    public GameObject pauseScreen;
    public PlayerController playerController;
    public KeyCode pauseKey = KeyCode.Escape;


    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Awake()
    {
        pauseScreen.SetActive(false); 
    }

    void Update()
    {
        if (Input.GetKeyDown(pauseKey))
        {
            Time.timeScale = 0;
            pauseScreen.SetActive(true); 

            playerController.DisableMovement();  

            Cursor.lockState = CursorLockMode.None;  
            Cursor.visible = true; 
        }
    }
}