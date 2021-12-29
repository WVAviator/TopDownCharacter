using Animancer;
using Animancer.FSM;
using KinematicCharacterController;
using UnityEngine;

namespace TopDownCharacter.States
{
    public class FallingState : CharacterState
    {

        [SerializeField] ClipTransition _fallingControlledAnimation;
        [SerializeField] ClipTransition _fallingUncontrolledAnimation;
        
        [Tooltip("If vertical velocity magnitude exceeds this value, the character will begin flailing and will land hard.")]
        [SerializeField] float maxVerticalControlledVelocity = -10f;

        [SerializeField] ClipTransition _safeLandingAnimation;
        [SerializeField] ClipTransition _hardLandingAnimation;

        bool _isFallingUncontrolled;
        bool _landed;
        bool _canExitState;

        public override bool CanExitState => _canExitState;

        protected override void LateAwake()
        {
            Character.Controller.GroundingStatusChanged += OnGroundingStatusChanged;
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

        void OnGroundingStatusChanged(CharacterGroundingReport groundingStatus)
        {
            Log($"Registered a change in grounding status, character is now {(groundingStatus.IsStableOnGround ? "stable on the ground." : "in the air.")}");
            if (!groundingStatus.IsStableOnGround) Character.SubStateMachineBuffer.Buffer(this, 0.5f);
            else Landed();
        }

        void FixedUpdate()
        {
            if (!_isFallingUncontrolled && Character.Motor.Velocity.y < maxVerticalControlledVelocity)
            {
                _isFallingUncontrolled = true;
                Character.Animancer.Play(_fallingUncontrolledAnimation);
            }
        }

        void Landed()
        {
            if (_landed) return;
            _landed = true;
            
            ClipTransition landingAnimation =
                _isFallingUncontrolled ? _hardLandingAnimation : _safeLandingAnimation;

            Character.Controller.RootMotionEnabled = true;
                
            Character.Animancer.Play(landingAnimation);
        }

        void ResumeGroundedState()
        {
            _canExitState = true;
            Character.Controller.RootMotionEnabled = false;

            Character.ParentStateMachine.CurrentState.ForceSetDefaultState();
        }
    }
}