using Character;
using FSM.Data;
using FSM.Runtime.States;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FSM.Runtime
{
    public class StateMachine
    {
        public Action<StateName> OnStateChanged;
    
        private readonly Dictionary<StateName, State> _states = new();
        private readonly NpcCharacterController _npcCharacterController;
        private readonly bool _showStateChangeLogs;
        private State _currentState;

        public StateMachine(NpcCharacterController npcCharacterController, bool showStateChangeLogs)
        {
            _showStateChangeLogs = showStateChangeLogs;
            _npcCharacterController = npcCharacterController;
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
            if (_currentState)
            {
                _currentState.OnUpdate(deltaTime);
            }
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
    
        public void ForceChangeState(StateName stateName)
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
            _currentState = newState;
            _currentState.OnEnter();
        }
    }
}
