using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    private Dictionary<StateEnum, State> _states = new();
    private State _currentState;
    private CharacterController _characterController;

    public StateMachine(CharacterController characterController)
    {
        _characterController = characterController;
    }
    
    public void Add(State state)
    {
        state.Init(this, _characterController);
        _states.Add(state.Name, state);
    }

    public State GetState(StateEnum key)
    {
        if (_states.TryGetValue(key, out var state))
        {
            return state;
        }
        
        Debug.LogError("State not found: " + key);
        return null;
    }

    public void TryChangeState(State state)
    {
        // Check if you can change state
        
        if (_currentState != null)
        {
            _currentState.OnExit();
        }

        _currentState = state;

        if (_currentState != null)
        {
            _currentState.OnEnter();
        }
    }

    public void Update()
    {
        _currentState.OnUpdate(Time.deltaTime);
    }
}
