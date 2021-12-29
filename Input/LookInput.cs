using UnityEngine;

namespace TopDownCharacter
{
    public struct LookInput
    {
        //public Vector3 RawLookVector;
        //public Vector3 NormalizedLookVector;
        
        public Vector3 LookDirection;

        public bool HasInput;
        
        public Vector3 TargetLookPosition;
        
        public static implicit operator Vector3(LookInput lookInput) => lookInput.LookDirection;
        
        public override string ToString()
        {
            return $"LookInput [ Direction: {LookDirection}: Input: {HasInput}, Target: {TargetLookPosition} ]";
        }
    }
}