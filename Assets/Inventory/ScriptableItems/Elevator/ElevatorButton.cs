using UnityEngine;

public class ElevatorButton : Clickable
{
    public Elevator elevator;
    public int floorNumber;


    protected override void Interact()
    {
        elevator.GoToFloor(floorNumber);
    }

}