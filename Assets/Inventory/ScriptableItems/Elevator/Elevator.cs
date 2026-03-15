using System.Collections;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    public Transform[] floors;      // Assign floor positions in Inspector
    public float moveSpeed = 2f;    // Elevator movement speed
    public float waitTime = 1f;     // Delay before moving

    [Header("Doors")]
    public Transform leftDoor;      // Left door transform
    public Transform rightDoor;     // Right door transform
    public float doorSpeed = 2f;    // Speed at which doors open/close
    public float doorOpenDistance = 1f; // How far doors move when open

    [Header("Audio")]
    public AudioSource doorAudio;   // Assign an AudioSource with door sound

    private bool isMoving = false;
    private int currentFloor = 0;

    public void GoToFloor(int floorIndex)
    {
        if (isMoving) return;
        if (floorIndex < 0 || floorIndex >= floors.Length) return;
        if (floorIndex == currentFloor) return;

        StartCoroutine(MoveElevatorWithDoors(floorIndex));
    }

    IEnumerator MoveElevatorWithDoors(int targetFloor)
    {
        isMoving = true;

        // 1. Close doors before moving
        yield return StartCoroutine(CloseDoors());

        yield return new WaitForSeconds(0.1f);

        // 2. Move elevator
        Vector3 targetPosition = floors[targetFloor].position;
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            );

            yield return null;
        }
        transform.position = targetPosition;
        currentFloor = targetFloor;

        // 3. Open doors after reaching floor
        yield return StartCoroutine(OpenDoors());

        isMoving = false;
    }

    IEnumerator OpenDoors()
    {
        // Play door sound at the start of opening
        if (doorAudio != null)
        {
            doorAudio.Play();
        }

        Vector3 leftTarget = leftDoor.position + Vector3.back * doorOpenDistance;
        Vector3 rightTarget = rightDoor.position + Vector3.forward * doorOpenDistance;

        while (Vector3.Distance(leftDoor.position, leftTarget) > 0.01f)
        {
            leftDoor.position = Vector3.MoveTowards(leftDoor.position, leftTarget, doorSpeed * Time.deltaTime);
            rightDoor.position = Vector3.MoveTowards(rightDoor.position, rightTarget, doorSpeed * Time.deltaTime);
            yield return null;
        }
    }

    IEnumerator CloseDoors()
    {
        Vector3 leftTarget = leftDoor.position + Vector3.forward * doorOpenDistance;
        Vector3 rightTarget = rightDoor.position + Vector3.back * doorOpenDistance;

        while (Vector3.Distance(leftDoor.position, leftTarget) > 0.01f)
        {
            leftDoor.position = Vector3.MoveTowards(leftDoor.position, leftTarget, doorSpeed * Time.deltaTime);
            rightDoor.position = Vector3.MoveTowards(rightDoor.position, rightTarget, doorSpeed * Time.deltaTime);
            yield return null;
        }
    }
}