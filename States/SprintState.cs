using Animancer;
using TopDownCharacter.Calculators;
using UnityEngine;

namespace TopDownCharacter.States
{
    public class SprintState : CharacterState
    {
        [SerializeField] MixerTransition2D _sprintMixer;

        [Tooltip("If the character tries to strafe facing this many degrees away from the direction they are moving, sprint state will be exited.")]
        [SerializeField] float _minimumVelocityFacingAngle = 50f;
        
        [Tooltip("When the sprint state is enabled, these parameters will be swapped for the current parameters. Sprint should be faster but less maneuverable.")]
        [SerializeField] ControllerParameters _sprintParameters;
        ControllerParameters _previousParameters;

        VelocityDirectionCalculator _velocityDirectionCalculator;

        protected override void LateAwake()
        {
            _velocityDirectionCalculator = new VelocityDirectionCalculator(Character.Motor, Character.Controller);
            Character.MovementInput.MovementInputUpdated += OnMovementInputUpdated;
        }

        public override float Priority => 6;

        public override bool CanEnterState => Character.MovementInput.SprintEnabled;

        void OnMovementInputUpdated(MovementInput movementInput)
        {
            if (movementInput.SprintEnabled)
            {
                Log($"Sprint input detected... Trying to enter sprint state.");
                Character.SubStateMachine.TrySetState(this);
            }
        }

        void OnEnable()
        {
            Log($"Swapping controller parameters for sprint speed/maneuverability.");
            _previousParameters = Character.Controller.ActiveControllerParameters;
            Character.Controller.ActiveControllerParameters = _sprintParameters;
            
            Character.Animancer.Play(_sprintMixer);
        }

        void OnDisable()
        {
            Log($"Exiting sprint state and restoring previous controller parameters.");
            Character.Controller.ActiveControllerParameters = _previousParameters;
        }
        
        public override bool CanExitState => !Character.MovementInput.SprintEnabled || !Character.Motor.GroundingStatus.IsStableOnGround;


        void FixedUpdate()
        {
            Vector2 velocityFacingDirection = _velocityDirectionCalculator.VelocityFacingDirection;
            
            Trace("Sprint Parameter", velocityFacingDirection);
            _sprintMixer.State.Parameter = velocityFacingDirection;

            if (_velocityDirectionCalculator.VelocityFacingAngle > _minimumVelocityFacingAngle)
            {
                Log($"Character is trying to strafe, sprint will be disabled.");
                Character.MovementInput.SprintEnabled = false;
            }
            
            if (!Character.MovementInput.SprintEnabled) Character.State.Reset();
        }
    }
}