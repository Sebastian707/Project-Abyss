using UnityEngine;

public class InspectInteractable : Interactable
{
    private InspectSystem inspectSystem;

    private void Start()
    {
        // Find the inspect system in the scene (or you can drag it in manually)
        inspectSystem = FindObjectOfType<InspectSystem>();

        if (inspectSystem == null)
            Debug.LogError("No InspectSystem found in scene!");
    }

    protected override void Interact()
    {
        if (inspectSystem == null) return;

        // Start inspecting THIS object
        inspectSystem.StartInspect(transform);
    }
}
