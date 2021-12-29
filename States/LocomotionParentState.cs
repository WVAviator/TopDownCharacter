using System;
using Animancer.FSM;
using UnityEngine;

namespace TopDownCharacter.States
{
    public class LocomotionParentState : CharacterState
    {
        public StateMachine<CharacterState> StateMachine;
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
        }

        void OnEnable()
        {
            StateMachine.ForceSetState(Idle);
            Character.Controller.ActiveControllerParameters = _stateControllerParameters;
        }

        void OnDisable() => StateMachine.ForceSetState(null);

        void FixedUpdate()
        {
            if (!Character.Motor.GroundingStatus.IsStableOnGround)
            {
                StateMachine.TrySetState(Falling);
            }

            else if (Character.Motor.Velocity.HasValue() || Character.MovementInput.CurrentMovementInput.HasInput)
            {
                if (StateMachine.TrySetState(Sprint)) return;
               StateMachine.TrySetState(Movement);
            }
            
            else StateMachine.TrySetState(Idle);
        }
        


    }
}