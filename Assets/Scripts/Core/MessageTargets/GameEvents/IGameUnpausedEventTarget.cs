using UnityEngine.EventSystems;

namespace Core.MessageTargets.GameEvents
{
    public interface IGameUnpausedEventTarget : IEventSystemHandler
    {
        void GameUnpaused();
    }
}