namespace FSM.Runtime.States
{
    public class WalkToBoxState : State
    {
        public override void OnEnter()
        { 
            base.OnEnter();
            _npcCharacterController.MoveToTarget();
        }

        public override void OnExit()
        {
            base.OnExit();
            _npcCharacterController.StopWalkAnimation();
        }
    }
}
