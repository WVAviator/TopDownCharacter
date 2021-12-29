using UnityEngine;

namespace TopDownCharacter
{
    public class RootMotionRedirect : CharacterBehaviour
    {
        /// <summary>
        /// Returns the total cached rotation delta since the last time this property was called, or since ResetDeltas was called.
        /// </summary>
        public Quaternion CachedRotationDelta
        {
            get
            {
                if (!RootMotionEnabled) Debug.LogWarning("CachedRotationDelta was retrieved even though root motion has been disabled. This value will likely be zero. Enable root motion before retrieving this value.");

                
                Quaternion rotationValue = _cachedRotationDelta;
                ResetRotationDelta();
                return rotationValue;
            }
            
        }

        /// <summary>
        /// Returns the total cached position delta since the last time this property was called, or since ResetDeltas was called.
        /// </summary>
        public Vector3 CachedPositionDelta
        {
            get
            {
                if (!RootMotionEnabled) Debug.LogWarning("CachedPositionDelta was retrieved even though root motion has been disabled. This value will likely be zero. Enable root motion before retrieving this value.");
                
                Vector3 positionValue = _cachedPositionDelta;
                ResetPositionDelta();
                return positionValue;
            }
            
        }

        /// <summary>
        /// Returns whether root motion has been enabled for the animator. Setting this to true will enable root motion on the animator.
        /// </summary>
        public bool RootMotionEnabled
        {
            get => Character.Animancer.Animator.applyRootMotion;
            set => Character.Animancer.Animator.applyRootMotion = value;
        }

        Vector3 _cachedPositionDelta;
        Quaternion _cachedRotationDelta;

        protected override void Awake()
        {
            base.Awake();
            RootMotionEnabled = false;
            ResetDeltas();
        }
        
        /// <summary>
        /// Resets position and rotation deltas (used to erase any saved deltas from before root motion was disabled)
        /// </summary>
        public void ResetDeltas()
        {
            ResetPositionDelta();
            ResetRotationDelta();
        }

        void OnAnimatorMove()
        {
            if (!RootMotionEnabled) return;
            _cachedRotationDelta *= Character.Animancer.Animator.deltaRotation;
            _cachedPositionDelta += Character.Animancer.Animator.deltaPosition;
        }
        
        void ResetPositionDelta() => _cachedPositionDelta = Vector3.zero;
        void ResetRotationDelta() => _cachedRotationDelta = Quaternion.identity;
    }
}