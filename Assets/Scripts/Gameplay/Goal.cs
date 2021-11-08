using Core.Managers;
using Core.MessageTargets;
using Core.MessageTargets.PlayerEvents;
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
            
            Helpers.DispatchEvent<IPlayerReachedFinishEventTarget>(x => x.PlayerReachedFinish());
        }
    }
}