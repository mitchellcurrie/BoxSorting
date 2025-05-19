using FSM.Data;
using System.Collections;
using UnityEngine;

namespace FSM.Runtime.States
{
    // State for throwing the box over a blocking box
    public class ThrowBoxState : State
    {
        [SerializeField] private float _delayAfterThrow = 0.2f;
    
        public override void OnEnter()
        {
            base.OnEnter();
            _npcCharacterController.ThrowBox();
            StartCoroutine(DelayedChangeState());
        }

        private IEnumerator DelayedChangeState()
        {
            yield return new WaitForSeconds(_delayAfterThrow);
            _stateMachine.TryChangeState(StateName.SearchForBoxes);
        }
    }
}
