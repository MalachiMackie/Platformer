using System;
using Gameplay;
using UnityEditor;
using UnityEngine;

namespace Core
{
    public class EnemySpawn : MonoBehaviour
    {
        public Enemy.EnemyType type;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawIcon(transform.position, "Enemy");
        }
    }
}
