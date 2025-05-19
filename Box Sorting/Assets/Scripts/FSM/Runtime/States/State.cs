using Character;
using FSM.Data;
using System.Collections.Generic;
using UnityEngine;

namespace FSM.Runtime.States
{
    public abstract class State : MonoBehaviour
    {
        [field:SerializeField] public StateName Name { get; private set; }
        
        [SerializeField] private List<State> _canTransitionFrom = new();
    
        protected StateMachine _stateMachine;
        protected NpcCharacterController _npcCharacterController;
    
        public virtual void Init(StateMachine stateMachine, NpcCharacterController npcCharacterController)
        {
            _stateMachine = stateMachine;
            _npcCharacterController = npcCharacterController;
        }
        
        public virtual bool CanEnter(State currentState)
        {
            // Can only enter a state if the current state is in the specified list of states it can transition from
            return _canTransitionFrom.Contains(currentState);
        }
    
        public virtual void OnEnter() { }

        public virtual void OnUpdate(float deltaTime) { }

        public virtual void OnExit()  { }
    }
}
