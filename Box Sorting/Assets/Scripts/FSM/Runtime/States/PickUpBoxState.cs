using FSM.Data;

namespace FSM.Runtime.States
{
    public class PickUpBoxState : State
    {
        public override void OnEnter()
        {
            _npcCharacterController.PickUpBox();
            _stateMachine.TryChangeState(StateName.WalkWithBox);
        }
    }
}
