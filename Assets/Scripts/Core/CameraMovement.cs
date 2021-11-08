using Shared;
using UnityEngine;

namespace Core
{
    public class CameraMovement : MonoBehaviour, IRequireSetup
    {
        [SerializeField] private float smoothTime = 0.15f;
        [SerializeField] private Vector2 followPlayerOffset;

        private Transform _player;
        private Vector3 _currentVelocity;

        public void Setup()
        {
            _player = GameObject.FindWithTag(Tags.Player)?.transform;
            Helpers.AssertIsNotNullOrQuit(_player, "Could not find player in scene");   
        }

        private void FixedUpdate()
        {
            FollowPlayer();
        }

        private void FollowPlayer()
        {
            if (_player is null)
            {
                return;
            }
            var localTransform = transform;
            var transformPosition = localTransform.position;
            var newPosition = Vector3.SmoothDamp(transformPosition, _player.position, ref _currentVelocity, smoothTime);
            transform.position = new Vector3(newPosition.x + followPlayerOffset.x, newPosition.y + followPlayerOffset.y, transformPosition.z);
        }
    }
}