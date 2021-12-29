using KinematicCharacterController;
using UnityEngine;

namespace TopDownCharacter.Calculators
{
    public class TurnInPlaceCalculator
    {
        KinematicCharacterMotor _motor;
        IMovementInput _movementInput;

        /// <summary>
        /// The desired rotation represents the signed angle difference on the y-axis between the character facing direction and the player's desired direction.
        /// </summary>
        public float DesiredRotation
        {
            get
            {
                Vector3 currentLookDirection = _movementInput.CurrentLookInput.HasInput ? _movementInput.CurrentLookInput : _motor.CharacterForward;
                Vector3 currentForwardDirection = _motor.CharacterForward;
            
                float directionDifference =
                    Vector3.SignedAngle(currentForwardDirection, currentLookDirection, Vector3.up);

                return directionDifference;
            }
        }

        /// <summary>
        /// Returns true if the desired rotation direction is clockwise, otherwise returns false.
        /// </summary>
        public bool RightTurnDesired => DesiredRotation > 0;

        public TurnInPlaceCalculator(KinematicCharacterMotor motor, IMovementInput movementInput)
        {
            _motor = motor;
            _movementInput = movementInput;
        }

        /// <summary>
        /// Returns true if the desired rotation is greater than the specified threshold.
        /// </summary>
        public bool BeyondThreshold(float threshold)
        {
            return Mathf.Abs(DesiredRotation) > threshold;
        }
    }
}