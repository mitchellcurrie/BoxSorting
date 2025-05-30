using FSM.Data;
using UnityEngine;

namespace FSM.Runtime.States
{
    public class SearchForBoxesState : BoxScanState
    {
        [SerializeField] private Vector2 _flipLookDirectionMinMaxSeconds;

        private float _flipTimer;
        private float _currentFlipTime;

        public override void OnEnter()
        {
            base.OnEnter();
            _npcController.StopAllMovingAnimations();
            SetRandomFlipTime();
        }

        public override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);
            _flipTimer += deltaTime;
        
            if (_flipTimer >= _currentFlipTime)
            {
                // At random intervals, flip the character's sprite to appear as though they are searching for boxes
                _npcController.FlipLookDirection();
                SetRandomFlipTime();
                _flipTimer = 0;
            }
        }

        protected override void OnBoxFound(RaycastHit2D hitBox)
        {
            base.OnBoxFound(hitBox);
            _npcController.SetMoveTarget(hitBox.point);
            _stateMachine.TryChangeState(StateName.WalkToBox);
        }
    
        private void SetRandomFlipTime()
        {
            _currentFlipTime = Random.Range(_flipLookDirectionMinMaxSeconds.x, _flipLookDirectionMinMaxSeconds.y);
        }
    }
}
