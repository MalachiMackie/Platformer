using UnityEngine.EventSystems;

namespace Core.MessageTargets.LevelEvents
{
    public interface ILevelRestartedEventTarget : IEventSystemHandler
    {
        public void LevelRestarted();
    }
}