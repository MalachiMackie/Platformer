using UnityEngine.EventSystems;

namespace Core.MessageTargets.GameEvents
{
    public interface IGameUnpauseRequestedEventTarget : IEventSystemHandler
    {
        void GameUnpauseRequested();
    }
}