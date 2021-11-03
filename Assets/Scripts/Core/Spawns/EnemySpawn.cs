using Gameplay;
using UnityEngine;

namespace Core.Spawns
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
