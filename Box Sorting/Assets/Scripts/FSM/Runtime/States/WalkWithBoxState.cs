using FSM.Data;
using UnityEngine;

namespace FSM.Runtime.States
{
    public class WalkWithBoxState : BoxScanState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            _npcController.MoveToTarget();
        }

        public override void OnExit()
        {
            base.OnExit();
            _npcController.StopAllMovingAnimations();
        }
    
        protected override void OnBoxFound(RaycastHit2D hitBox)
        {
            base.OnBoxFound(hitBox);
            
            if (_npcController.IsBoxBlockingTarget(hitBox.point))
            {
                _stateMachine.TryChangeState(StateName.ThrowBox);
            }
        }
    }
}
