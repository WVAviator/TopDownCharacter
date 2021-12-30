using System.Collections.Generic;
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

        [SerializeField] List<ClipTransition> _safeLandingAnimations;
        [SerializeField] List<ClipTransition> _hardLandingAnimations;

        bool _isFallingUncontrolled;
        bool _landed;
        bool _canExitState;

        public override bool CanEnterState => !Character.Motor.GroundingStatus.IsStableOnGround;
        public override bool CanExitState => _canExitState;

        protected override void LateAwake()
        {
            _safeLandingAnimations.ForEach(a => a.Events.OnEnd += ResumeGroundedState);
            _hardLandingAnimations.ForEach(a => a.Events.OnEnd += ResumeGroundedState);
        }

        public override float Priority => 10;

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

            if (Character.Motor.GroundingStatus.IsStableOnGround) Landed();
        }

        void Landed()
        {
            if (_landed) return;
            
            Log($"Landing detected. Playing landing animation.");
            
            _landed = true;
            
            ClipTransition landingAnimation =
                _isFallingUncontrolled ? AnimationSelector.RandomClipTransition(_hardLandingAnimations) : AnimationSelector.MatchLateralMotion(_safeLandingAnimations, Character.Motor.Velocity);

            Character.Controller.RootMotionEnabled = true;
                
            Character.Animancer.Play(landingAnimation);
        }

        void ResumeGroundedState()
        {
            _canExitState = true;
            Character.Controller.RootMotionEnabled = false;
            
            Log($"Landing animation completed. Resuming idle.");

            Character.State.Reset();
        }
    }
}