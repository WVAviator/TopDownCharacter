using System.Collections;
using System.Collections.Generic;
using Animancer;
using Animancer.FSM;
using KinematicCharacterController;
using TopDownCharacter.States;
using UnityEngine;

namespace TopDownCharacter
{
    public class Character : MonoBehaviour
    {
        [HideInInspector] public AnimancerComponent Animancer;
        [HideInInspector] public KinematicCharacterMotor Motor;

        [HideInInspector]
        public RootMotionRedirect RootMotionRedirect
        {
            get
            {
                if (_rootMotionRedirect == null) _rootMotionRedirect = GetComponentInChildren<RootMotionRedirect>();
                return _rootMotionRedirect;
            }
        }
        RootMotionRedirect _rootMotionRedirect;
        
        [HideInInspector] public IMovementInput MovementInput;
        [HideInInspector] public IActionInput ActionInput;
        [HideInInspector] public TopDownController Controller;

        [HideInInspector] public StateMachine<LocomotionParentState> ParentStateMachine;
        public LocomotionParentState BasicLocomotion;

        public StateMachine<CharacterState> SubStateMachine => ParentStateMachine.CurrentState.StateMachine;

        public StateMachine<CharacterState>.InputBuffer SubStateMachineBuffer =>
            ParentStateMachine.CurrentState.InputBuffer;

        void Awake()
        {
            Animancer = GetComponentInChildren<AnimancerComponent>();
            Motor = GetComponentInChildren<KinematicCharacterMotor>();
            _rootMotionRedirect = GetComponentInChildren<RootMotionRedirect>();
            MovementInput = GetComponentInChildren<IMovementInput>();
            ActionInput = GetComponentInChildren<IActionInput>();
            Controller = GetComponentInChildren<TopDownController>();
        }

        void Start()
        {
            ParentStateMachine = new StateMachine<LocomotionParentState>(BasicLocomotion);
        }
    }
}
