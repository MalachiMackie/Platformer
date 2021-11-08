using System;
using Core.Managers;
using Core.MessageTargets;
using Core.MessageTargets.GameEvents;
using Core.MessageTargets.LevelEvents;
using Core.MessageTargets.PlayerEvents;
using Shared;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI
{
    public class LevelEndScreen :
        MonoBehaviour,
        IRequireSetup,
        IPlayerReachedFinishEventTarget,
        INextLevelStartedEventTarget,
        ILevelRestartedEventTarget
    {
        [SerializeField] private Text collectablesText;
        [SerializeField] private string collectablesTextFormat;
        [SerializeField] private Button nextLevelButton;
        [SerializeField] private Button restartLevelButton;
        [SerializeField] private Button quitButton;

        [SerializeField] private Transform button1Position;
        [SerializeField] private Transform button2Position;
        
        public void Setup()
        {
            gameObject.SetActive(false);
            var anyMoreLevels = false;
            Helpers.DispatchEvent<IRequestAnyMoreLevelsEventTarget>(x => x.RequestAnyMoreLevels(out anyMoreLevels));
            if (!anyMoreLevels)
            {
                nextLevelButton.gameObject.SetActive(false);
                restartLevelButton.transform.position = button1Position.transform.position;
                quitButton.transform.position = button2Position.transform.position;
            }
        }

        public void NextLevel()
        {
            Helpers.DispatchEvent<INextLevelRequestEventTarget>(x => x.NextLevelRequested());
        }

        public void Quit()
        {
            Helpers.DispatchEvent<IGameQuitRequestedEventTarget>(x => x.GameQuitRequested());
        }

        public void RestartLevel()
        {
            Helpers.DispatchEvent<IRestartLevelRequestEventTarget>(x => x.RestartLevelRequested());
        }

        public void PlayerReachedFinish()
        {
            gameObject.SetActive(true);
            var totalCollectables = 0;
            var collectedCollectables = 0;
            Helpers.DispatchEvent<IRequestTotalCollectablesEventTarget>(x =>
                x.RequestTotalCollectables(out totalCollectables));
            Helpers.DispatchEvent<IRequestCollectedCollectablesEventTarget>(x =>
                x.RequestCollectedCollectables(out collectedCollectables));
            collectablesText.text = string.Format(collectablesTextFormat, collectedCollectables, totalCollectables);
        }

        public void NextLevelStarted()
        {
            gameObject.SetActive(false);
        }

        public void LevelRestarted()
        {
            gameObject.SetActive(false);
        }
    }
}