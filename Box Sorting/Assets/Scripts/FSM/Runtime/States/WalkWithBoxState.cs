using FSM.Data;
using UnityEngine;

namespace FSM.Runtime.States
{
    public class WalkWithBoxState : BoxScanState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            _npcCharacterController.MoveToTarget();
        }

        public override void OnExit()
        {
            base.OnExit();
            _npcCharacterController.StopAllMovingAnimations();
        }
    
        protected override void OnBoxFound(RaycastHit2D hitBox)
        {
            base.OnBoxFound(hitBox);
            
            if (_npcCharacterController.IsBoxBlockingTarget(hitBox.point))
            {
                _stateMachine.TryChangeState(StateName.ThrowBox);
            }
        }
    }
}
