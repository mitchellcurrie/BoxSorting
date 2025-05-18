using UnityEngine;

public class SearchForBoxesState : State
{
    [SerializeField] private float _searchesPerSecond = 4f;
    [SerializeField] private Vector2 _flipLookDirectionMinMaxSeconds = Vector2.zero;
    private float _searchTimer;
    private float _flipTimer;
    private float _currentFlipTime;
    private Vector2 _searchDirection;

    public override void OnEnter()
    {
        _searchDirection = Random.Range(0, 2) == 1 ? Vector2.left : Vector2.right;
        SetRandomFlipTime();
    }

    public override void OnUpdate(float deltaTime)
    {
        _searchTimer += deltaTime;
        _flipTimer += deltaTime;
        
        if (_searchTimer >= 1 / _searchesPerSecond)
        {
            SearchForBoxes(_searchDirection);
            _searchTimer = 0;
            _searchDirection = _searchDirection == Vector2.left ? Vector2.right : Vector2.left;
        }

        if (_flipTimer >= _currentFlipTime)
        {
            _characterController.FlipLookDirection();
            SetRandomFlipTime();
            _flipTimer = 0;
        }
    }
    
    private void SearchForBoxes(Vector2 direction)
    {
        var hit = Physics2D.Raycast(transform.position, direction);
        
        if (hit && hit.transform.CompareTag("Box"))
        {
            _characterController.SetMoveTarget(hit.point);
            _stateMachine.TryChangeState(StateName.WalkToBox);
        }
    }

    private void SetRandomFlipTime()
    {
        _currentFlipTime = Random.Range(_flipLookDirectionMinMaxSeconds.x, _flipLookDirectionMinMaxSeconds.y);
    }
}
