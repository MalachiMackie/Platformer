using UnityEngine.EventSystems;

namespace Core.MessageTargets.LevelEvents
{
    public interface IRequestAnyMoreLevelsEventTarget : IEventSystemHandler
    {
        void RequestAnyMoreLevels(out bool anyMoreLevels);
    }
}