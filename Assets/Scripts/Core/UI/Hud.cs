using Core.Managers;
using Shared;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI
{
    public class Hud : MonoBehaviour, IRequireSetup
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

            _totalCollectables = LevelManager.Instance.GetTotalCollectables();
            SetCollectables(LevelManager.Instance.GetTotalCollectedCollectables(), _totalCollectables);
            SetPlayerLives(GameManager.Instance.GetPlayerLives());
            
            GameManager.Instance.PlayerLivesChanged += (_, lives) => SetPlayerLives(lives);
            LevelManager.Instance.PlayerPickedUpCollectable += (_, collected) => SetCollectables(collected, _totalCollectables);
        }

        private void SetCollectables(int collected, int total)
        {
            collectablesText.text = string.Format(collectablesTextFormat, collected, total);
        }

        private void SetPlayerLives(int lives)
        {
            playerLivesText.text = string.Format(playerLivesTextFormat, lives);
        }
    }
}