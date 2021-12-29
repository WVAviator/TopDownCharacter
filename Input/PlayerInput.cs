using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TopDownCharacter
{
    public class PlayerInput : CharacterBehaviour, IMovementInput, IActionInput
    {
        public event Action<MovementInput> MovementInputUpdated;
        public event Action<LookInput> LookInputUpdated;

        public MovementInput CurrentMovementInput => _currentMovementInput;
        public LookInput CurrentLookInput => _currentLookInput;

        MovementInput _currentMovementInput;
        LookInput _currentLookInput;

        [SerializeField] InputActionAsset _inputActions;
        [SerializeField] InputType _inputType = InputType.Controller;

        InputActionMap _activeInputActionMap;

        [SerializeField] Camera _mainCamera;

        [SerializeField] float _minimumSprintInputMagnitude = 0.75f;

        public bool SprintEnabled { get; set; }


        protected override void Awake()
        {
            InitializeInputActions();
            _inputActions.Enable();

            if (_mainCamera == null)
            {
                Log("Input camera not set in inspector - will use Camera.main");
                _mainCamera = Camera.main;
            }

            InitializeDefaultInputs();
        }

        
        
        void OnEnable() => SubscribeInputActionEvents();

        void OnDisable() => UnsubscribeInputActionEvents();


        void InitializeInputActions()
        {
            Log($"Initializing InputActionAsset and ActionMap");
            
            string actionMap = _inputType switch
            {
                InputType.Controller => "Controller",
                InputType.Mouse => "Mouse",
                _ => throw new ArgumentOutOfRangeException()
            };
            _activeInputActionMap = _inputActions.FindActionMap($"MovementMap_{actionMap}");
            
            Log($"Active ActionMap: {_activeInputActionMap.name}");
        }

        void InitializeDefaultInputs()
        {
            Log($"Initializing default inputs");
            
            _currentMovementInput = new MovementInput()
            {
                MovementVector = Vector3.zero,
                HasInput = false
            };
            
            Log($"Default movement input set to {_currentMovementInput.ToString()}");

            _currentLookInput = new LookInput()
            {
                LookDirection = transform.forward,
                HasInput = false,
                TargetLookPosition = transform.position + transform.forward
            };
            
            Log($"Default look input set to {_currentLookInput.ToString()}");
        }

        void SubscribeInputActionEvents()
        {
            Log($"Subscribing to all InputAction events");
            
            _activeInputActionMap.FindAction("Movement").started += OnMovementInputChanged;
            _activeInputActionMap.FindAction("Movement").canceled += OnMovementInputChanged;
            _activeInputActionMap.FindAction("Movement").performed += OnMovementInputChanged;

            _activeInputActionMap.FindAction("Look").started += OnLookInputChanged;
            _activeInputActionMap.FindAction("Look").canceled += OnLookInputChanged;
            _activeInputActionMap.FindAction("Look").performed += OnLookInputChanged;
            
            _activeInputActionMap.FindAction("Sprint").performed += ToggleSprint;
            _activeInputActionMap.FindAction("Jump").performed += TriggerJump;
        }

        void TriggerJump(InputAction.CallbackContext obj)
        {
            Jump?.Invoke();
        }

        void UnsubscribeInputActionEvents()
        {
            Log($"Unsubscribing to all InputAction events");
            
            _activeInputActionMap.FindAction("Movement").started -= OnMovementInputChanged;
            _activeInputActionMap.FindAction("Movement").canceled -= OnMovementInputChanged;
            _activeInputActionMap.FindAction("Movement").performed -= OnMovementInputChanged;

            _activeInputActionMap.FindAction("Look").started -= OnLookInputChanged;
            _activeInputActionMap.FindAction("Look").canceled -= OnLookInputChanged;
            _activeInputActionMap.FindAction("Look").performed -= OnLookInputChanged;
            
            _activeInputActionMap.FindAction("Sprint").performed -= ToggleSprint;
            _activeInputActionMap.FindAction("Jump").performed -= TriggerJump;
        }

        void OnMovementInputChanged(InputAction.CallbackContext context)
        {
            (bool hasInput, Vector3 cameraCorrectedInputVector) = ExtractMovementVector(context);
            
            Trace("Final Movement Vector", cameraCorrectedInputVector);

            ModifySprint(cameraCorrectedInputVector);
            
            Draw( "Sprint Enabled", SprintEnabled);

            _currentMovementInput = new MovementInput()
            {
                MovementVector = cameraCorrectedInputVector,
                HasInput = hasInput,
                SprintEnabled = SprintEnabled
            };
            
            MovementInputUpdated?.Invoke(_currentMovementInput);
        }

        void ModifySprint(Vector3 cameraCorrectedInputVector)
        {
            if (cameraCorrectedInputVector.sqrMagnitude < _minimumSprintInputMagnitude * _minimumSprintInputMagnitude) SprintEnabled = false;
        }

        (bool hasInput, Vector3 cameraCorrectedInputVector) ExtractMovementVector(InputAction.CallbackContext context)
        {
            Vector2 rawInputVector = context.ReadValue<Vector2>();
            
            Trace( $"Raw {context.action.name} Input Vector", rawInputVector);

            bool hasInput = false;
            Vector3 cameraCorrectedInputVector = Vector3.zero;

            if (rawInputVector.sqrMagnitude > 0.001f)
            {
                Vector3 xzInputVector = new Vector3(rawInputVector.x, 0, rawInputVector.y);
                Vector3 normalizedInputVector = Normalize(xzInputVector);
                cameraCorrectedInputVector = OrientToCamera(normalizedInputVector);
                hasInput = true;
            }

            return (hasInput, cameraCorrectedInputVector);
        }

        Vector3 OrientToCamera(Vector3 normalizedInputVector)
        {
            Vector3 forward = Vector3.ProjectOnPlane(_mainCamera.transform.forward, Vector3.up);
            return Quaternion.FromToRotation(Vector3.forward, forward) * normalizedInputVector;
        }

        Vector3 Normalize(Vector3 inputVector) => inputVector.sqrMagnitude > 1f ? inputVector.normalized : inputVector;

        void OnLookInputChanged(InputAction.CallbackContext context)
        {
            if (_inputType == InputType.Controller) ProcessControllerInputContext(context);
            else if (_inputType == InputType.Mouse) ProcessMouseInputContext(context);
        }

        void ProcessControllerInputContext(InputAction.CallbackContext context)
        {
            (bool hasInput, Vector3 cameraCorrectedInputVector) = ExtractMovementVector(context);

            if (!hasInput) cameraCorrectedInputVector = GetDefaultLookDirection();
            Vector3 targetLookPosition = cameraCorrectedInputVector * 3f;
            
            Trace( "Final Look Direction",cameraCorrectedInputVector.Flatten());
            Trace( "Target Look Position", targetLookPosition.Flatten());

            _currentLookInput = new LookInput()
            {
                LookDirection = cameraCorrectedInputVector,
                HasInput = hasInput,
                TargetLookPosition = targetLookPosition
            };
            
            LookInputUpdated?.Invoke(_currentLookInput);
        }

        void ProcessMouseInputContext(InputAction.CallbackContext context)
        {
            Vector2 mousePosition = context.ReadValue<Vector2>();
            Ray mouseRay = _mainCamera.ScreenPointToRay(mousePosition);

            Vector3 lookDirection = GetDefaultLookDirection();
            Vector3 lookTarget = lookDirection * 3f;
            bool hasInput = false;
            
            if (Physics.Raycast(mouseRay, out RaycastHit mouseHit))
            {
                lookDirection = Normalize(mouseHit.point - transform.position);
                lookTarget = mouseHit.point;
                hasInput = true;
            }
            
            Trace( "Final Look Direction", lookDirection.Flatten());
            Trace( "Target Look Position", lookTarget.Flatten());

            _currentLookInput = new LookInput()
            {
                LookDirection = lookDirection,
                HasInput = hasInput,
                TargetLookPosition = lookTarget
            };
            
            LookInputUpdated?.Invoke(_currentLookInput);
        }

        Vector3 GetDefaultLookDirection()
        {
            return _currentMovementInput.HasInput 
                ? _currentMovementInput.MovementVector.normalized 
                : _currentLookInput.LookDirection;
        }

        void ToggleSprint(InputAction.CallbackContext context)
        {
            Log("Enabling sprint");
            SprintEnabled = !SprintEnabled;
        }

        public event Action Jump;
    }
}