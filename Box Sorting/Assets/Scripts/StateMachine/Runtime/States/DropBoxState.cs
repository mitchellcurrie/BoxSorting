using System.Collections;
using UnityEngine;

public class DropBoxState : State
{
    [SerializeField] private float _delayAfterDrop = 0.2f;
    
    public override void OnEnter()
    {
       _characterController.DropBox();
       StartCoroutine(DelayedChangeState());
    }

    private IEnumerator DelayedChangeState()
    {
        yield return new WaitForSeconds(_delayAfterDrop);
        _stateMachine.TryChangeState(StateName.SearchForBoxes);
    }
}
