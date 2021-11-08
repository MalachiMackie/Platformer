using UnityEngine.EventSystems;

namespace Core.MessageTargets.PlayerEvents
{
    public interface IPlayerLivesChangedEventTarget : IEventSystemHandler
    {
        void PlayerLivesChanged(int playerLives);
    }
}