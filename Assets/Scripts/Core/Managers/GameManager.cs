using System;
using System.Linq;
using Shared;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core.Managers
{
    public class GameManager : Singleton<GameManager>
    {
        
        public event EventHandler GamePaused;
        public event EventHandler GameUnpaused;
        public event EventHandler<int> PlayerLivesChanged;
        
        [SerializeField] private int playerLives = 3;


        private int _currentLevel;
        private int _lastLevel;
        private bool _paused;

        public void LevelFinished()
        {
            if (_currentLevel == _lastLevel)
            {
                Helpers.Quit();
                return;
            }

            _currentLevel++;
            SceneManager.LoadScene(_currentLevel);

        }        

        public void PlayerDied()
        {
            playerLives -= 1;
            PlayerLivesChanged?.Invoke(this, playerLives);

            if (!PlayerHasMoreLives())
            {
                Quit();
            }
        }

        public bool PlayerHasMoreLives()
        {
            return playerLives >= 1;
        }

        public int GetPlayerLives() => playerLives;
        

        public void TogglePause()
        {
            if (_paused)
            {
                Unpause();
            }
            else
            {
                Pause();
            }
        }

        private void Pause()
        {
            Time.timeScale = 0f;
            _paused = true;
            GamePaused?.Invoke(this, EventArgs.Empty);
        }

        public void Unpause()
        {
            Time.timeScale = 1f;
            _paused = false;
            GameUnpaused?.Invoke(this, EventArgs.Empty);
        }
        
        public void Quit()
        {
            Helpers.Quit();
        }

        private void Awake()
        {
            EnsureOnlyOneGameManager();
            GetLevelNumbers();
        }

        private void GetLevelNumbers()
        {
            _currentLevel = SceneManager.GetActiveScene().buildIndex;
            _lastLevel = SceneManager.sceneCountInBuildSettings - 1;
        }

        private void EnsureOnlyOneGameManager()
        {
            var gameManagers = FindObjectsOfType<GameManager>();
            if (gameManagers.Length == 1)
            {
                DontDestroyOnLoad(gameObject);
                SingletonInstance = this;
                return;
            }

            var isDontDestroyOnLoad = Helpers.IsDontDestroyOnLoad(gameObject);

            if (isDontDestroyOnLoad)
            {
                foreach (var manager in gameManagers.Where(x => x != this))
                {
                    Destroy(manager.gameObject);
                }
            }
            
            Helpers.AssertIsTrueOrQuit(gameManagers.Count(x => Helpers.IsDontDestroyOnLoad(x.gameObject)) == 1,
                "There was more than one GameManager in the scene, fix your shit!");

            if (!isDontDestroyOnLoad)
            {
                Destroy(gameObject);
            }
        }

    }
}