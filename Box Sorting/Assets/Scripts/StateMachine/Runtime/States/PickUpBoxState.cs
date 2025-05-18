
public class PickUpBoxState : State
{
    public override void OnEnter()
    {
       _characterController.PickUpBox();
       _characterController.MoveToTarget();
    }
}
