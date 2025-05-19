namespace FSM.Runtime.States
{
    public class WalkToBoxState : State
    {
        public override void OnEnter()
        { 
            _npcCharacterController.MoveToTarget();
        }

        public override void OnExit()
        {
            _npcCharacterController.StopWalkAnimation();
        }
    }
}
