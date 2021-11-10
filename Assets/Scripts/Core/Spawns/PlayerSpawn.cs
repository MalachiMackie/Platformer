using UnityEngine;

namespace Core.Spawns
{
    public class PlayerSpawn : MonoBehaviour
    {
        private void OnDrawGizmos()
        {
            Gizmos.DrawIcon(transform.position, "Start");
        }
    }
}