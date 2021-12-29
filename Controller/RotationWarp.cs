using UnityEngine;

namespace TopDownCharacter
{
    public class RotationWarp
    {
        public bool IsActive => _timeSinceInitialization < _timeToWarp;
        float _timeToWarp;
        float _timeSinceInitialization;
        Quaternion _desiredRotation;

        public RotationWarp()
        {
            _timeToWarp = -1f;
        }

        public RotationWarp(Quaternion desiredRotation, float time = 0.25f)
        {
            _timeToWarp = time;
            _desiredRotation = desiredRotation;
        }

        public Quaternion GetNextRotation(Quaternion currentRotation, float deltaTime)
        {
            if (_timeSinceInitialization > _timeToWarp) return currentRotation;
            _timeSinceInitialization += deltaTime;
            return Quaternion.Slerp(currentRotation, _desiredRotation, _timeSinceInitialization / _timeToWarp);
        }
    }
}