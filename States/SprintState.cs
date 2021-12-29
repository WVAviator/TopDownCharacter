using Animancer;
using TopDownCharacter.Calculators;
using UnityEngine;

namespace TopDownCharacter.States
{
    public class SprintState : CharacterState
    {

        [SerializeField] MixerTransition2D _sprintMixer;
        
        [Tooltip("If the character's speed falls below this amount, sprint state will be exited.")]
        [SerializeField] float minimumSprintSpeed = 3f;

        [Tooltip("If the character tries to strafe facing this many degrees away from the direction they are moving, sprint state will be exited.")]
        [SerializeField] float _minimumVelocityFacingAngle = 20f;
        
        [SerializeField] ControllerParameters _sprintParameters;
        ControllerParameters _previousParameters;

        VelocityDirectionCalculator _velocityDirectionCalculator;
        VelocityDirectionCalculator VelocityDirectionCalculator =>
            _velocityDirectionCalculator ??= new VelocityDirectionCalculator(Character.Motor, Character.Controller);

        void Start()
        {
            _velocityDirectionCalculator = new VelocityDirectionCalculator(Character.Motor, Character.Controller);
        }

        void OnEnable()
        {
            _previousParameters = Character.Controller.ActiveControllerParameters;
            Character.Controller.ActiveControllerParameters = _sprintParameters;
            Character.Animancer.Play(_sprintMixer);
        }

        void OnDisable()
        {
            Character.Controller.ActiveControllerParameters = _previousParameters;
        }

        public override bool CanEnterState
        {
            get
            {
                if (!Character.MovementInput.SprintEnabled) return false;

                if (!GoingTooSlow() && !TryingToStrafe()) return true;
                
                Character.MovementInput.SprintEnabled = false;
                return false;
            }
        }

        void FixedUpdate()
        {
            _sprintMixer.State.Parameter = _velocityDirectionCalculator.VelocityFacingDirection;
            
            if (!GoingTooSlow() && !TryingToStrafe() && Character.MovementInput.SprintEnabled) return;
            Character.MovementInput.SprintEnabled = false;
                
            if (!Character.SubStateMachine.TrySetState(Character.ParentStateMachine.CurrentState.Movement))
                Character.SubStateMachine.TrySetState(Character.ParentStateMachine.CurrentState.Idle);

            
        }

        bool TryingToStrafe()
        {
            return VelocityDirectionCalculator.VelocityFacingAngle.Abs() > _minimumVelocityFacingAngle;
        }

        bool GoingTooSlow() => Character.Motor.Velocity.sqrMagnitude < minimumSprintSpeed * minimumSprintSpeed;
    }
}