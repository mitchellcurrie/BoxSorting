using UnityEngine;

public class WalkWithBoxState : BoxScanState
{
    public override void OnEnter()
    {
        base.OnEnter();
        _characterController.MoveToTarget();
    }
    
    protected override void OnBoxFound(RaycastHit2D hitBox)
    {
        if (_characterController.IsBoxBlockingTarget(hitBox.point))
        {
            _stateMachine.TryChangeState(StateName.ThrowBox);
        }
    }
}
