using UnityEngine;

public class CallElevatorButton : Clickable
{
    public Elevator elevator;      // Drag your Elevator object here
    public int floorIndex;         // Floor where this button is located

    // Call this from a UI button or trigger
    protected override void Interact()
    {
        // Move elevator to this floor
        elevator.GoToFloor(floorIndex);
    }
}