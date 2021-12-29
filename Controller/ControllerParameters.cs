using UnityEngine;

namespace TopDownCharacter
{
    [CreateAssetMenu(menuName = "TopDownCharacter/Controller Parameters")]
    public class ControllerParameters : ScriptableObject
    {
        [Tooltip("The speed to which this character will accelerate with maximum input.")]
        public float MaxMovementSpeed = 3.37f;
        
        [Tooltip("Increasing this will accelerate the character to the max speed more quickly and also allow for quicker changes in direction.")]
        public float MaxAcceleration = 5f;
        
        [Tooltip("The maximum angular speed at which the character will rotate.")]
        public float MaxRotationSpeed = 3f;

        [Tooltip("The maximum movement speed while in the air.")]
        public float MaxAirMovementSpeed = 3f;

        [Tooltip("The maximum acceleration speed while in the air.")]
        public float MaxAirAcceleration = 3f;
        
        [Tooltip("When airborne, the character will be accelerated by this velocity.")]
        public Vector3 Gravity = new Vector3(0, -10, 0);
        
        [Tooltip("When airborne, the character's lateral movement will be inhibited by this amount.")]
        public float Drag = 0.1f;
    }
}