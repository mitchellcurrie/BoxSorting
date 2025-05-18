public class PickUpBoxState : State
{
    public override void OnEnter()
    {
       _characterController.PickUpBox();
       _stateMachine.TryChangeState(StateName.WalkWithBox);
    }
}
