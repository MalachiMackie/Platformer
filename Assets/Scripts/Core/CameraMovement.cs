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
        [SerializeField] private Transform player;
        [SerializeField] private float smoothTime;
        [SerializeField] private Vector2 followPlayerOffset;

        private void Awake()
        {
            Helpers.AssertScriptFieldIsAssignedOrQuit(this, x => x.player);
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
            var newPosition = Vector3.SmoothDamp(transformPosition, player.position, ref _currentVelocity, smoothTime);
            transform.position = new Vector3(newPosition.x + followPlayerOffset.x, newPosition.y + followPlayerOffset.y, transformPosition.z);
        }
    }
}