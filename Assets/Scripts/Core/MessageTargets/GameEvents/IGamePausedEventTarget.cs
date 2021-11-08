using UnityEngine.EventSystems;

namespace Core.MessageTargets.GameEvents
{
    public interface IGamePausedEventTarget : IEventSystemHandler
    {
        void GamePaused();
    }
}