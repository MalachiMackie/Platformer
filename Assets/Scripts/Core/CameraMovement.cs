using System;
using System.Collections;
using System.Collections.Generic;
using Gameplay;
using Shared;
using UnityEngine;

namespace Core
{
    public class CameraMovement : MonoBehaviour
    {
        [SerializeField] private float smoothTime;
        [SerializeField] private Vector2 followPlayerOffset;

        private Transform _player;

        private void Start()
        {
            _player = GameObject.FindWithTag(Tags.Player)?.transform;
            Helpers.AssertIsNotNullOrQuit(_player, "Could not find player in scene");
        }

        private Vector3 _currentVelocity;

        // Update is called once per frame
        void FixedUpdate()
        {
            FollowPlayer();
        }

        void FollowPlayer()
        {
            var localTransform = transform;
            var transformPosition = localTransform.position;
            var newPosition = Vector3.SmoothDamp(transformPosition, _player.position, ref _currentVelocity, smoothTime);
            transform.position = new Vector3(newPosition.x + followPlayerOffset.x, newPosition.y + followPlayerOffset.y, transformPosition.z);
        }
    }
}