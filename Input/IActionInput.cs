using System;

namespace TopDownCharacter
{
    public interface IActionInput
    {
        public event Action Jump;
        bool JumpedThisFrame { get; }
    }
}