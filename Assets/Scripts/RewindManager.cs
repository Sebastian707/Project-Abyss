using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewindManager : MonoBehaviour
{
    private List<TimeRewind> rewindableObjects = new List<TimeRewind>();

    void Start()
    {
        // Collect all objects in the scene with the TimeRewind component
        rewindableObjects.AddRange(FindObjectsOfType<TimeRewind>());
    }

    public void StartRewind()
    {
        foreach (TimeRewind obj in rewindableObjects)
        {
            obj.StartRewind();
        }
    }
}