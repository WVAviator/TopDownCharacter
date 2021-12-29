using System;
using Animancer.FSM;
using UnityEngine;

namespace TopDownCharacter.States
{
    public class LocomotionParentState : CharacterState
    {
        public StateMachine<CharacterState> StateMachine;
        public StateMachine<CharacterState>.InputBuffer InputBuffer;

        public CharacterState DefaultState;
        
        public IdleState Idle;
        public MovementState Movement;
        public FallingState Falling;
        public SprintState Sprint;

        [SerializeField] ControllerParameters _stateControllerParameters;
        protected override void Awake()
        {
            base.Awake();
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

        void Update() => InputBuffer.Update();

        public bool TrySetDefaultSubState()
        {
            return StateMachine.TrySetState(DefaultState);
        }

        public void ForceSetDefaultState()
        {
            StateMachine.ForceSetState(DefaultState);
        }
    }
}