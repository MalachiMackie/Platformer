using Core.Managers;
using Core.MessageTargets;
using Core.MessageTargets.GameEvents;
using Shared;
using UnityEngine;
using UnityEngine.InputSystem;
// ReSharper disable UnusedMember.Global

namespace Gameplay.Player
{
    
    /// <summary>
    /// Manages the input callbacks for the player coming from the <see cref="PlayerInput"/> component.
    /// </summary>
    public class PlayerInputManager : MonoBehaviour
    {
        private bool _jumpDown;
        private float _moveInput;

        /// <summary>
        /// Because the <see cref="InputAction"/> shouldn't be held on to longer than the frame,
        /// <see cref="InputAction.WasPressedThisFrame()"/> can not be reliably used. Instead, _unsetJumpDown is set
        /// so that at the end of the frame in <see cref="LateUpdate"/>, _jumpDawn is set to false.
        /// This has the consequence that _jumpDown may be false when called from another script's LateUpdate function,
        /// even if it was pressed this frame. StartCoroutine can't be used to unset it the next frame as coroutines
        /// get processed after the Update methods
        /// </summary>
        private bool _unsetJumpDown;

        public bool WasJumpPressedThisFrame()
        {
            return _jumpDown;
        }

        public float GetMoveInput()
        {
            return _moveInput;
        }
        
        public void MoveInputCallback(InputAction.CallbackContext ctx)
        {
            _moveInput = ctx.ReadValue<float>();
        }

        public void JumpInputCallback(InputAction.CallbackContext ctx)
        {
            _jumpDown = ctx.action.WasPressedThisFrame();
            _unsetJumpDown = true;
        }

        public void PauseInputCallback(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
            {
                Helpers.DispatchEvent<IGamePauseToggleRequestedEventTarget>(x => x.GamePauseToggleRequested());
            }
        }

        private void LateUpdate()
        {
            if (_unsetJumpDown)
            {
                _jumpDown = false;
                _unsetJumpDown = false;
            }
        }
    }
}