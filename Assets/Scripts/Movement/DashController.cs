using UnityEngine;
using TMPro;

public class PlayerDash : MonoBehaviour
{
    public float dashDistance = 5f;
    public float dashDuration = 0.5f;
    public float dashCooldown = 2f;
    public AudioClip dashSound;

    public int maxDashCharges = 3;
    public float chargeRestoreRate = 1f;
    private int currentDashCharges;

    private bool isDashing = false;
    private float dashStartTime;
    private CharacterController characterController;
    private AudioSource audioSource;

    public TextMeshProUGUI dashText;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();

        currentDashCharges = maxDashCharges;
    }

    void Update()
    {
        Vector3 moveDirection = GetMovementDirection();

        if (currentDashCharges > 0 && Input.GetKeyDown(KeyCode.LeftShift) && !isDashing)
        {
            Dash(moveDirection);
        }

        if (!isDashing && currentDashCharges < maxDashCharges)
        {
            if (Time.time >= dashStartTime + chargeRestoreRate)
            {
                currentDashCharges++;
                dashStartTime = Time.time;
            }
        }

        if (dashText != null)
        {
            dashText.text = "Dashes: " + currentDashCharges + " / " + maxDashCharges;
        }
    }

    Vector3 GetMovementDirection()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(horizontal, 0f, vertical);

        if (moveDirection.magnitude > 0.1f)
        {
            moveDirection = transform.TransformDirection(moveDirection).normalized;
        }
        else
        {
            moveDirection = transform.forward;
        }

        return moveDirection;
    }

    void Dash(Vector3 dashDirection)
    {
        if (audioSource != null && dashSound != null)
        {
            audioSource.PlayOneShot(dashSound);
        }

        isDashing = true;
        currentDashCharges--;
        characterController.Move(dashDirection * dashDistance);

        Invoke("EndDash", dashDuration);
    }

    void EndDash()
    {
        isDashing = false;
    }
}