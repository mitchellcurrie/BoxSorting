using System;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    public StateName CurrentState => _currentState.Name;
    public Action<StateName> OnStateChanged;
    
    private readonly Dictionary<StateName, State> _states = new();
    private readonly CharacterController _characterController;
    private State _currentState;

    public StateMachine(CharacterController characterController)
    {
        _characterController = characterController;
    }
    
    public void Add(State state)
    {
        state.Init(this, _characterController);
        _states.Add(state.Name, state);
    }

    private State GetState(StateName key) //TODO: Do we need this method?
    {
        if (_states.TryGetValue(key, out var state))
        {
            return state;
        }
        
        Debug.LogError($"State not found: {key}");
        return null;
    }

    public bool TryChangeState(StateName stateName)
    {
        var newState = GetState(stateName);
        
        if (!newState)
        {
            Debug.LogError("State not valid");
            return false;
        }

        if (_currentState)
        {
            if (!newState.CanEnter(_currentState))
            {
                Debug.LogError($"Cannot Enter {newState.Name} from {_currentState}"); // TODO: Change to normal log or remove
                return false;
            }
            
            _currentState.OnExit();
        }
        
        OnStateChanged?.Invoke(newState.Name);
        _currentState = newState;
        _currentState.OnEnter();
        return true;
    }

    public void Update(float deltaTime)
    {
        _currentState.OnUpdate(deltaTime);
    }
}
