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
    
    [Header("State Machine")]
    [SerializeField] private State _defaultState;
    [SerializeField] private State[] _states;
    
    [Header("Setup")]
    [SerializeField] private Transform _blueBoxParent;
    [SerializeField] private Transform _redBoxParent;
    [SerializeField] private Transform _blueFlag;
    [SerializeField] private Transform _redFlag;
    
    private StateMachine _stateMachine;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private Vector2 _targetPosition;
    private Box _collidedBox;
    private bool _holdingBox;
    private bool _moving;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        ValidateFields();
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

    public void SetMoveTarget(Vector2 targetPosition)
    {
        _targetPosition = targetPosition;
    }

    public void MoveToTarget()
    {
        _spriteRenderer.flipX = Vector2.Dot(_targetPosition - (Vector2)transform.position, Vector2.left) > 0;
        //_spriteRenderer.flipX = _targetDirection == Vector2.left;
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
        
        _collidedBox.transform.SetParent(_collidedBox.Colour == BoxColour.Blue ? _blueBoxParent : _redBoxParent);
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
        if (collision.CompareTag("Box"))
        {
            if (_holdingBox || !collision.TryGetComponent<Box>(out var box))
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
