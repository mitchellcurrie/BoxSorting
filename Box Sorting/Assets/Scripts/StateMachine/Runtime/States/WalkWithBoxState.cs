public class WalkWithBoxState : State
{
    public override void OnEnter()
    {
        _characterController.MoveToTarget();
    }
}
