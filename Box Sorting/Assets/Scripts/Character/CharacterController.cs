using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class CharacterController : MonoBehaviour
{
    private static readonly int WALK_ANIM_BOOL = Animator.StringToHash("Walk");
    private static readonly int HOLD_ANIM_BOOL = Animator.StringToHash("Hold");

    [Header("Settings")] 
    [SerializeField] private float _movementSpeed = 2;
    [SerializeField] private Transform _heldObjectTransform;
    
    [Header("State Machine")]
    [SerializeField] private State _defaultState;
    [SerializeField] private State[] _states;

    private StateMachine _stateMachine;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private Vector2 _targetDirection;
    private Vector2 _targetPosition;
    private GameObject _collidedBox;
    private bool _holdingBox;
    private bool _moving;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        if (!_heldObjectTransform)
        {
            Debug.LogError("Held Object Transform is not set");
        }
        
        InitStateMachine();
    }

    private void InitStateMachine()
    {
        _stateMachine = new StateMachine(this);

        foreach (var state in _states)
        {
            _stateMachine.Add(state); // add any other relevant SO data in here
        }
        
        _stateMachine.TryChangeState(_defaultState.Name);
    }

    private void Update()
    {
        _stateMachine.Update(Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (!_moving)
        {
            return;
        }
        
        transform.position = Vector2.MoveTowards(transform.position, _targetPosition, 
            Time.deltaTime * _movementSpeed);
    }

    public void SetMoveTarget(Vector2 targetDirection, Vector2 targetPosition)
    {
        _targetDirection = targetDirection;
        _targetPosition = targetPosition;
    }

    public void MoveToTarget()
    {
        _spriteRenderer.flipX = _targetDirection == Vector2.left;
        _moving = true;
        _animator.SetBool(WALK_ANIM_BOOL, true);
    }
    
    public void StopMoving()
    {
        _moving = false;
        _animator.SetBool(WALK_ANIM_BOOL, false);
    }

    public void PickUpBox()
    {
        if (_holdingBox) // TODO: Do we need this?
        {
            return;
        }
        
        _collidedBox.transform.SetParent(_heldObjectTransform);
        _collidedBox.transform.SetLocalPositionAndRotation(Vector3.zero, _collidedBox.transform.rotation);
        
        _animator.SetBool(HOLD_ANIM_BOOL, true);
        _holdingBox = true;
    }
    
    public void ThrowBox()
    {
        if (!_holdingBox)
        {
            return;
        }
        
        _animator.SetBool(HOLD_ANIM_BOOL, false);
        _animator.SetBool(WALK_ANIM_BOOL, false);
        _holdingBox = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Box") && !_holdingBox)
        {
            _collidedBox = collision.gameObject;
            _stateMachine.TryChangeState(StateEnum.PickUpBox);
        }
    }
}
