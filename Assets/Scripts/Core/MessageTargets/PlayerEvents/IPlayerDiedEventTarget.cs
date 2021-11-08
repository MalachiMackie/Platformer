using UnityEngine.EventSystems;

namespace Core.MessageTargets.PlayerEvents
{
    public interface IPlayerDiedEventTarget : IEventSystemHandler
    {
        void PlayerDied();
    }
}