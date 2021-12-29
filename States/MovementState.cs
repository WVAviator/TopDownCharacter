using Animancer;
using TopDownCharacter.Calculators;
using UnityEngine;

namespace TopDownCharacter.States
{
    public class MovementState : CharacterState
    {
        [SerializeField] MixerTransition2D _movementAnimations;

        VelocityDirectionCalculator _velocityDirectionCalculator;

        void Start()
        {
            _velocityDirectionCalculator = new VelocityDirectionCalculator(Character.Motor, Character.Controller);
        }

        void OnEnable()
        {
            Character.Animancer.Play(_movementAnimations);
        }

        void FixedUpdate()
        {
            Draw("Movement Parameter", _velocityDirectionCalculator.VelocityFacingDirection);
            _movementAnimations.State.Parameter = _velocityDirectionCalculator.VelocityFacingDirection;
        }
    }
}