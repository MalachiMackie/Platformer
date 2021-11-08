using UnityEngine.EventSystems;

namespace Core.MessageTargets.PlayerEvents
{
    public interface IRequestPlayerLivesEventTarget : IEventSystemHandler
    {
        void RequestPlayerLives(out int lives);
    }
}