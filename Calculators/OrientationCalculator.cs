using UnityEngine;

namespace TopDownCharacter
{
    public class OrientationCalculator
    {
        Character _character;
        ControllerParameters _parameters;

        public OrientationCalculator(Character character, ControllerParameters parameters)
        {
            _character = character;
            _parameters = parameters;
        }

        /// <summary>
        /// This will return a slerped rotation towards the direction of the character's velocity, or no rotation if the velocity is zero.
        /// </summary>
        public Quaternion VelocityAlignedOrientation(Quaternion currentRotation, float deltaTime)
        {
            Vector3 lateralVelocity = new Vector3(_character.Motor.Velocity.x, 0, _character.Motor.Velocity.z);
            if (lateralVelocity.IsBasicallyZero()) return currentRotation;
            return RotationTowardsDirection(lateralVelocity, deltaTime);
        }

        /// <summary>
        /// This will return a slerped rotation from the current forward direction towards the target direction.
        /// </summary>
        public Quaternion DirectionAlignedOrientation(Vector3 targetLookDirection, float deltaTime)
        {
            return RotationTowardsDirection(targetLookDirection, deltaTime);
        }

        Quaternion RotationTowardsDirection(Vector3 targetLookDirection, float deltaTime)
        {
            Vector3 smoothedLookInputDirection = Vector3.Slerp(_character.Motor.CharacterForward, targetLookDirection, 
                1 - Mathf.Exp(-_parameters.MaxRotationSpeed * deltaTime)).normalized;
            return Quaternion.LookRotation(smoothedLookInputDirection, _character.Motor.CharacterUp);
        }
    }
}