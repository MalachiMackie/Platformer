using UnityEngine.EventSystems;

namespace Core.MessageTargets.PlayerEvents
{
    public interface ICollectedCollectablesChangedEventTarget : IEventSystemHandler
    {
        void CollectedCollectablesChanged(int collectedCollectables);
    }
}