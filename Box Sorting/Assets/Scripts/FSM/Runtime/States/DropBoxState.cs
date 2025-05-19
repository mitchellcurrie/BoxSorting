using FSM.Data;
using System.Collections;
using UnityEngine;

namespace FSM.Runtime.States
{
    // State for dropping the box into the correct target zone
    public class DropBoxState : State
    {
        [SerializeField] private float _delayAfterDrop = 0.2f;
    
        public override void OnEnter()
        {
            base.OnEnter();
            _npcCharacterController.DropBoxAtTarget();
            StartCoroutine(DelayedChangeState());
        }

        private IEnumerator DelayedChangeState()
        {
            yield return new WaitForSeconds(_delayAfterDrop);
            _stateMachine.TryChangeState(StateName.SearchForBoxes);
        }
    }
}
