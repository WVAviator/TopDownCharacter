using UnityEngine;

namespace TopDownCharacter
{
    public class VelocityCalculator
    {
        Character _character;
        ControllerParameters _parameters;

        public VelocityCalculator(Character character, ControllerParameters parameters)
        {
            _character = character;
            _parameters = parameters;
        }

        /// <summary>
        /// Returns a new velocity based on the character's grounding status, player input, and ground normal.
        /// </summary>
        public Vector3 CalculateVelocity(Vector3 currentVelocity, Vector3 movementInput, float deltaTime)
        {
            if (!_character.Motor.GroundingStatus.IsStableOnGround)
                return AirMovementVelocity(currentVelocity, movementInput, deltaTime);
            
            currentVelocity = HandleMovementOnSlope(currentVelocity, out Vector3 effectiveGroundNormal);
            Vector3 targetVelocity = GetTargetVelocity(movementInput, effectiveGroundNormal);
            return Vector3.Lerp(currentVelocity, targetVelocity,
                1f - Mathf.Exp(-_parameters.MaxAcceleration * deltaTime));
        }

        Vector3 AirMovementVelocity(Vector3 currentVelocity, Vector3 movementInput, float deltaTime)
        {
            if (movementInput.HasValue())
            {
                Vector3 targetMovementVelocity = movementInput * _parameters.MaxAirMovementSpeed;
                Vector3 velocityDiff = Vector3.ProjectOnPlane(targetMovementVelocity - currentVelocity, _parameters.Gravity);
                currentVelocity += velocityDiff * _parameters.MaxAirAcceleration * deltaTime;
            }

            currentVelocity += _parameters.Gravity * deltaTime;
            currentVelocity *= 1f / (1f + _parameters.Drag * deltaTime);

            return currentVelocity;
        }

        Vector3 GetTargetVelocity(Vector3 moveInput, Vector3 effectiveGroundNormal)
        {
            Vector3 inputRight = Vector3.Cross(moveInput, _character.Motor.CharacterUp);
            Vector3 reorientedInput = Vector3.Cross(effectiveGroundNormal, inputRight).normalized * moveInput.magnitude;
            Vector3 targetVelocity = reorientedInput * _parameters.MaxMovementSpeed;
            return targetVelocity;
        }

        Vector3 HandleMovementOnSlope(Vector3 currentVelocity, out Vector3 effectiveGroundNormal)
        {
            float currentVelocityMagnitude = currentVelocity.magnitude;
            effectiveGroundNormal = GetEffectiveGroundNormal(currentVelocity, currentVelocityMagnitude);

            return _character.Motor.GetDirectionTangentToSurface(currentVelocity, effectiveGroundNormal) *
                   currentVelocityMagnitude;
        }

        Vector3 GetEffectiveGroundNormal(Vector3 currentVelocity, float currentVelocityMagnitude)
        {
            Vector3 effectiveGroundNormal = _character.Motor.GroundingStatus.GroundNormal;

            if (currentVelocityMagnitude > 0f && _character.Motor.GroundingStatus.SnappingPrevented)
            {
                Vector3 groundPointToCharacter = _character.Motor.TransientPosition - _character.Motor.GroundingStatus.GroundPoint;
                if (Vector3.Dot(currentVelocity, groundPointToCharacter) >= 0f)
                {
                    effectiveGroundNormal = _character.Motor.GroundingStatus.OuterGroundNormal;
                }
                else
                {
                    effectiveGroundNormal = _character.Motor.GroundingStatus.InnerGroundNormal;
                }
            }

            return effectiveGroundNormal;
        }
        
    }
}