using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Gameplay.Player;
using Shared;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gameplay
{
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class Enemy : MonoBehaviour, IRequireSetup
    {
        [SerializeField] private float jumpTimeout = 3f;
        [SerializeField] private float jumpDistance = 5f;
        [SerializeField] private float viewDistance = 12f;
        [SerializeField] private float movementForce = 10f;
        [SerializeField] private float maxMovementSpeed = 5f;
        [SerializeField] private float jumpForce = 10f;
        [SerializeField] private float chanceToJump = 1.2f;
        
        private Transform _playerTransform;
        private Rigidbody2D _rigidbody;
        private Collider2D _collider;

        private float _timeSinceJumped;
        private float _distanceToPlayer;
        private bool _onGround;
        private EnemyType _type = EnemyType.Normal;

        public void Setup()
        {
            _timeSinceJumped = jumpTimeout;
        }

        private void Start()
        {
            _playerTransform = GameObject.FindWithTag(Tags.Player)?.transform;
            Helpers.AssertIsNotNullOrQuit(_playerTransform, "Could not find player in scene");

            _rigidbody = Helpers.AssertGameObjectHasComponent<Rigidbody2D>(gameObject);
            _collider = Helpers.AssertGameObjectHasComponent<Collider2D>(gameObject);
        }
        
        private void OnCollisionEnter2D(Collision2D col)
        {
            if (!col.gameObject.CompareTag(Tags.Player)) return;
            var playerBehaviour = Helpers.AssertGameObjectHasComponent<PlayerBehaviour>(col.gameObject);
            playerBehaviour.Damage(1);
        }

        private void Update()
        {
            _timeSinceJumped += Time.deltaTime;
        }

        private void FixedUpdate()
        {
            _distanceToPlayer = _playerTransform.position.x - transform.position.x;
            CheckForGroundCollisions();
            FollowPlayer();
            TryJump();
            RestrictSpeed();
        }
        
        private void CheckForGroundCollisions()
        {
            var results = new List<Collider2D>();
            _collider.OverlapCollider(new ContactFilter2D().NoFilter(), results);
            _onGround = results.Any(x => x.gameObject.CompareTag(Tags.Ground));
        }

        private void FollowPlayer()
        {
            if (Math.Abs(_distanceToPlayer) < viewDistance)
            {
                _rigidbody.AddForce(new Vector2(Math.Sign(_distanceToPlayer), 0) * movementForce);
            }
        }

        private void RestrictSpeed()
        {
            var rbVelocity = _rigidbody.velocity;
            if (Math.Abs(rbVelocity.x) > maxMovementSpeed)
            {
                rbVelocity.x = maxMovementSpeed * Math.Sign(rbVelocity.x);
                _rigidbody.velocity = rbVelocity;
            }
        }

        private void TryJump()
        {
            if (jumpDistance < Math.Abs(_distanceToPlayer)  || _timeSinceJumped < jumpTimeout || !_onGround)
            {
                return;
            }

            var random = Random.Range(0f, 1f);

            if (random < chanceToJump * Time.fixedDeltaTime)
            {
                _rigidbody.AddForce(new Vector2(0, 1) * jumpForce, ForceMode2D.Impulse);
                _timeSinceJumped = 0f;                
            }
        }

        public enum EnemyType
        {
            Normal
        }
    }
}