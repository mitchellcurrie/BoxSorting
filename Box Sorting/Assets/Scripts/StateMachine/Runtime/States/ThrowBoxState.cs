using System.Collections;
using UnityEngine;

public class ThrowBoxState : State
{
    [SerializeField] private float _delayAfterThrow = 0.2f;
    
    public override void OnEnter()
    {
       _npcCharacterController.ThrowBox();
       StartCoroutine(DelayedChangeState());
    }

    private IEnumerator DelayedChangeState()
    {
        yield return new WaitForSeconds(_delayAfterThrow);
        _stateMachine.TryChangeState(StateName.SearchForBoxes);
    }
}
