using UnityEngine.EventSystems;

namespace Core.MessageTargets.PlayerEvents
{
    public interface IRequestCollectedCollectablesEventTarget : IEventSystemHandler
    {
        void RequestCollectedCollectables(out int collectedCollectables);
    }
}