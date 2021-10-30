using Core.Managers;
using Shared;
using UnityEngine;

namespace Gameplay.Player
{
    public class PlayerBehaviour : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D col)
        {
            switch (col.gameObject.tag)
            {
                case Tags.KillZone:
                {
                    transform.position = new Vector3(0, 0, 0);
                    break;
                }
                case Tags.Goal:
                {
                    LevelManager.Instance.FinishLevel();
                    break;
                }
            }
        }
    }
}