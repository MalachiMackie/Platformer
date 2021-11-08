using UnityEngine.EventSystems;

namespace Core.MessageTargets.PlayerEvents
{
    public interface IPlayerReachedFinishEventTarget : IEventSystemHandler
    {
        public void PlayerReachedFinish();
    }
}