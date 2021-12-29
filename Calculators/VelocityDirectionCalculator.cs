using KinematicCharacterController;
using UnityEngine;

namespace TopDownCharacter.Calculators
{
    public class VelocityDirectionCalculator
    {
        KinematicCharacterMotor _motor;
        TopDownController _controller;

        public VelocityDirectionCalculator(KinematicCharacterMotor motor, TopDownController controller)
        {
            _motor = motor;
            _controller = controller;
        }
        
        
        public Vector2 VelocityFacingDirection
        {
            get
            {
                Vector3 facingDirection = _motor.CharacterForward;
                Vector3 velocityDirection = _motor.Velocity;
                Vector2 movementParameter =
                    (Quaternion.FromToRotation(facingDirection, velocityDirection) * Vector3.forward).RotateOntoXZPlane();
                movementParameter *=
                    velocityDirection.magnitude / _controller.ActiveControllerParameters.MaxMovementSpeed;
                return movementParameter;
            }
        }

        public float VelocityFacingAngle
        {
            get
            {
                Vector3 facingDirection = _motor.CharacterForward;
                Vector3 velocityDirection = _motor.Velocity;
                return Vector3.SignedAngle(facingDirection, velocityDirection, Vector3.up);
            }
        }
    }
}