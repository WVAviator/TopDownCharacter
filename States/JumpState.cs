using Animancer;
using UnityEngine;

namespace TopDownCharacter.States
{
    public class JumpState : CharacterState
    {

        [SerializeField] ClipTransition _jumpAnimation;

        [Tooltip("How high should the character jump?")]
        [SerializeField] float _jumpHeight = 1f;

        FallingState _fallingState;

        float _jumpForce;

        bool _canExitState;

        public override bool CanEnterState => Character.Motor.GroundingStatus.IsStableOnGround;
        public override bool CanExitState => _canExitState;

        protected override void LateAwake()
        {
            _jumpAnimation.Events.OnEnd += JumpComplete;
            _jumpForce = Mathf.Sqrt(2f * -Character.Controller.ActiveControllerParameters.Gravity.y * _jumpHeight);
            Character.ActionInput.Jump += OnJumpInput;
            
            if(!TryGetComponent(out _fallingState)) Log($"No falling state found to follow jump animations.");
        }

        void OnEnable()
        {
            _canExitState = false;
            Character.Motor.ForceUnground();
            Character.Controller.AddVelocity(Vector3.up * _jumpForce);

            Character.Animancer.Play(_jumpAnimation);
        }

        void OnJumpInput()
        {
            Character.SubStateMachine.TrySetState(this);
        }

        void JumpComplete()
        {
            _canExitState = true;
            if (_fallingState == null) Character.ParentStateMachine.CurrentState.TrySetDefaultSubState();
            else Character.SubStateMachine.TrySetState(_fallingState);
        }

    }
}