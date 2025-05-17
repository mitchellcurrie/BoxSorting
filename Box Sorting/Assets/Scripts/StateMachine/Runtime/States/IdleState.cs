using System;
using UnityEngine;

public class IdleState : State
{
    public override void OnUpdate(float deltaTime)
    {
        SearchForBoxes(); // TODO: Not every frame
    }
    
    private void SearchForBoxes()
    {
        // Look Right
        if (Raycast(Vector2.right, out RaycastHit2D hitRight))
        {
            _characterController.MoveTo(Vector2.right, hitRight.point);
            _stateMachine.TryChangeState(StateEnum.WalkToBox);
        }
        
        // Look Left
        if (Raycast(Vector2.left, out RaycastHit2D hitLeft))
        {
            _characterController.MoveTo(Vector2.left,hitLeft.point);
            _stateMachine.TryChangeState(StateEnum.WalkToBox);
        }
    }

    private bool Raycast(Vector2 direction, out RaycastHit2D hit)
    {
        //hit = Vector2.zero;
        //RaycastHit2D[] results = Array.Empty<RaycastHit2D>();
        //ContactFilter2D contactFilter = new ContactFilter2D(); // TODO add parameters here?
        //Physics2D.Raycast(_characterController.transform.position, Vector2.left, contactFilter, results);

        hit = Physics2D.Raycast(transform.position, direction);//, Mathf.Infinity, 0);
        return hit;
    }
}
