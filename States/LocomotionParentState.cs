using System;
using System.Collections.Generic;
using Animancer.FSM;
using UnityEngine;

namespace TopDownCharacter.States
{
    public class LocomotionParentState : StateBehaviour
    {
        public Character Character;
        
        public StateMachine<CharacterState> StateMachine;
        public StateMachine<CharacterState>.InputBuffer InputBuffer;
        public StateMachine<CharacterState>.StateSelector StateSelector;

        public CharacterState DefaultState;

        CharacterState[] _characterStates;

        [SerializeField] ControllerParameters _stateControllerParameters;
        void Awake()
        {
            Character = transform.root.GetComponentInChildren<Character>();

            _characterStates = GetComponents<CharacterState>();
            StateSelector = new StateMachine<CharacterState>.StateSelector();
            
            foreach (CharacterState characterState in _characterStates)
            {
                StateSelector.Add(characterState);
            }
            
            StateMachine = new StateMachine<CharacterState>();
            StateMachine.SetAllowNullStates();
            InputBuffer = new StateMachine<CharacterState>.InputBuffer(StateMachine);
        }

        void OnEnable()
        {
            ForceSetDefaultState();
            Character.Controller.ActiveControllerParameters = _stateControllerParameters;
        }

        void OnDisable() => StateMachine.ForceSetState(null);

        void Update()
        {
            InputBuffer.Update();
            SelectNewState();
        }

        public bool TrySetDefaultSubState()
        {
            return StateMachine.TrySetState(DefaultState);
        }

        public void ForceSetDefaultState()
        {
            StateMachine.ForceSetState(DefaultState);
        }

        public void SelectNewState()
        {
            StateMachine.TrySetState(StateSelector.Values);
        }

        public void Reset()
        {
            StateMachine.TryResetState(StateSelector.Values);
        }
    }
}