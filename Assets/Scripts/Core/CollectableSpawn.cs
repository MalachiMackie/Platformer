﻿using UnityEngine;

namespace Core
{
    public class CollectableSpawn : MonoBehaviour
    {
        [SerializeField] private int collectableNum;

        public int GetCollectableNum() => collectableNum;
        
        private void OnDrawGizmos()
        {
            Gizmos.DrawIcon(transform.position, "Collectable");
        }
    }
}