using UnityEngine.EventSystems;

namespace Core.MessageTargets.LevelEvents
{
    public interface INextLevelStartedEventTarget : IEventSystemHandler
    {
        void NextLevelStarted();
    }
}