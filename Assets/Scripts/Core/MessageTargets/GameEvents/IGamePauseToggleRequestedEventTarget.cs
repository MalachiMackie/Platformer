using UnityEngine.EventSystems;

namespace Core.MessageTargets.GameEvents
{
    public interface IGamePauseToggleRequestedEventTarget : IEventSystemHandler
    {
        void GamePauseToggleRequested();
    }
}