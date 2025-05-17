using System.Collections.Generic;
using UnityEngine;

public abstract class State : MonoBehaviour
{
    //[SerializeField] private UnityEvent _onEnter = new();
    //[SerializeField] private UnityEvent _onExit = new();
    [field:SerializeField] public StateEnum Name { get; private set; }
    [SerializeField] private List<State> _canTransitionFrom = new();
    
    protected StateMachine _stateMachine;
    protected CharacterController _characterController;
    
    public virtual void Init(StateMachine stateMachine, CharacterController characterController)
    {
        _stateMachine = stateMachine;
        _characterController = characterController;
    }

    public virtual bool CanEnter(State currentState)
    {
        return _canTransitionFrom.Contains(currentState);
    }
    
    public virtual void OnEnter()
    {
        //_onEnter?.Invoke();
    }
    
    public virtual void OnUpdate(float deltaTime) { }

    public virtual void OnExit()
    {
        //_onExit?.Invoke();
    }
}
