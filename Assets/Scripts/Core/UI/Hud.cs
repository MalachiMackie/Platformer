using Core.MessageTargets;
using Core.MessageTargets.LevelEvents;
using Core.MessageTargets.PlayerEvents;
using Shared;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI
{
    public class Hud : MonoBehaviour,
        IRequireSetup,
        ICollectedCollectablesChangedEventTarget,
        IPlayerReachedFinishEventTarget,
        INextLevelStartedEventTarget,
        ILevelRestartedEventTarget,
        IPlayerLivesChangedEventTarget
    {
        [SerializeField] private Text playerLivesText;
        [SerializeField] private string playerLivesTextFormat;

        [SerializeField] private Text collectablesText;
        [SerializeField] private string collectablesTextFormat;

        private int _totalCollectables;
        
        public void Setup()
        {
            Helpers.AssertScriptFieldIsAssignedOrQuit(this, x => x.playerLivesText);
            Helpers.AssertIsTrueOrQuit(!string.IsNullOrWhiteSpace(playerLivesTextFormat), "hud Player lives text format is not set");
            Helpers.AssertScriptFieldIsAssignedOrQuit(this, x => x.collectablesText);
            Helpers.AssertIsTrueOrQuit(!string.IsNullOrWhiteSpace(collectablesTextFormat), "hud Player lives text format is not set");

            Helpers.DispatchEvent<IRequestTotalCollectablesEventTarget>(x =>
                x.RequestTotalCollectables(out _totalCollectables));
            var collectedCollectables = 0;

            Helpers.DispatchEvent<IRequestCollectedCollectablesEventTarget>(x =>
                x.RequestCollectedCollectables(out collectedCollectables));
            
            SetCollectables(collectedCollectables, _totalCollectables);

            var playerLives = 0;
            Helpers.DispatchEvent<IRequestPlayerLivesEventTarget>(x => x.RequestPlayerLives(out playerLives));
            SetPlayerLives(playerLives);
        }

        public void NextLevelStarted()
        {
            gameObject.SetActive(true);
        }

        public void LevelRestarted()
        {
            gameObject.SetActive(true);
        }

        public void PlayerReachedFinish()
        {
            gameObject.SetActive(false);
        }
        
        public void PlayerLivesChanged(int lives)
        {
            SetPlayerLives(lives);
        }

        private void SetCollectables(int collected, int total)
        {
            collectablesText.text = string.Format(collectablesTextFormat, collected, total);
        }

        private void SetPlayerLives(int lives)
        {
            playerLivesText.text = string.Format(playerLivesTextFormat, lives);
        }

        public void CollectedCollectablesChanged(int collectedCollectables)
        {
            SetCollectables(collectedCollectables, _totalCollectables);
        }
    }
}