using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [SerializeField] private float _movementSpeed;
    [SerializeField] private State[] _states;

    private StateMachine _stateMachine;
    private bool _holdingBox;

    private void Awake()
    {
        InitStateMachine();
    }

    private void InitStateMachine()
    {
        _stateMachine = new StateMachine(this);

        foreach (var state in _states)
        {
            _stateMachine.Add(state); // add any other relevant SO data in here
        }
    }

    public void Move(Vector2 direction)
    {
        
    }

    public void StopMoving()
    {
        
    }

    private void PickUpBox()
    {
        if (_holdingBox)
        {
            return;
        }
        
        _holdingBox = true;
    }
    
    private void DropBox()
    {
        if (!_holdingBox)
        {
            return;
        }
        
        _holdingBox = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Box"))
        {
            Destroy(collision.gameObject);
        }
    }
}
