using UnityEngine;

public class ComputerTerminal : Interactable
{
    [Header("Camera References")]
    public Camera mainCamera;
    public Camera secondaryCamera;

    protected override void Interact()
    {
        base.Interact();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (mainCamera != null && secondaryCamera != null)
        {
            mainCamera.enabled = false;
            secondaryCamera.enabled = true;
        }
        else
        {
            Debug.LogWarning("Cameras are not assigned in the inspector!");
        }
    }

    void Update()
    {
        // If secondary camera is active and Q is pressed, switch back
        if (secondaryCamera != null && secondaryCamera.enabled && Input.GetKeyDown(KeyCode.Q))
        {
            secondaryCamera.enabled = false;
            mainCamera.enabled = true;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}