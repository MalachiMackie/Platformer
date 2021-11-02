using System;
using Core.Managers;
using Shared;
using UnityEngine;

namespace Gameplay
{
    [RequireComponent(typeof(Collider2D))]
    public class Collectable : MonoBehaviour
    {
        [SerializeField] private int num;

        public void SetCollectableNum(int collectableNum)
        {
            num = collectableNum;
        }
        
        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.gameObject.CompareTag(Tags.Player))
            {
                LevelManager.Instance.OnPlayerPickedUpCollectable(num);
                Destroy(gameObject);
            }
        }
    }
}