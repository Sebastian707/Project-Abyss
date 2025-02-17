using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalTeleporter : MonoBehaviour
{
    public Transform player;
    public Transform reciever;
    private CharacterController characterController;
    private bool playerIsOverlapping = false;
    private Vector3 teleportOffset;

    void Start()
    {
        characterController = player.GetComponent<CharacterController>();
    }

    void Update()
    {
        if (playerIsOverlapping)
        {
            Vector3 portalToPlayer = player.position - transform.position;
            float dotProduct = Vector3.Dot(transform.up, portalToPlayer);

            // Check if the player has moved across the portal
            if (dotProduct < 0f)
            {
                float rotationDiff = -Quaternion.Angle(transform.rotation, reciever.rotation);
                rotationDiff += 180;
                player.Rotate(Vector3.up, rotationDiff);

                Vector3 positionOffset = Quaternion.Euler(0f, rotationDiff, 0f) * portalToPlayer;
                Vector3 newPosition = reciever.position + positionOffset;

                // Calculate the required movement offset
                teleportOffset = newPosition - player.position;

                // Apply instant teleportation without stutter
                StartCoroutine(TeleportPlayer());

                playerIsOverlapping = false;
            }
        }
    }

    IEnumerator TeleportPlayer()
    {
        yield return new WaitForEndOfFrame(); // Wait for physics update
        characterController.enabled = false;  // Temporarily disable physics to prevent issues
        player.position += teleportOffset;    // Instantly move the player
        characterController.enabled = true;   // Re-enable physics
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsOverlapping = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsOverlapping = false;
        }
    }
}
