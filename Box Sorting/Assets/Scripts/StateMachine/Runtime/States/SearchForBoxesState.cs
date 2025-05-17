using System.Collections;
using UnityEngine;

public class SearchForBoxesState : State
{
    [SerializeField] private float _searchesPerSecond = 4f;
    private Coroutine _searchCoroutine;

    public override void OnEnter()
    {
        _searchCoroutine = StartCoroutine(SearchForBoxes());
    }
    
    public override void OnExit()
    {
        if (_searchCoroutine != null)
        {
            StopAllCoroutines();
        }
    }
    
    private IEnumerator SearchForBoxes()
    {
        SearchForBoxes(Vector2.right);
        yield return new WaitForSeconds(1 / _searchesPerSecond);
        SearchForBoxes(Vector2.left);
        yield return new WaitForSeconds(1 / _searchesPerSecond);
        _searchCoroutine = StartCoroutine(SearchForBoxes());
    }
    
    private void SearchForBoxes(Vector2 direction)
    {
        var hit = Physics2D.Raycast(transform.position, direction);
        
        if (hit)
        {
            _characterController.SetMoveTarget(direction, hit.point);
            _stateMachine.TryChangeState(StateEnum.WalkToBox);
        }
    }
}
