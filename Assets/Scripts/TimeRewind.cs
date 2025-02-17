using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeRewind : MonoBehaviour
{
    [System.Serializable]
    public class ObjectState
    {
        public Vector3 position;
        public Quaternion rotation;
        public float timestamp;

        public ObjectState(Vector3 pos, Quaternion rot, float time)
        {
            position = pos;
            rotation = rot;
            timestamp = time;
        }
    }

    private List<ObjectState> stateBuffer = new List<ObjectState>();
    public float rewindDuration = 3f; // How far back in time we can rewind
    public float recordInterval = 0.1f; // Frequency of state recording

    private float recordTimer;
    private PlayerController playerController;

    void Start()
    {
        playerController = GetComponent<PlayerController>();
    }

    void Update()
    {
        // Regularly record state
        recordTimer += Time.deltaTime;
        if (recordTimer >= recordInterval)
        {
            recordTimer = 0f;
            RecordState();
        }

        // Remove states older than rewindDuration
        stateBuffer.RemoveAll(state => Time.time - state.timestamp > rewindDuration);
    }

    void RecordState()
    {
        stateBuffer.Add(new ObjectState(transform.position, transform.rotation, Time.time));
    }

    public void StartRewind()
    {
        if (playerController != null)
        {
            //playerController.DisableMovement();
        }
        StartCoroutine(RewindCoroutine());
    }

    private IEnumerator RewindCoroutine()
    {
        while (stateBuffer.Count > 0)
        {
            // Retrieve the most recent state
            ObjectState lastState = stateBuffer[stateBuffer.Count - 1];
            stateBuffer.RemoveAt(stateBuffer.Count - 1);

            // Restore the object's state
            transform.position = lastState.position;
            transform.rotation = lastState.rotation;

            yield return new WaitForFixedUpdate(); // Sync with physics update
        }

        if (playerController != null)
        {
            //playerController.EnableMovement();
        }
    }
}
