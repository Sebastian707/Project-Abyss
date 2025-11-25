using UnityEngine;

public class BoolActivatorTrigger : MonoBehaviour
{
    [Header("Name of the HallwayManager bool to activate")]
    public string boolName;

    private BoolActivator boolActivator;

    private void Awake()
    {
        // Automatically find BoolActivator in the scene
        boolActivator = FindObjectOfType<BoolActivator>();

        if (boolActivator == null)
        {
            Debug.LogError("BoolActivator not found in the scene!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && boolActivator != null)
        {
            boolActivator.ActivateBool(boolName);
        }
    }
}
