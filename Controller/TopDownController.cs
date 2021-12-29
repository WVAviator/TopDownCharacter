using System;
using KinematicCharacterController;
using UnityEngine;

namespace TopDownCharacter
{
    public class TopDownController : CharacterBehaviour, ICharacterController
    {

        [Tooltip("These controller parameters will be applied upon initialization and remain in effect unless swapped out at runtime.")]
        [SerializeField] ControllerParameters _defaultControllerParameters;

        /// <summary>
        /// Assign new ControllerParameters to swap out the active parameters which control movement velocity and orientation calculations.
        /// </summary>
        public ControllerParameters ActiveControllerParameters
        {
            get => _activeControllerParameters;
            set
            {
                _activeControllerParameters = value;
                _activeVelocityCalculator = new VelocityCalculator(Character, value);
                _activeOrientationCalculator = new OrientationCalculator(Character, value);
            }
        }

        ControllerParameters _activeControllerParameters;
        VelocityCalculator _activeVelocityCalculator;
        OrientationCalculator _activeOrientationCalculator;

        
        /// <summary>
        /// Setting this to true will enable animation root motion and cancel any regular velocity and rotation calculations.
        /// </summary>
        public bool RootMotionEnabled
        {
            get => Character.RootMotionRedirect.RootMotionEnabled;
            set
            {
                Character.RootMotionRedirect.RootMotionEnabled = value;
                if (value) Character.RootMotionRedirect.ResetDeltas();
            }
        }

        /// <summary>
        /// Disabling collisions will allow the player to pass through any object, including the ground.
        /// </summary>
        public bool CollisionEnabled { get; set; } = true;


        Vector3 _additionalVelocity;
        Quaternion _additionalRotation;

        Vector3 _lastVelocity = Vector3.zero;

        PositionWarp _activePositionWarp = new PositionWarp();
        RotationWarp _activeRotationWarp = new RotationWarp();

        public event Action<CharacterGroundingReport> GroundingStatusChanged;
        public event Action<Vector3> VelocityUpdated;

        protected override void Awake()
        {
            base.Awake();

            if (_defaultControllerParameters == null)
                _defaultControllerParameters = ScriptableObject.CreateInstance<ControllerParameters>();
            ActiveControllerParameters = _defaultControllerParameters;
            
        }

        void Start() => Character.Motor.CharacterController = this;
        
        public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
        {
            if (_activeRotationWarp.IsActive)
            {
                currentRotation = _activeRotationWarp.GetNextRotation(currentRotation, deltaTime);
                return;
            }
            
            if (RootMotionEnabled)
            {
                currentRotation = Character.RootMotionRedirect.CachedRotationDelta * currentRotation;
                return;
            }

            if (Character.MovementInput.CurrentLookInput.HasInput)
                currentRotation =
                    _activeOrientationCalculator.DirectionAlignedOrientation(Character.MovementInput.CurrentLookInput,
                        deltaTime);
            else currentRotation = _activeOrientationCalculator.VelocityAlignedOrientation(currentRotation, deltaTime);

            currentRotation *= _additionalRotation;
        }

        public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            if (_activePositionWarp.IsActive)
            {
                currentVelocity = Vector3.zero;
                Character.Motor.SetPosition(_activePositionWarp.GetNextPosition(transform.position, deltaTime));
            }
            
            if (RootMotionEnabled)
            {
                currentVelocity = Character.RootMotionRedirect.CachedPositionDelta / deltaTime;
                return;
            }

            currentVelocity =
                _activeVelocityCalculator.CalculateVelocity(currentVelocity, Character.MovementInput.CurrentMovementInput,
                    deltaTime);

            currentVelocity += _additionalVelocity;
        }

        public void AddVelocity(Vector3 velocity)
        {
            _additionalVelocity += velocity;
        }

        public void AddRotation(Quaternion rotation)
        {
            _additionalRotation *= rotation;
        }

        public void Warp(Vector3 position, Quaternion rotation, float time)
        {
            
        }

        public void BeforeCharacterUpdate(float deltaTime)
        {
            
        }

        public void PostGroundingUpdate(float deltaTime)
        {
            if (!Character.Motor.GroundingStatus.IsStableOnGround && Character.Motor.LastGroundingStatus.IsStableOnGround) GroundingStatusChanged?.Invoke(Character.Motor.GroundingStatus);
            if (Character.Motor.GroundingStatus.IsStableOnGround && !Character.Motor.LastGroundingStatus.IsStableOnGround) GroundingStatusChanged?.Invoke(Character.Motor.GroundingStatus);
        }

        public void AfterCharacterUpdate(float deltaTime)
        {
            _additionalVelocity = Vector3.zero;
            _additionalRotation = Quaternion.identity;
            
            if(Character.Motor.Velocity != _lastVelocity) VelocityUpdated?.Invoke(Character.Motor.Velocity);
            _lastVelocity = Character.Motor.Velocity;
        }

        public bool IsColliderValidForCollisions(Collider coll) => CollisionEnabled;

        public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
        {
            
        }

        public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint,
            ref HitStabilityReport hitStabilityReport)
        {
            
        }

        public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition,
            Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
        {
            
        }

        public void OnDiscreteCollisionDetected(Collider hitCollider)
        {
            
        }
    }
}