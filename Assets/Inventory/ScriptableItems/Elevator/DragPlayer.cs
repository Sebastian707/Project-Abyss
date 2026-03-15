using UnityEngine;

public class DragPlayer : MonoBehaviour
{
    private Transform player;
    private Vector3 lastPlatformPosition;
    private bool playerOnPlatform = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.transform;
            playerOnPlatform = true;

            lastPlatformPosition = transform.position;

            Debug.Log("Player entered platform");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerOnPlatform = false;
            player = null;

            Debug.Log("Player exited platform");
        }
    }

    private void LateUpdate()
    {
        if (playerOnPlatform && player != null)
        {
            // Calculate platform movement delta
            Vector3 platformDelta = transform.position - lastPlatformPosition;

            // Only move along Z-axis
            platformDelta.x = 0;
            platformDelta.y = 0;

            player.position += platformDelta;

            // Update last frame position
            lastPlatformPosition = transform.position;
        }
    }
}