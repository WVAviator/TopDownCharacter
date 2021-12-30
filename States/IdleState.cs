using System;
using System.Collections.Generic;
using Animancer;
using TopDownCharacter.Calculators;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TopDownCharacter.States
{
    public class IdleState : CharacterState
    {
        [SerializeField] ClipTransition _primaryIdleAnimation;
        [SerializeField] List<ClipTransition> _secondaryIdleAnimations;
        [Tooltip("The number of times to play the primary idle animation clip before shuffling a secondary idle clip.")]
        [SerializeField] int _primaryIdlePlays = 3;

        [SerializeField] ClipTransition _leftTurn90Animation;
        [SerializeField] ClipTransition _leftTurn180Animation;
        [SerializeField] ClipTransition _rightTurn90Animation;
        [SerializeField] ClipTransition _rightTurn180Animation;
        [SerializeField] float _turn90Threshold = 55f;
        [SerializeField] float _turn180Threshold = 145f;

        TurnInPlaceCalculator _turnInPlaceCalculator;
        
        int _primaryIdlePlaysRemaining;
        int _lastSecondaryIdleIndex;

        bool _isTurningInPlace;

        protected override void LateAwake()
        {
            //Character.MovementInput.MovementInputUpdated += OnMovementUpdated;
            //Character.Controller.VelocityUpdated += OnMovementUpdated;

            _leftTurn90Animation.Events.OnEnd += ResetPrimaryIdle;
            _leftTurn180Animation.Events.OnEnd += ResetPrimaryIdle;
            _rightTurn90Animation.Events.OnEnd += ResetPrimaryIdle;
            _rightTurn180Animation.Events.OnEnd += ResetPrimaryIdle;
            _primaryIdleAnimation.Events.OnEnd += ResetPrimaryIdle;
            _secondaryIdleAnimations.ForEach(s => s.Events.OnEnd += ResetPrimaryIdle);
        }

        public override float Priority => 0;

        public override bool CanEnterState =>
            Character.Motor.Velocity.IsBasicallyZero();

        void Start()
        {
            _turnInPlaceCalculator =
                new TurnInPlaceCalculator(Character.Motor, Character.MovementInput);
        }

        void OnMovementUpdated(Vector3 velocity)
        {
            OnMovementUpdated(Character.MovementInput.CurrentMovementInput);
        }

        void OnMovementUpdated(MovementInput movementInput)
        {
            if (movementInput.HasInput || Character.Motor.Velocity.HasValue()) return;
            
            Log($"No input or movement detected... Trying to enter idle state.");
            Character.SubStateMachine.TrySetState(this);
        }

        void OnEnable()
        {
            Log($"Idle state enabled.");
            
            Character.Controller.RootMotionEnabled = true;
            _primaryIdlePlaysRemaining = _primaryIdlePlays;
            _isTurningInPlace = false;
            PlayIdle();
        }

        void OnDisable()
        {
            Log($"Idle state disabled.");
            Character.Controller.RootMotionEnabled = false;
        }

        void PlayIdle()
        { 
            if (_primaryIdlePlaysRemaining <= 0)
            {
                _primaryIdlePlaysRemaining = _primaryIdlePlays;
                ShuffleSecondaryIdle();
                return;
            }

            
            Character.Animancer.Play(_primaryIdleAnimation);
            _primaryIdlePlaysRemaining--;
            Log($"Playing primary idle. Remaining primary idle plays: {_primaryIdlePlaysRemaining}.");
        }

        void ResetPrimaryIdle()
        {
            Log($"Resetting the primary idle to time 0.");
            _primaryIdleAnimation.State.Time = 0;
            
            Log($"Set isTurningInPlace to false.");
            _isTurningInPlace = false;
            
            PlayIdle();
        }

        void ShuffleSecondaryIdle()
        {
            Log($"Shuffling secondary idles. Secondary idle count: {_secondaryIdleAnimations.Count}.");

            if (_secondaryIdleAnimations.Count == 0)
            {
                Log($"No secondary idles. Resuming primary idles.");
                ResetPrimaryIdle();
            }
            else if (_secondaryIdleAnimations.Count == 1)
            {
                Log($"{_secondaryIdleAnimations[0].Name} is the only secondary idle animation. No shuffling is necessary.");
                Character.Animancer.Play(_secondaryIdleAnimations[0]);
            }
            else
            {
                Log($"Shuffling secondary idles, but skipping {_secondaryIdleAnimations[_lastSecondaryIdleIndex]} since it was played last.");
                int index = _lastSecondaryIdleIndex;
                while (index == _lastSecondaryIdleIndex) index = Random.Range(0, _secondaryIdleAnimations.Count);
                _lastSecondaryIdleIndex = index;
                
                Log($"Playing {_secondaryIdleAnimations[index].Name} and caching index {index} so that it will not be repeated in the next shuffle.");
                Character.Animancer.Play(_secondaryIdleAnimations[index]);
            }
        }

        //public override bool CanExitState => !Character.Motor.GroundingStatus.IsStableOnGround || Character.MovementInput.CurrentMovementInput.HasInput;

        void FixedUpdate()
        {
            HandleTurnInPlace();
        }

        void HandleTurnInPlace()
        {
            Draw("Turning In Place", _isTurningInPlace);
            if (_isTurningInPlace) return;
            

            if (_turnInPlaceCalculator.BeyondThreshold(_turn180Threshold))
            {
                Log($"A 180 degree turn in place was detected with desired rotation of {_turnInPlaceCalculator.DesiredRotation} which is beyond the threshold of {_turn180Threshold}.");
                ClipTransition rotationClip = _turnInPlaceCalculator.RightTurnDesired
                    ? _rightTurn180Animation
                    : _leftTurn180Animation;
                Character.Animancer.Play(rotationClip);
                _isTurningInPlace = true;
            }
            else if (_turnInPlaceCalculator.BeyondThreshold(_turn90Threshold))
            {
                Log($"A 90 degree turn in place was detected with desired rotation of {_turnInPlaceCalculator.DesiredRotation} which is beyond the threshold of {_turn90Threshold}.");
                ClipTransition rotationClip = _turnInPlaceCalculator.RightTurnDesired
                    ? _rightTurn90Animation
                    : _leftTurn90Animation;
                Character.Animancer.Play(rotationClip);
                _isTurningInPlace = true;
            }
            else
            {
                Draw("Desired TIP Rotation", _turnInPlaceCalculator.DesiredRotation);
            }
        }
    }
}