using UnityEngine.EventSystems;

namespace Core.MessageTargets.LevelEvents
{
    public interface IRestartLevelRequestEventTarget : IEventSystemHandler
    {
        void RestartLevelRequested();
    }
}