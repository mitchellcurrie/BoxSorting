using UnityEngine;

namespace FSM.Runtime.States
{
    // Base class for states that need to know about surrounding boxes
    public abstract class BoxScanState : State
    {
        [SerializeField] protected float _scansPerSecond = 4f;
    
        private float _scanTimer;
        private Vector2 _scanDirection;

        public override void OnEnter()
        {
            base.OnEnter();
            _scanDirection = Random.Range(0, 2) == 1 ? Vector2.left : Vector2.right;
        }

        public override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);
            _scanTimer += deltaTime;
        
            if (_scanTimer >= 1 / _scansPerSecond)
            {
                ScanForBoxes(_scanDirection);
                _scanTimer = 0;
                
                // Alternate scanning between left and right
                _scanDirection = _scanDirection == Vector2.left ? Vector2.right : Vector2.left;
            }
        }

        protected virtual void OnBoxFound(RaycastHit2D hitBox) { }
        
        private void ScanForBoxes(Vector2 direction)
        {
            var hit = Physics2D.Raycast(transform.position, direction);
        
            if (hit && hit.transform.CompareTag("Box"))
            {
                OnBoxFound(hit);
            }
        }
    }
}


