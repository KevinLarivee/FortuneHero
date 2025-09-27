using UnityEngine;

public interface IInteractable
{
    public float exitTime { get; set; }
    public void Enter();
    public void Exit();
    public void Interact();
}
