using FSM.Data;

namespace FSM.Runtime.States
{
    public class PickUpBoxState : State
    {
        public override void OnEnter()
        {
            base.OnEnter();
            _npcCharacterController.PickUpBox();
            _stateMachine.TryChangeState(StateName.WalkWithBox);
        }
    }
}
