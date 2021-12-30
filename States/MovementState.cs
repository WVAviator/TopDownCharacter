using Animancer;
using TopDownCharacter.Calculators;
using UnityEngine;

namespace TopDownCharacter.States
{
    public class MovementState : CharacterState
    {
        [SerializeField] MixerTransition2D _movementAnimations;

        VelocityDirectionCalculator _velocityDirectionCalculator;

        protected override void LateAwake()
        {
            _velocityDirectionCalculator = new VelocityDirectionCalculator(Character.Motor, Character.Controller);
            //Character.MovementInput.MovementInputUpdated += OnMovementInputUpdated;
            //Character.Controller.VelocityUpdated += OnVelocityUpdated;
        }

        public override float Priority => 1;

        public override bool CanEnterState =>
            Character.Motor.Velocity.HasValue() || Character.MovementInput.CurrentMovementInput.HasInput;

        void OnVelocityUpdated(Vector3 velocity)
        {
            if (velocity.HasValue())
            {
                Log($"Movement detected... Trying to enter movement state.");
                Character.SubStateMachine.TrySetState(this);
            }
        }

        void OnMovementInputUpdated(MovementInput movementInput)
        {
            if (movementInput.HasInput)
            {
                Log($"Movement detected... Trying to enter movement state.");
                Character.SubStateMachine.TrySetState(this);
            }
        }

        void OnEnable()
        {
            Character.Animancer.Play(_movementAnimations);
        }

        void FixedUpdate()
        {
            Draw("Movement Parameter", _velocityDirectionCalculator.VelocityFacingDirection);
            _movementAnimations.State.Parameter = _velocityDirectionCalculator.VelocityFacingDirection;
            
            if (Character.Motor.Velocity.IsBasicallyZero()) Character.State.Reset();
        }
    }
}