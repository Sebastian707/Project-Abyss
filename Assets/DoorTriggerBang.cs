using UnityEngine;
using System.Collections;

public class DoorTriggerBang : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip bangSound;        // First sound
    public AudioClip afterDelaySound;  // Second sound
    public DoorLock doorObject;      // Door to rotate

    public float delayfirst = 2f;
    public float delaysecond = 2f;

    [Header("Rotation Bump Settings")]
    public float bumpAngle = 8f;       // How many degrees to rotate on Y
    public float bumpTime = 0.08f;     // Time to rotate forward
    public float returnTime = 0.15f;   // Time to rotate back

    private bool triggered = false;

    private void Start()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

      
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        triggered = true;
        StartCoroutine(BangSequence());
    }

    private IEnumerator BangSequence()
    {
        // Play hit sound
        if (bangSound)
            audioSource.PlayOneShot(bangSound);

        // Do the rotation bump
        yield return StartCoroutine(RotateDoorBump());

        // Wait before second sound
        yield return new WaitForSeconds(delayfirst);

        // Second sound
        if (afterDelaySound)
            audioSource.PlayOneShot(afterDelaySound);

        yield return new WaitForSeconds(delaysecond);

        doorObject.UnlockDoor();
        doorObject.ToggleDoor();
    }

    private IEnumerator RotateDoorBump()
    {
        if (doorObject == null) yield break;

        Transform t = doorObject.transform;
        Quaternion originalRot = t.localRotation;
        Quaternion bumpedRot = originalRot * Quaternion.Euler(0f, bumpAngle, 0f);

        // Rotate forward quickly
        float elapsed = 0f;
        while (elapsed < bumpTime)
        {
            elapsed += Time.deltaTime;
            float tValue = elapsed / bumpTime;
            t.localRotation = Quaternion.Lerp(originalRot, bumpedRot, tValue);
            yield return null;
        }

        // Rotate back slower
        elapsed = 0f;
        while (elapsed < returnTime)
        {
            elapsed += Time.deltaTime;
            float tValue = elapsed / returnTime;
            t.localRotation = Quaternion.Lerp(bumpedRot, originalRot, tValue);
            yield return null;
        }

        t.localRotation = originalRot;
    }
}
