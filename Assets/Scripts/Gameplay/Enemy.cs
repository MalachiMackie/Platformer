using System;
using Gameplay.Player;
using Shared;
using UnityEngine;

namespace Gameplay
{
    [RequireComponent(typeof(Collider2D))]
    public class Enemy : MonoBehaviour
    {
        private void OnCollisionEnter2D(Collision2D col)
        {
            if (col.gameObject.CompareTag(Tags.Player))
            {
                var playerBehaviour = Helpers.AssertGameObjectHasComponent<PlayerBehaviour>(col.gameObject);
                playerBehaviour.Damage(1);
            }
        }
    }
}