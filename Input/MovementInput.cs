using UnityEngine;

namespace TopDownCharacter
{
    public struct MovementInput
    {
        //public Vector3 RawMovementVector;
        //public Vector3 NormalizedMovementVector;
        public Vector3 MovementVector;
        public bool HasInput;
        public bool SprintEnabled;

        public static implicit operator Vector3(MovementInput movementInput) => movementInput.MovementVector;
        public override string ToString()
        {
            return $"MovementInput [ MovementVector: {MovementVector}, Sprint: {SprintEnabled}, Input: {HasInput} ]";
        }
    }
}