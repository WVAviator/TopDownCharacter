using Animancer;
using Animancer.FSM;
using UnityEngine;

namespace TopDownCharacter.States
{
    public class FallingState : CharacterState
    {

        [SerializeField] ClipTransition _fallingControlledAnimation;
        [SerializeField] ClipTransition _fallingUncontrolledAnimation;
        [SerializeField] float maxVerticalControlledVelocity = -10f;

        [SerializeField] ClipTransition _safeLandingAnimation;
        [SerializeField] ClipTransition _hardLandingAnimation;

        bool _isFallingUncontrolled;
        bool _landed;
        bool _canExitState;

        public override bool CanExitState => _canExitState;

        void Start()
        {
            _safeLandingAnimation.Events.OnEnd += ResumeGroundedState;
            _hardLandingAnimation.Events.OnEnd += ResumeGroundedState;
        }

        void OnEnable()
        {
            _isFallingUncontrolled = false;
            _landed = false;
            _canExitState = false;
            
            Character.Animancer.Play(_fallingControlledAnimation);
        }

        void FixedUpdate()
        {
            if (!_isFallingUncontrolled && Character.Motor.Velocity.y < maxVerticalControlledVelocity)
            {
                _isFallingUncontrolled = true;
                Character.Animancer.Play(_fallingUncontrolledAnimation);
            }

            if (!_landed && Character.Motor.GroundingStatus.IsStableOnGround)
            {
                _landed = true;

                ClipTransition landingAnimation =
                    _isFallingUncontrolled ? _hardLandingAnimation : _safeLandingAnimation;

                Character.Controller.RootMotionEnabled = true;
                
                Character.Animancer.Play(landingAnimation);
            }
        }

        void ResumeGroundedState()
        {
            _canExitState = true;
            Character.Controller.RootMotionEnabled = false;
            
            StateMachine<CharacterState> parentStateMachine = Character.SubStateMachine;
            CharacterState targetState = Character.MovementInput.CurrentMovementInput.HasInput 
                ? Character.ParentStateMachine.CurrentState.Movement 
                : Character.ParentStateMachine.CurrentState.Idle;
            parentStateMachine.TrySetState(targetState);
        }
    }
}