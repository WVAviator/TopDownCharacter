using System;
using UnityEngine;

namespace TopDownCharacter
{
    public interface IMovementInput
    {
        public event Action<MovementInput> MovementInputUpdated;
        public MovementInput CurrentMovementInput { get; }
        
        public event Action<LookInput> LookInputUpdated;
        public LookInput CurrentLookInput { get; }
        bool SprintEnabled { get; set; }
    }
}