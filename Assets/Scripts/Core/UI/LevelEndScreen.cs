using System;
using Core.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI
{
    public class LevelEndScreen : MonoBehaviour, IRequireSetup, IRequireTareDown
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
            LevelManager.Instance.LevelEnded += OnLevelEnded;
            LevelManager.Instance.NextLevelStarted += OnNextLevelStarted;
            LevelManager.Instance.LevelRestarted += OnLevelRestarted;
            gameObject.SetActive(false);
            if (!GameManager.Instance.HasNextLevel())
            {
                nextLevelButton.gameObject.SetActive(false);
                restartLevelButton.transform.position = button1Position.transform.position;
                quitButton.transform.position = button2Position.transform.position;
            }
        }

        public void NextLevel()
        {
            LevelManager.Instance.GoToNextLevel();
        }

        public void Quit()
        {
            GameManager.Instance.Quit();
        }

        public void RestartLevel()
        {
            LevelManager.Instance.RestartLevel();
        }

        private void OnLevelEnded(object seder, EventArgs e)
        {
            gameObject.SetActive(true);
            var totalCollectables = LevelManager.Instance.GetTotalCollectables();
            var collectedCollectables = LevelManager.Instance.GetTotalCollectedCollectables();
            collectablesText.text = string.Format(collectablesTextFormat, collectedCollectables, totalCollectables);
        }

        private void OnNextLevelStarted(object sender, EventArgs e)
        {
            gameObject.SetActive(false);
        }

        private void OnLevelRestarted(object sender, EventArgs e)
        {
            gameObject.SetActive(false);
        }

        public void TareDown()
        {
            LevelManager.Instance.LevelEnded -= OnLevelEnded;
            LevelManager.Instance.NextLevelStarted -= OnNextLevelStarted;
            LevelManager.Instance.LevelRestarted -= OnLevelRestarted;
        }
    }
}