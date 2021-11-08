using UnityEngine.EventSystems;

namespace Core.MessageTargets.PlayerEvents
{
    public interface IPlayerCollectedCollectableEventTarget : IEventSystemHandler
    {
        void PlayerCollectedCollectable(int collectableNum);
    }
}