using UnityEngine;

public class SearchForBoxesState : BoxScanState
{
    [SerializeField] private Vector2 _flipLookDirectionMinMaxSeconds = Vector2.zero;

    private float _flipTimer;
    private float _currentFlipTime;

    public override void OnEnter()
    {
        base.OnEnter();
        SetRandomFlipTime();
    }

    public override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);
        
        _flipTimer += deltaTime;
        
        if (_flipTimer >= _currentFlipTime)
        {
            _characterController.FlipLookDirection();
            SetRandomFlipTime();
            _flipTimer = 0;
        }
    }

    protected override void OnBoxFound(RaycastHit2D hitBox)
    {
        _characterController.SetMoveTarget(hitBox.point);
        _stateMachine.TryChangeState(StateName.WalkToBox);
    }
    
    private void SetRandomFlipTime()
    {
        _currentFlipTime = Random.Range(_flipLookDirectionMinMaxSeconds.x, _flipLookDirectionMinMaxSeconds.y);
    }
}
