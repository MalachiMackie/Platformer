using System;
using Core.Managers;
using Shared;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI
{
    public class Hud : MonoBehaviour, IRequireSetup, IRequireTareDown
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
            
            GameManager.Instance.PlayerLivesChanged += OnPlayerLivesChanged;
            LevelManager.Instance.PlayerPickedUpCollectable += OnPlayerPickedUpCollectable;
            LevelManager.Instance.LevelEnded += OnLevelEnded;
            LevelManager.Instance.LevelRestarted += OnLevelRestarted;
            LevelManager.Instance.NextLevelStarted += OnNextLevelStarted;
        }

        private void OnNextLevelStarted(object sender, EventArgs e)
        {
            gameObject.SetActive(true);
        }

        private void OnLevelRestarted(object sender, EventArgs e)
        {
            gameObject.SetActive(true);
        }

        private void OnLevelEnded(object sender, EventArgs e)
        {
            gameObject.SetActive(false);
        }

        private void OnPlayerPickedUpCollectable(object sender, int collected)
        {
            SetCollectables(collected, _totalCollectables);
        }

        private void OnPlayerLivesChanged(object sender, int lives)
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

        public void TareDown()
        {
            GameManager.Instance.PlayerLivesChanged -= OnPlayerLivesChanged;
            LevelManager.Instance.PlayerPickedUpCollectable -= OnPlayerPickedUpCollectable;
            LevelManager.Instance.LevelEnded -= OnLevelEnded;
            LevelManager.Instance.LevelRestarted -= OnLevelRestarted;
            LevelManager.Instance.NextLevelStarted -= OnNextLevelStarted;
        }
    }
}