using System;
using Gameplay;
using UnityEditor;
using UnityEngine;

namespace Core
{
    public class EnemySpawn : MonoBehaviour
    {
        public Enemy.EnemyType type;

        
        
        // [DrawGizmo(GizmoType.Pickable)]
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            // Gizmos.DrawSphere(transform.position, 1f);
            Gizmos.DrawIcon(transform.position, "Enemy");
            // Gizmos.DrawSphere(component.transform.position, 1f);
            // DrawGizmo
            // Debug.Log("Hello World");
        }
    }
}
