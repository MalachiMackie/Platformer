using UnityEngine.EventSystems;

namespace Core.MessageTargets.PlayerEvents
{
    public interface IRequestTotalCollectablesEventTarget : IEventSystemHandler
    {
        void RequestTotalCollectables(out int totalCollectables);
    }
}