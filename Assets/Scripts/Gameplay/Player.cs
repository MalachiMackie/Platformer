using System;
using System.Collections.Generic;
using System.Linq;
using Shared;
using UnityEngine;

namespace Gameplay
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class Player : MonoBehaviour
    {
        [SerializeField] private float movementForce = 3f;
        [SerializeField] private float jumpForce = 3f;
        [SerializeField] private float maxSpeed = 5f;

        private Rigidbody2D _rigidBody;
        private Collider2D _collider;

        private bool _onGround;

        private void Awake()
        {
            _rigidBody = GetComponent<Rigidbody2D>();
            _collider = GetComponent<Collider2D>();
            Helpers.AssertIsNotNullOrQuit(_rigidBody, "Could not find rigidbody2d component on player");
            Helpers.AssertIsNotNullOrQuit(_collider, "Could not find collider2d component on player");
        }

        private void Start()
        {
            CheckForGroundCollisions();
        }

        private void Update()
        {
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
            var isLeftPressed = Input.GetKey(KeyCode.A);
            var isRightPressed = Input.GetKey(KeyCode.D);
            if (isLeftPressed ^ isRightPressed)
            {
                var direction = isLeftPressed ? -1f : 1f;
                _rigidBody.AddForce(new Vector2(direction, 0) * movementForce);
            }

            var rbVelocity = _rigidBody.velocity;

            if (Math.Abs(rbVelocity.x) > maxSpeed)
            {
                _rigidBody.velocity = new Vector2(maxSpeed * Math.Sign(rbVelocity.x), rbVelocity.y);
            }   
        }

        private void DoJump()
        {
            if (Input.GetKeyDown(KeyCode.Space) && _onGround)
            {
                _rigidBody.AddForce(new Vector2(0, 1) * jumpForce, ForceMode2D.Impulse);
            }
        }
    }
}