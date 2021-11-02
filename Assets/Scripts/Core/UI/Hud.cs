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
            OnPlayerPickedUpCollectable(this, LevelManager.Instance.GetTotalCollectedCollectables());
            
            LevelManager.Instance.PlayerLivesChanged += OnPlayerLivesChanged;
            LevelManager.Instance.PlayerPickedUpCollectable += OnPlayerPickedUpCollectable;
        }

        private void OnPlayerPickedUpCollectable(object sender, int e)
        {
            collectablesText.text = string.Format(collectablesTextFormat, e, _totalCollectables);
        }

        private void OnPlayerLivesChanged(object sender, int e)
        {
            playerLivesText.text = string.Format(playerLivesTextFormat, e);
        }
    }
}