using UnityEngine;

namespace TopDownCharacter
{
    public class PositionWarp
    {
        public bool IsActive => _timeSinceInitialization < _timeToWarp;
        float _timeToWarp;
        float _timeSinceInitialization;
        Vector3 _desiredPosition;

        public PositionWarp()
        {
            _timeToWarp = -1f;
        }

        public PositionWarp(Vector3 desiredPosition, float time = 0.25f)
        {
            _timeToWarp = time;
            _desiredPosition = desiredPosition;
        }

        public Vector3 GetNextPosition(Vector3 currentPosition, float deltaTime)
        {
            if (_timeSinceInitialization > _timeToWarp) return currentPosition;
            _timeSinceInitialization += deltaTime;
            return Vector3.Lerp(currentPosition, _desiredPosition, _timeSinceInitialization / _timeToWarp);
        }
    }
}