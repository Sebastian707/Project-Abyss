using UnityEngine;
using System.Reflection;

public class BoolActivator : MonoBehaviour
{
    [Header("Reference to HallwayManager")]
    public HallwayManager hallwayManager;

    // Call this function to set any public bool inside HallwayManager to true
    public void ActivateBool(string boolName)
    {
        if (hallwayManager == null)
        {
            Debug.LogError("BoolActivator: HallwayManager reference is missing.");
            return;
        }

        // Get the bool field by name
        FieldInfo fieldInfo = typeof(HallwayManager).GetField(
            boolName,
            BindingFlags.Public | BindingFlags.Instance
        );

        if (fieldInfo == null)
        {
            Debug.LogError($"BoolActivator: No public bool named '{boolName}' found in HallwayManager.");
            return;
        }

        if (fieldInfo.FieldType != typeof(bool))
        {
            Debug.LogError($"BoolActivator: Field '{boolName}' exists but is not a bool.");
            return;
        }

        // Set the bool to true
        fieldInfo.SetValue(hallwayManager, true);

        Debug.Log($"BoolActivator: Set '{boolName}' to TRUE on HallwayManager.");
    }
}
