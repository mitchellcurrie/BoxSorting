using UnityEngine;

public class WalkWithBoxState : BoxScanState
{
    public override void OnEnter()
    {
        base.OnEnter();
        _npcCharacterController.MoveToTarget();
    }
    
    protected override void OnBoxFound(RaycastHit2D hitBox)
    {
        if (_npcCharacterController.IsBoxBlockingTarget(hitBox.point))
        {
            _stateMachine.TryChangeState(StateName.ThrowBox);
        }
    }
}
