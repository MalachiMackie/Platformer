using Core;
using Core.MessageTargets;
using Core.MessageTargets.PlayerEvents;
using Shared;
using UnityEngine;

namespace Gameplay.Player
{
    public class PlayerBehaviour : MonoBehaviour, IRequireSetup
    {
        [SerializeField] private int initialHealth = 1;
        private int _health;

        public void Setup()
        {
            _health = initialHealth;
        }

        public void FellOutOfWorld()
        {
            Died();
        }

        public void Damage(int damagePoints)
        {
            _health -= damagePoints;
            if (_health <= 0)
            {
                Died();
            }
        }

        private void Died()
        {
            Helpers.DispatchEvent<IPlayerDiedEventTarget>(x => x.PlayerDied());
        }
    }
}