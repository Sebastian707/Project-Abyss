using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class PortalPressurePlate : MonoBehaviour
{
    [Header("Plate Settings")]
    public float pressDistance = 0.1f;       // How far the plate moves down
    public float pressSpeed = 2f;            // How fast it moves
    public float pressDelay = 0.1f;          // Optional delay before moving up
    public bool useWeight = false;           // If true, heavier objects press more (optional extension)

    [Header("Debug")]
    public bool debugTrigger = true;

    private Vector3 initialPosition;
    private Vector3 pressedPosition;
    private Rigidbody plateRb;
    private int objectsOnPlate = 0;

    void Awake()
    {
        // Ensure collider is trigger
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;

        // Rigidbody setup
        plateRb = GetComponent<Rigidbody>();
        plateRb.isKinematic = true; // We move it manually
        plateRb.interpolation = RigidbodyInterpolation.Interpolate;

        initialPosition = transform.position;
        pressedPosition = initialPosition - new Vector3(0, pressDistance, 0);
    }

    void OnTriggerEnter(Collider other)
    {
        // Count objects (player or box) on the plate
        objectsOnPlate++;
        if (objectsOnPlate == 1) // Only trigger first time
        {
            StartCoroutine(MovePlate(pressedPosition));
            StartCoroutine(PlateTriggered());
        }
    }

    void OnTriggerExit(Collider other)
    {
        objectsOnPlate--;
        objectsOnPlate = Mathf.Max(objectsOnPlate, 0);
        if (objectsOnPlate == 0)
        {
            StartCoroutine(DelayedMoveUp());
        }
    }

    private IEnumerator DelayedMoveUp()
    {
        yield return new WaitForSeconds(pressDelay);
        if (objectsOnPlate == 0)
        {
            StartCoroutine(MovePlate(initialPosition));
        }
    }

    private IEnumerator MovePlate(Vector3 targetPos)
    {
        while (Vector3.Distance(transform.position, targetPos) > 0.001f)
        {
            plateRb.MovePosition(Vector3.MoveTowards(transform.position, targetPos, pressSpeed * Time.fixedDeltaTime));
            yield return new WaitForFixedUpdate();
        }
        plateRb.MovePosition(targetPos);
    }

    private IEnumerator PlateTriggered()
    {
        if (debugTrigger)
            Debug.Log("Pressure plate triggered!");
        // Your enumerator logic goes here
        yield return null;
    }
}
