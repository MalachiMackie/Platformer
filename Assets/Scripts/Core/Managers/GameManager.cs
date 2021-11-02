using System;
using System.Linq;
using Shared;
using UnityEngine;

namespace Core.Managers
{
    public class GameManager : Singleton<GameManager>
    {
        
        [SerializeField] private int playerLives = 3;

        private bool _paused;
        public event EventHandler GamePaused;
        public event EventHandler GameUnpaused;
        public event EventHandler<int> PlayerLivesChanged;

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

        public void Pause()
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
        }

        private void EnsureOnlyOneGameManager()
        {
            if (Helpers.IsDontDestroyOnLoad(gameObject))
            {
                return;
            }
            
            var gameManagers = FindObjectsOfType<GameManager>();
            if (gameManagers.Length == 1)
            {
                DontDestroyOnLoad(gameObject);
                return;
            }

            var foundDontDestroyOnLoad = false;
            
            foreach (var gameManager in gameManagers)
            {
                if (!foundDontDestroyOnLoad && Helpers.IsDontDestroyOnLoad(gameManager.gameObject))
                {
                    foundDontDestroyOnLoad = true;
                    continue;
                }

                if (gameManager != this)
                {
                    Debug.Log($"Destroyed {gameManager.gameObject.name}");
                    Destroy(gameManager.gameObject);
                }
            }
            DontDestroyOnLoad(gameObject);
            SingletonInstance = this;
        }

    }
}