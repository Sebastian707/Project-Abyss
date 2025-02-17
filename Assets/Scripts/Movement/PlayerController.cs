using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Camera playerCamera;
    public float walkSpeed = 6f;
    public float sprintSpeed = 12f; 
    public float jumpPower = 7f;
    public float gravity = 10f;

    public float lookSpeed = 2f;
    public float lookXLimit = 45f;

    public AudioClip[] DefaultFootstepClips;
    public AudioClip[] WoodFootstepClips;
    public AudioClip[] MetalFootstepClips;

    [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;

    private CharacterController characterController;
    private AudioSource audioSource;

    public bool canMove = true;
    private bool isMoving;
    private float footstepTimer = 0f;
    public float footstepDelay = 0.5f;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.volume = FootstepAudioVolume;
        audioSource.spatialBlend = 1f;
    }

    void Update()
    {
        if (!canMove) return;

        #region Handles Movement
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        float currentSpeed = walkSpeed;

        // Check if the player is holding the shift key for sprinting
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            currentSpeed = sprintSpeed;  // Set to sprint speed
        }

        float curSpeedX = currentSpeed * Input.GetAxis("Vertical");
        float curSpeedY = currentSpeed * Input.GetAxis("Horizontal");
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        isMoving = (curSpeedX != 0 || curSpeedY != 0) && characterController.isGrounded;
        #endregion

        #region Handles Jumping
        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpPower;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }
        #endregion

        #region Handles Rotation
        characterController.Move(moveDirection * Time.deltaTime);

        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
        #endregion

        #region Handle Footsteps
        if (isMoving)
        {
            footstepTimer += Time.deltaTime;
            if (footstepTimer >= footstepDelay)
            {
                PlayFootstepSound();
                footstepTimer = 0f;
            }
        }
        else
        {
            footstepTimer = 0f;
        }
        #endregion
    }

    public void DisableMovement()
    {
        canMove = false;
    }

    public void EnableMovement()
    {
        canMove = true;
    }

    private void PlayFootstepSound()
    {
        string surfaceTag = GetSurfaceTag();

        AudioClip[] chosenClips = DefaultFootstepClips;

        switch (surfaceTag)
        {
            case "Wood":
                chosenClips = WoodFootstepClips;
                break;
            case "Metal":
                chosenClips = MetalFootstepClips;
                break;
        }

        if (chosenClips.Length > 0)
        {
            int index = Random.Range(0, chosenClips.Length);
            audioSource.PlayOneShot(chosenClips[index], FootstepAudioVolume);
        }
    }

    private string GetSurfaceTag()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 2f))
        {
            return hit.collider.tag;
        }
        return "Default";
    }
}
