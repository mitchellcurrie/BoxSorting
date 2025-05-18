using UnityEngine;

public class SearchForBoxesState : State
{
    [SerializeField] private float _searchesPerSecond = 4f;
    private float _searchTimer;
    private Vector2 _searchDirection;

    public override void OnEnter()
    {
        _searchDirection = Random.Range(0, 2) == 1 ? Vector2.left : Vector2.right;
    }

    public override void OnUpdate(float deltaTime)
    {
        _searchTimer += deltaTime;
        
        if (_searchTimer >= 1 / _searchesPerSecond)
        {
            SearchForBoxes(_searchDirection);
            _searchTimer = 0;
            _searchDirection = _searchDirection == Vector2.left ? Vector2.right : Vector2.left;
        }
    }
    
    // public override void OnExit()
    // {
    //     if (_searchCoroutine != null)
    //     {
    //         StopAllCoroutines();
    //     }
    // }
    //
    // private IEnumerator SearchForBoxes()
    // {
    //     SearchForBoxes(Vector2.right);
    //     yield return new WaitForSeconds(1 / _searchesPerSecond);
    //     SearchForBoxes(Vector2.left);
    //     yield return new WaitForSeconds(1 / _searchesPerSecond);
    //     _searchCoroutine = StartCoroutine(SearchForBoxes());
    // }
    
    private void SearchForBoxes(Vector2 direction)
    {
        var hit = Physics2D.Raycast(transform.position, direction);
        
        if (hit && hit.transform.CompareTag("Box"))
        {
            _characterController.SetMoveTarget(hit.point);
            _stateMachine.TryChangeState(StateName.WalkToBox);
        }
    }
}
