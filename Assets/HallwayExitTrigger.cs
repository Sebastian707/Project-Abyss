using UnityEngine;

public class HallwayExitTrigger : MonoBehaviour
{
    public HallwayManager hallwayManager;
    public Transform exitPoint; // assigned automatically

    private bool used = false;

    private void OnTriggerEnter(Collider other)
    {
        if (used) return;               // prevent duplicate hallway spawning
        if (!other.CompareTag("Player")) return;

        used = true;                    // lock this trigger forever
        GetComponent<Collider>().enabled = false;

        hallwayManager.SpawnNextHallway(exitPoint.position, exitPoint.rotation);
    }
}
