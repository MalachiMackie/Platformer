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

        void Awake()
        {
            Helpers.AssertScriptFieldIsAssignedOrQuit(this, x => x.player);
        }

        // Start is called before the first frame update
        void Start()
        {
        }

        private Vector3 _currentVelocity;

        // Update is called once per frame
        void FixedUpdate()
        {
            var newPosition = Vector3.SmoothDamp(transform.position, player.position, ref _currentVelocity, smoothTime);
            transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);
            // Debug.Log(_currentVelocity);
        }

        void FollowPlayer()
        {
            var localTransform = transform;
            var transformPosition = localTransform.position;
            // var position = player.transform.position;
            
            // Vector2.SmoothDamp(transformPosition, )
            var position = Vector2.Lerp(transformPosition, player.GetComponent<Rigidbody2D>().position, Time.deltaTime *2f);
            // var position = Vector2.Lerp(transformPosition, player.transform.position, 0.1f);
            transformPosition = new Vector3(position.x, position.y, transformPosition.z);
            localTransform.position = transformPosition;
        }
    }
}