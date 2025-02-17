using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearanceKey : MonoBehaviour
{
    public int keyLevel = 1; // The clearance level this key provides

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerClearance playerClearance = other.GetComponent<PlayerClearance>();

            if (playerClearance != null && playerClearance.TryIncreaseClearance(keyLevel))
            {
                Destroy(gameObject); // Remove the key from the scene
            }
        }
    }
}
