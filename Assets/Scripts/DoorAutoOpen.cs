using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorAutoOpen : MonoBehaviour
{
    public int requiredClearance = 1; // The clearance level required to open the door
    private bool doorOpen;

    private void Start()
    {
        doorOpen = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerClearance playerClearance = other.GetComponent<PlayerClearance>();

            if (playerClearance != null && playerClearance.clearanceLevel >= requiredClearance)
            {
                doorOpen = true;
                this.GetComponent<Animator>().SetBool("IsOpen", doorOpen);
            }
            else
            {
                Debug.Log("Access Denied: Clearance Level Too Low.");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerClearance playerClearance = other.GetComponent<PlayerClearance>();

            if (playerClearance != null && playerClearance.clearanceLevel >= requiredClearance)
            {
                doorOpen = false;
                this.GetComponent<Animator>().SetBool("IsOpen", doorOpen);
            }
        }
    }
}