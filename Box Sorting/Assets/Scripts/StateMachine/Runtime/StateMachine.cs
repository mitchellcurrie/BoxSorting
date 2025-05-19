using System;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    public StateName CurrentState => _currentState.Name; 
    public StateName PreviousState => _previousState.Name;
    public Action<StateName> OnStateChanged;
    
    private readonly Dictionary<StateName, State> _states = new();
    private readonly NPCCharacterController _npcCharacterController;
    private State _currentState;
    private State _previousState;
    private bool _showStateChangeLogs;

    public StateMachine(NPCCharacterController npcCharacterController, bool showStateChangeLogs)
    {
        _npcCharacterController = npcCharacterController;
        _showStateChangeLogs = showStateChangeLogs;
    }
    
    public void Add(State state)
    {
        state.Init(this, _npcCharacterController);
        _states.Add(state.Name, state);
    }

    private State GetState(StateName key)
    {
        if (_states.TryGetValue(key, out var state))
        {
            return state;
        }
        
        Debug.LogError($"State not found: {key}");
        return null;
    }
    
    public void Update(float deltaTime)
    {
        _currentState.OnUpdate(deltaTime);
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
                Debug.Log($"Cannot Enter {newState.Name} from {_currentState}");
                return false;
            }
            
            _currentState.OnExit();
        }

        if (_showStateChangeLogs)
        {
            Debug.Log($"Change state to <color=green>{newState.Name}</color> from <color=red>{(_currentState ? _currentState.Name : "None")}</color>"); 
        }
        
        SetNewState(newState);
        return true;
    }
    
    public void ForceChageState(StateName stateName)
    {
        var newState = GetState(stateName);
        
        if (!newState)
        {
            Debug.LogError("State not valid");
            return;
        }

        if (_currentState)
        {
            _currentState.OnExit();
        }

        SetNewState(newState);
    }

    private void SetNewState(State newState)
    {
        OnStateChanged?.Invoke(newState.Name);
        _previousState = _currentState;
        _currentState = newState;
        _currentState.OnEnter();
    }
}
