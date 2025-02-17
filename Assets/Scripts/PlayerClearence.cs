using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClearance : MonoBehaviour
{
    public int clearanceLevel = 0; // The player's current clearance level

    public bool TryIncreaseClearance(int keyLevel)
    {
        if (keyLevel == clearanceLevel + 1) // Only allow picking up the next level of clearance
        {
            clearanceLevel = keyLevel;
            Debug.Log("Clearance Level Increased to: " + clearanceLevel);
            return true; // Successfully increased clearance
        }

        Debug.Log("Cannot pick up this clearance key. Current Level: " + clearanceLevel + ", Key Level: " + keyLevel);
        return false; // Clearance not increased
    }
}