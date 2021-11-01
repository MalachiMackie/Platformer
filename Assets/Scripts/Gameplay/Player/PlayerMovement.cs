using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Shared;
using UnityEngine;

namespace Gameplay.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(PlayerInputManager))]
    public class PlayerMovement : MonoBehaviour, IRequireSetup
    {
        [SerializeField] private float movementForce = 50f;
        [SerializeField] private float jumpForce = 30f;
        [SerializeField] private float maxSpeed = 8f;
        [SerializeField] private float earlyJumpForgiveness = 0.1f;

        private Rigidbody2D _rigidBody;
        private Collider2D _collider;
        private PlayerInputManager _inputManager;

        private bool _onGround;
        private float? _timeSinceJumpPressed;

        public void Setup()
        {
            _rigidBody = GetComponent<Rigidbody2D>();
            _collider = GetComponent<Collider2D>();
            _inputManager = GetComponent<PlayerInputManager>();
            Helpers.AssertIsNotNullOrQuit(_rigidBody, "Could not find rigidbody2d component on player");
            Helpers.AssertIsNotNullOrQuit(_collider, "Could not find collider2d component on player");
            Helpers.AssertIsNotNullOrQuit(_inputManager, "Could not find input manager component on player");

            _timeSinceJumpPressed = earlyJumpForgiveness;
            CheckForGroundCollisions();
        }

        private void Update()
        {
            if (_timeSinceJumpPressed.HasValue)
            {
                _timeSinceJumpPressed += Time.deltaTime;
            }
            DoJump();
        }

        private void FixedUpdate()
        {
            CheckForGroundCollisions();
            DoMovement();
        }

        private void CheckForGroundCollisions()
        {
            var results = new List<Collider2D>();
            _collider.OverlapCollider(new ContactFilter2D().NoFilter(), results);
            _onGround = results.Any(x => x.gameObject.CompareTag(Tags.Ground));
        }


        private void DoMovement()
        {
            var movementInput = _inputManager.GetMoveInput();
            if (Math.Abs(movementInput) > 0.1f)
            {
                _rigidBody.AddForce(new Vector2(Math.Sign(movementInput), 0) * movementForce);
            }

            var rbVelocity = _rigidBody.velocity;

            if (Math.Abs(rbVelocity.x) > maxSpeed)
            {
                _rigidBody.velocity = new Vector2(maxSpeed * Math.Sign(rbVelocity.x), rbVelocity.y);
            }
        }

        /// <summary>
        /// Tries to jump if the jump button was pressed this frame.
        /// If the player tries to jump just before they land (less that <see cref="earlyJumpForgiveness"/> seconds),
        /// we make them jump when they do land
        /// </summary>
        private void DoJump()
        {
            var jumpedThisFrame = _inputManager.WasJumpPressedThisFrame();

            if (jumpedThisFrame && !_onGround)
            {
                _timeSinceJumpPressed = 0f;
            }

            if ((jumpedThisFrame || _timeSinceJumpPressed < earlyJumpForgiveness) && _onGround)
            {
                _rigidBody.AddForce(new Vector2(0, 1) * jumpForce, ForceMode2D.Impulse);
                _timeSinceJumpPressed = null;
            }
        }
    }
}