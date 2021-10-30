using System;
using Core.Managers;
using Gameplay.Player;
using Shared;
using UnityEngine;

namespace Gameplay
{
    [RequireComponent(typeof(Collider2D))]
    public class Goal : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D col)
        {
            if (!col.CompareTag(Tags.Player))
            {
                return;
            }

            LevelManager.Instance.FinishLevel();
        }
    }
}