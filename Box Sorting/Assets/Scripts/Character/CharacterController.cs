using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class CharacterController : MonoBehaviour
{
    private static readonly int WALK_ANIM_BOOL = Animator.StringToHash("Walk");
    private static readonly int HOLD_ANIM_BOOL = Animator.StringToHash("Hold");

    [Header("Settings")] 
    [SerializeField] private float _movementSpeed = 2;
    [SerializeField] private int _dropBoxForce = 300;
    [SerializeField] private int _throwBoxForce = 400;
    [SerializeField] private int _throwBoxAngleDegrees = 60;
    [SerializeField] private float _blockingBoxDistance = 2f;
    
    [Header("State Machine")]
    [SerializeField] private State _defaultState;
    [SerializeField] private State[] _states;
    
    [Header("Events")]
    [SerializeField] UnityEvent<StateName> _onStateChanged;
    
    [Header("Setup")]
    [SerializeField] private Transform _blueBoxParent;
    [SerializeField] private Transform _redBoxParent;
    [SerializeField] private Transform _blueFlag;
    [SerializeField] private Transform _redFlag;
    
    private StateMachine _stateMachine;
    private StateName _currentState;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private bool _movingLeft;
    private Vector2 _targetPosition;
    private Box _collidedBox;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        ValidateFields();
    }

    private void Start()
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
        
        _stateMachine.OnStateChanged += newState =>
        {
            _currentState = newState;
            _onStateChanged?.Invoke(newState);
        };
        
        _stateMachine.TryChangeState(_defaultState.Name);
    }

    private void Update()
    {
        _stateMachine.Update(Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (_currentState != StateName.WalkToBox && _currentState != StateName.WalkWithBox)
        {
            return;
        }
        
        transform.position = Vector2.MoveTowards(transform.position, _targetPosition, 
            Time.deltaTime * _movementSpeed);
    }

    public void SetMoveTarget(Vector2 targetPosition)
    {
        _targetPosition = new Vector2(targetPosition.x, 0); // Keep character at Y pos 0.
    }

    public void MoveToTarget()
    {
        _movingLeft = Vector2.Dot(_targetPosition - (Vector2)transform.position, Vector2.left) > 0;
        _spriteRenderer.flipX = _movingLeft;
        _animator.SetBool(WALK_ANIM_BOOL, true);
    }

    public void FlipLookDirection()
    {
        _spriteRenderer.flipX = !_spriteRenderer.flipX;
    }
    
    public void StopMoving()
    {
        _animator.SetBool(WALK_ANIM_BOOL, false);
    }

    public bool IsBoxBlockingTarget(Vector2 blockingBoxPosition)
    {
        var targetDirection = _movingLeft ? Vector2.left : Vector2.right;
        var distanceToBlockingBox = Vector2.Distance(blockingBoxPosition, transform.position);
        
        // TODO: Add comment
        return distanceToBlockingBox < _blockingBoxDistance &&
                    Vector2.Dot(blockingBoxPosition - (Vector2)transform.position, targetDirection) > 0;
    }

    public void PickUpBox()
    {
        _collidedBox.transform.SetParent(_collidedBox.Colour == BoxColour.Blue ? _blueBoxParent : _redBoxParent);
        _collidedBox.transform.SetLocalPositionAndRotation(Vector3.zero, _collidedBox.transform.rotation);
        
        _animator.SetBool(HOLD_ANIM_BOOL, true);
    }
    
    public void DropBoxAtTarget()
    {
        var dropDirection = _movingLeft ? Vector2.left : Vector2.right;
        ReleaseBox(dropDirection *_dropBoxForce);
    }
    
    public void ThrowBox()
    {
        var throwDirection = _movingLeft ? Vector2.left : Vector2.right;
        var angle = _movingLeft ? -_throwBoxAngleDegrees : _throwBoxAngleDegrees;
        throwDirection = Quaternion.Euler(0, 0, angle) * throwDirection;
        
        Debug.Log($"Throw Direction: {throwDirection}   --   Angle: {_throwBoxAngleDegrees}");
        
        ReleaseBox(throwDirection * _throwBoxForce);
    }

    private void ReleaseBox(Vector2 releaseForce)
    {
        if (!_collidedBox)
        {
            Debug.LogError("Can't release box as it is null!");
            return;
        }
        
        _collidedBox.transform.SetParent(null);

        var dropDirection = _movingLeft ? Vector2.left : Vector2.right;
        _collidedBox.OnReleased(releaseForce);
        
        _collidedBox = null;
        
        _animator.SetBool(HOLD_ANIM_BOOL, false);
        _animator.SetBool(WALK_ANIM_BOOL, false);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!other.gameObject.CompareTag("Box") ||
            (_currentState != StateName.SearchForBoxes && _currentState != StateName.WalkToBox) 
            || !other.gameObject.TryGetComponent<Box>(out var box))
        {
            return;
        }
        
        _collidedBox = box;
        SetMoveTarget(_collidedBox.Colour == BoxColour.Blue ? _blueFlag.position : _redFlag.position);
            
        if (_stateMachine.TryChangeState(StateName.PickUpBox))
        {
            _collidedBox.OnPickedUp();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        CheckFlagCollision(other);
    }
    
    private void OnTriggerStay2D(Collider2D other)
    {
        CheckFlagCollision(other);
    }

    private void CheckFlagCollision(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Flag") || _currentState != StateName.WalkWithBox)
        {
            return;
        }

        _stateMachine.TryChangeState(StateName.DropBox);
    }
    
    private void ValidateFields()
    {
        if (!_blueBoxParent || !_redBoxParent)
        {
            Debug.LogError("Held Object Transforms are not set");
        }

        if (!_blueFlag || !_redFlag)
        {
            Debug.LogError("Flags are not set");
        }
    }
}
