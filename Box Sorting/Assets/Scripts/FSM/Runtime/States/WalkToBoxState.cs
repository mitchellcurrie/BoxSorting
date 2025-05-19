namespace FSM.Runtime.States
{
    public class WalkToBoxState : State
    {
        public override void OnEnter()
        { 
            base.OnEnter();
            _npcController.MoveToTarget();
        }

        public override void OnExit()
        {
            base.OnExit();
            _npcController.StopWalkAnimation();
        }
    }
}
