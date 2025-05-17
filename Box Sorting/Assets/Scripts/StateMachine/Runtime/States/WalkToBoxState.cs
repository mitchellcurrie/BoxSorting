
using UnityEngine;

public class WalkToBoxState : State
{
    public override void OnEnter()
    { 
        _characterController.MoveToTarget();
    }

    public override void OnExit()
    {
        _characterController.StopMoving();
    }
}
