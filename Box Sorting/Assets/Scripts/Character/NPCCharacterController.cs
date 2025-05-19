using Box.Data;
using Box.Runtime;
using FSM.Data;
using FSM.Runtime;
using FSM.Runtime.States;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace Character
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class NpcCharacterController : MonoBehaviour
    {
        private static readonly int WALK_ANIM_BOOL = Animator.StringToHash("Walk");
        private static readonly int HOLD_ANIM_BOOL = Animator.StringToHash("Hold");

        [Header("Settings")] 
        [SerializeField] private float _movementSpeed = 2;
        [SerializeField] private float _stuckDistance = 0.1f;
        [SerializeField] private int _dropBoxForce = 300;
        [SerializeField] private int _throwBoxForce = 400;
        [SerializeField] private int _dropBoxAngleDegrees = 45;
        [SerializeField] private int _throwBoxAngleDegrees = 60;
        [SerializeField] private float _blockingBoxDistance = 2f;
        [SerializeField] private float _flagCollisionChecksPerSecond = 5f;
    
        [Header("State Machine")]
        [SerializeField] private State _defaultState;
        [SerializeField] private State[] _states = Array.Empty<State>();

        [Header("Events")] 
        [SerializeField] private UnityEvent<StateName> _onStateChanged = new();
    
        [Header("Debug")]
        [SerializeField] private bool _showDebugStateChangeLogs;
    
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
        private BoxController _collidedBox;
        private float _flagCollisionStayTimer;
        private Collider2D _flagCollision;

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
        
        private void Update()
        {
            _stateMachine.Update(Time.deltaTime);

            if (_flagCollision)
            {
                CheckFlagCollisionTimer();
            }
        }
        
        private void FixedUpdate()
        {
            if (_currentState != StateName.WalkToBox && _currentState != StateName.WalkWithBox)
            {
                return;
            }
        
            // Check if the NPC has reached the target point, which should only happen if the target box has despawned.
            // This prevents them from getting stuck
            if (Mathf.Abs(transform.position.x - _targetPosition.x) < _stuckDistance)
            {
                _stateMachine.TryChangeState(StateName.SearchForBoxes);
            }

            // For when a carried box despawns
            if (_currentState == StateName.WalkWithBox && !_collidedBox.gameObject.activeInHierarchy)
            {
                _stateMachine.TryChangeState(StateName.SearchForBoxes);
            }
        
            transform.position = Vector2.MoveTowards(transform.position, _targetPosition, 
                Time.fixedDeltaTime * _movementSpeed);
        }
    
        private void InitStateMachine()
        {
            _stateMachine = new StateMachine(this, _showDebugStateChangeLogs);

            foreach (var state in _states)
            {
                _stateMachine.AddState(state);
            }
        
            _stateMachine.OnStateChanged += newState =>
            {
                _currentState = newState;
                _onStateChanged?.Invoke(newState);
            };
        
            _stateMachine.ForceChangeState(_defaultState.Name);
        }
        
        private void CheckFlagCollisionTimer()
        {
            // Regular checks if the character is at the flag. This is for situations when the character acquires a box
            // when they are already standing at the flag, and the OnTriggerEnter check has already been completed
            _flagCollisionStayTimer += Time.deltaTime;
            
            if (_flagCollisionStayTimer >= 1 / _flagCollisionChecksPerSecond)
            {
                TryChangeStateAtFlag();
                _flagCollisionStayTimer = 0;
            }
        }
        
        public void SetMoveTarget(Vector2 targetPosition)
        {
            _targetPosition = new Vector2(targetPosition.x, 0); // Always keep character at Y position 0
        }

        public void MoveToTarget()
        {
            _movingLeft = Vector2.Dot(_targetPosition - (Vector2)transform.position, Vector2.left) > 0;
            _spriteRenderer.flipX = _movingLeft;
            _animator.SetBool(WALK_ANIM_BOOL, true);
        }

        public void Reset()
        {
            // Used when the reset button on the HUD is pressed
            _collidedBox = null;
            _targetPosition = Vector2.zero;
            _stateMachine.ForceChangeState(StateName.SearchForBoxes);
            transform.position = Vector2.zero;
        }

        public void FlipLookDirection()
        {
            _spriteRenderer.flipX = !_spriteRenderer.flipX;
        }
    
        public void StopWalkAnimation()
        {
            _animator.SetBool(WALK_ANIM_BOOL, false);
        }

        public void StopAllMovingAnimations()
        {
            _animator.SetBool(HOLD_ANIM_BOOL, false);
            _animator.SetBool(WALK_ANIM_BOOL, false);
        }

        public bool IsBoxBlockingTarget(Vector2 blockingBoxPosition)
        {
            var targetDirection = _movingLeft ? Vector2.left : Vector2.right;
            var distanceToBlockingBox = Vector2.Distance(blockingBoxPosition, transform.position);
        
            // Check if the box is closer than the specified blocking distance, and is in the same direction as
            // the character is currently moving
            return distanceToBlockingBox < _blockingBoxDistance &&
                   Vector2.Dot(blockingBoxPosition - (Vector2)transform.position, targetDirection) > 0;
        }

        public void PickUpBox()
        {
            if (!_collidedBox)
            {
                Debug.LogError("Can't pick up box as it is null");
                return;
            }
            
            // Parent the box to the character, depending on what colour the box is. The character has a transform
            // on each side to hold the carried box, because the sprite is flipped based on the movement direction.
            _collidedBox.transform.SetParent(_collidedBox.Colour == BoxColour.Blue ? _blueBoxParent : _redBoxParent);
            _collidedBox.transform.SetLocalPositionAndRotation(Vector3.zero, _collidedBox.transform.rotation);
        
            _animator.SetBool(HOLD_ANIM_BOOL, true);
        }
    
        public void DropBoxAtTarget()
        {
            // Drop in the direction the character is moving
            var angle = _movingLeft ? -_dropBoxAngleDegrees : _dropBoxAngleDegrees;
            ReleaseBox(angle, _dropBoxForce);
        }
    
        public void ThrowBox()
        {
            // Throw in the direction the character is moving
            var angle = _movingLeft ? -_throwBoxAngleDegrees : _throwBoxAngleDegrees;
            ReleaseBox(angle, _throwBoxForce);
        }

        private void ReleaseBox(float angle, float force)
        {
            if (!_collidedBox)
            {
                Debug.LogError("Can't release box as it is null");
                return;
            }
        
            _collidedBox.SetOriginalParent(); // For hierarchy management purposes
        
            var releaseDirection = _movingLeft ? Vector2.left : Vector2.right;
            releaseDirection = Quaternion.Euler(0, 0, angle) * releaseDirection;
        
            _collidedBox.OnReleased(releaseDirection * force);
            _collidedBox = null;
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            // Check for box collision in a correct state
            if (!other.gameObject.CompareTag("Box") ||
                (_currentState != StateName.SearchForBoxes && _currentState != StateName.WalkToBox) 
                || !other.gameObject.TryGetComponent<BoxController>(out var box))
            {
                return;
            }
        
            _collidedBox = box;
            
            // Set the move target to the correct flag, based on the colour of the box
            SetMoveTarget(_collidedBox.Colour == BoxColour.Blue ? _blueFlag.position : _redFlag.position);
            
            if (_stateMachine.TryChangeState(StateName.PickUpBox))
            {
                _collidedBox.OnPickedUp();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Flag"))
            {
                _flagCollision = other;
                TryChangeStateAtFlag();
            }
        }
    
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Flag"))
            {
                _flagCollision = null;
            }
        }

        private void TryChangeStateAtFlag()
        {
            if (_currentState == StateName.WalkWithBox)
            {
                _stateMachine.TryChangeState(StateName.DropBox);
            }
        }
    
        private void ValidateFields()
        {
            if (!_defaultState)
            {
                Debug.LogError("Default state is not set");
            }
            
            if (_states.Length == 0)
            {
                Debug.LogError("State list is empty");
            }
            
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
}
