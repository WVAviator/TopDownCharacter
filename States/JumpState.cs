using System.Collections.Generic;
using Animancer;
using UnityEngine;

namespace TopDownCharacter.States
{
    public class JumpState : CharacterState
    {

        [SerializeField] List<ClipTransition> _jumpAnimations;

        [Tooltip("How high should the character jump?")]
        [SerializeField] float _jumpHeight = 1f;

        FallingState _fallingState;

        float _jumpForce;

        bool _canExitState;

        public override bool CanEnterState => Character.Motor.GroundingStatus.IsStableOnGround && Character.ActionInput.JumpedThisFrame;
        public override bool CanExitState => _canExitState;

        protected override void LateAwake()
        {
            _jumpAnimations.ForEach(j => j.Events.OnEnd += JumpComplete);
            _jumpForce = Mathf.Sqrt(2f * -Character.Controller.ActiveControllerParameters.Gravity.y * _jumpHeight);
            //Character.ActionInput.Jump += OnJumpInput;
            
            if(!TryGetComponent(out _fallingState)) Log($"No falling state found to follow jump animations.");
        }

        public override float Priority => 7f;

        void OnEnable()
        {
            _canExitState = false;
            Character.Motor.ForceUnground();
            Character.Controller.AddVelocity(Vector3.up * _jumpForce);

            ClipTransition jumpAnimation =
                AnimationSelector.MatchLateralMotion(_jumpAnimations, Character.Motor.Velocity);
            Character.Animancer.Play(jumpAnimation);
        }

        void OnJumpInput()
        {
            Character.SubStateMachine.TrySetState(this);
        }

        void JumpComplete()
        {
            _canExitState = true;
            Character.State.Reset();
        }

    }
}