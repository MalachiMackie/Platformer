using UnityEngine.EventSystems;

namespace Core.MessageTargets.GameEvents
{
    public interface IGameQuitRequestedEventTarget : IEventSystemHandler
    {
        void GameQuitRequested();
    }
}