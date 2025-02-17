using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerSlide : MonoBehaviour
{
    public float slideSpeed = 15f; // Speed during the slide
    public float slideCooldown = 2f; // Time before the player can slide again
    public float slideFriction = 5f; // Friction to slow the slide gradually
    public float slideHeight = 0.5f; // Height of the character during slide

    public Transform cameraTransform; // Reference to the camera transform
    public float cameraSlideOffset = 0.5f; // How much the camera lowers during slide
    public float cameraSlideSpeed = 5f; // How fast the camera lowers

    private CharacterController characterController;
    private PlayerController playerController; // Reference to PlayerController script
    private bool isSliding = false;
    private bool canSlide = true;
    private Vector3 slideDirection;
    private Vector3 originalCameraPosition;
    private float originalHeight;
    private Vector3 originalCenter;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        playerController = GetComponent<PlayerController>(); // Get reference to PlayerController
        if (cameraTransform != null)
        {
            originalCameraPosition = cameraTransform.localPosition;
        }
        originalHeight = characterController.height;
        originalCenter = characterController.center;
    }

    void Update()
    {
        // Check if the player initiates or stops a slide
        if (Input.GetKeyDown(KeyCode.C) && canSlide) // "C" for slide, change to preferred key
        {
            StartSlide();
        }
        else if (Input.GetKeyUp(KeyCode.C) && isSliding)
        {
            EndSlide();
        }

        // Handle the sliding motion
        if (isSliding)
        {
            // Apply constant movement in the original slide direction
            Vector3 movement = slideDirection * slideSpeed * Time.deltaTime;
            characterController.Move(movement);

            // Smoothly lower the camera
            if (cameraTransform != null)
            {
                Vector3 targetPosition = originalCameraPosition - new Vector3(0, cameraSlideOffset, 0);
                cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, targetPosition, Time.deltaTime * cameraSlideSpeed);
            }
        }
        else if (cameraTransform != null) // Reset camera position when not sliding
        {
            cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, originalCameraPosition, Time.deltaTime * cameraSlideSpeed);
        }
    }

    void StartSlide()
    {
        if (!characterController.isGrounded || isSliding) return;

        // Capture the slide direction based on the player's current forward vector
        slideDirection = transform.forward;

        isSliding = true;
        canSlide = false;

        // Lower the CharacterController to allow sliding under objects
        characterController.height = slideHeight;
        characterController.center = new Vector3(characterController.center.x, slideHeight / 2f, characterController.center.z);

        // Disable movement but allow jumping and camera controls
        if (playerController != null)
        {
            playerController.canMove = false;
        }
    }

    void EndSlide()
    {
        isSliding = false;

        // Restore the CharacterController's original height and center
        characterController.height = originalHeight;
        characterController.center = originalCenter;

        Invoke("ResetSlideCooldown", slideCooldown);

        // Re-enable movement
        if (playerController != null)
        {
            playerController.canMove = true;
        }
    }

    void ResetSlideCooldown()
    {
        canSlide = true;
    }
}
