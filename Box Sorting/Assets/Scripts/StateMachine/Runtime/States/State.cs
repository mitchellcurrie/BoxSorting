using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class State : MonoBehaviour
{
    //protected List<StateEnum> _canTransitionFrom = new();

    //[SerializeField] private UnityEvent _onEnter = new();
    //[SerializeField] private UnityEvent _onExit = new();
    [field:SerializeField] public StateEnum Name { get; private set; }
    
    protected StateMachine _stateMachine;
    protected CharacterController _characterController;
    
    public virtual void Init(StateMachine stateMachine, CharacterController characterController)
    {
        _stateMachine = stateMachine;
        _characterController = characterController;
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
