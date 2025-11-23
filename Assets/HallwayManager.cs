using UnityEngine;
using System.Collections.Generic;

public class HallwayManager : MonoBehaviour
{
    [Header("Prefabs (ordered)")]
    public List<GameObject> hallwayPrefabs;  

    [Header("Runtime / Limits")]
    public int maxActiveHallways = 5;

    // Deterministic control flags (edit/extend these in inspector or by code)
    [Header("Deterministic Conditions (priority order)")]
    public bool completedPuzzle = false;   
    public bool hasKey = false;            
    public bool isNight = false;          
    public bool useSafeHallway = false;  

    private Queue<GameObject> activeHallways = new Queue<GameObject>();

    private Transform previousExitPoint;  
    private DoorLock previousDoor;         

    void Start()
    {
        GameObject firstPrefab = ChooseHallwayDeterministic();

        if (firstPrefab == null)
        {
            Debug.LogError("ChooseHallwayDeterministic returned null. Make sure hallwayPrefabs has entries.");
            return;
        }

        GameObject firstHallway = Instantiate(firstPrefab, Vector3.zero, Quaternion.identity);
        activeHallways.Enqueue(firstHallway);

        Transform entry = firstHallway.transform.Find("EntryPoint");
        Transform exit = firstHallway.transform.Find("ExitPoint");

        if (entry == null || exit == null)
        {
            Debug.LogError("First hallway prefab MUST contain EntryPoint and ExitPoint!");
            return;
        }

        previousExitPoint = exit;

        HallwayExitTrigger exitTrigger = firstHallway.GetComponentInChildren<HallwayExitTrigger>();
        if (exitTrigger != null)
        {
            exitTrigger.hallwayManager = this;
            exitTrigger.exitPoint = exit;
        }
        DoorLock firstDoor = firstHallway.GetComponentInChildren<DoorLock>();
        if (firstDoor != null)
            previousDoor = firstDoor;
    }

    public void SpawnNextHallway(Vector3 _, Quaternion __)
    {
        GameObject prefab = ChooseHallwayDeterministic();
        if (prefab == null)
        {
            Debug.LogError("No valid hallway prefab to spawn. Aborting spawn.");
            return;
        }

        SpawnHallway(prefab);

        if (activeHallways.Count > maxActiveHallways)
        {
            Destroy(activeHallways.Dequeue());
        }
    }

    // ------------------- Deterministic selection -------------------
    // Priority: check flags in order, first matching rule chooses the hallway.
    // Edit priority or add new flags here as needed.
    private GameObject ChooseHallwayDeterministic()
    {
        if (hallwayPrefabs == null || hallwayPrefabs.Count == 0)
        {
            Debug.LogError("hallwayPrefabs is empty. Add at least one prefab in the inspector.");
            return null;
        }

        if (completedPuzzle)
        {
            // Hallway2 (example) -> index 1
            return GetPrefabSafe(1, "completedPuzzle");
        }
        else if (hasKey)
        {
            // Hallway3 -> index 2
            return GetPrefabSafe(2, "hasKey");
        }
        else if (isNight)
        {
            // Hallway4 -> index 3
            return GetPrefabSafe(3, "isNight");
        }
        else if (useSafeHallway)
        {
            // Hallway5 -> index 4
            return GetPrefabSafe(4, "useSafeHallway");
        }
        else
        {
            // Default: Hallway1 -> index 0
            return GetPrefabSafe(0, "default");
        }
    }

    private GameObject GetPrefabSafe(int index, string reason)
    {
        if (index < 0 || index >= hallwayPrefabs.Count)
        {
            Debug.LogWarning($"Requested prefab index {index} for '{reason}' is out of range. Falling back to index 0.");
            return hallwayPrefabs[0];
        }
        if (hallwayPrefabs[index] == null)
        {
            Debug.LogWarning($"Prefab at index {index} for '{reason}' is null. Falling back to index 0.");
            return hallwayPrefabs[0];
        }
        return hallwayPrefabs[index];
    }
    // ---------------------------------------------------------------

    private void SpawnHallway(GameObject prefab)
    {
        GameObject newHallway = Instantiate(prefab);

        Transform entry = newHallway.transform.Find("EntryPoint");
        Transform exit = newHallway.transform.Find("ExitPoint");

        if (entry == null || exit == null)
        {
            Debug.LogError("Hallway prefab missing EntryPoint or ExitPoint!");
            return;
        }

        newHallway.transform.rotation = Quaternion.LookRotation(
            previousExitPoint.forward,
            previousExitPoint.up
        );

        Vector3 entryOffset = entry.position - newHallway.transform.position;
        newHallway.transform.position = previousExitPoint.position - entryOffset;

        activeHallways.Enqueue(newHallway);

        HallwayExitTrigger exitTrigger = newHallway.GetComponentInChildren<HallwayExitTrigger>();
        if (exitTrigger != null)
        {
            exitTrigger.hallwayManager = this;
            exitTrigger.exitPoint = exit;
        }

        HallwayEntryTrigger entryTrigger = newHallway.GetComponentInChildren<HallwayEntryTrigger>();
        if (entryTrigger != null)
        {
            entryTrigger.doorBehind = previousDoor;
        }

        DoorLock exitDoor = newHallway.GetComponentInChildren<DoorLock>();
        if (exitDoor != null)
            previousDoor = exitDoor;

        previousExitPoint = exit;
    }
}
