using UnityEngine.EventSystems;

namespace Core.MessageTargets.LevelEvents
{
    public interface INextLevelRequestEventTarget : IEventSystemHandler
    {
        void NextLevelRequested();
    }
}