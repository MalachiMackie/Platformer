using System;
using Core.MessageTargets;
using Core.MessageTargets.GameEvents;
using Core.MessageTargets.LevelEvents;
using Core.MessageTargets.PlayerEvents;
using Shared;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core.Managers
{
    public class GameManager : MonoBehaviour,
        IRequestPlayerLivesEventTarget,
        IGameUnpauseRequestedEventTarget,
        IGameQuitRequestedEventTarget,
        IPlayerDiedEventTarget,
        INextLevelStartedEventTarget,
        IRequestAnyMoreLevelsEventTarget,
        IGamePauseToggleRequestedEventTarget
    {        
        [SerializeField] private int playerLives = 3;

        private int _currentLevel;
        private int _lastLevel;
        private bool _paused;

        public void NextLevelStarted()
        {
            if (_currentLevel == _lastLevel)
            {
                Helpers.Quit();
                return;
            }

            SceneManager.UnloadSceneAsync(_currentLevel);
            _currentLevel++;
            SceneManager.LoadScene(_currentLevel, LoadSceneMode.Additive);
        }

        public void RequestAnyMoreLevels(out bool anyMoreLevels)
        {
            anyMoreLevels = _currentLevel != _lastLevel;
        }

        public void PlayerDied()
        {
            playerLives -= 1;
            Helpers.DispatchEvent<IPlayerLivesChangedEventTarget>(x => x.PlayerLivesChanged(playerLives));

            if (playerLives <= 0)
            {
                Quit();
            }
        }

        public void RequestPlayerLives(out int outPlayerLives)
        {
            outPlayerLives = playerLives;
        }

        public void GameUnpauseRequested()
        {
            if (_paused)
            {
                Unpause();                
            }
        }

        public void GameQuitRequested()
        {
            Quit();
        }
        
        public void GamePauseToggleRequested()
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
            Helpers.DispatchEvent<IGamePausedEventTarget>(x => x.GamePaused());
        }

        private void Unpause()
        {
            Time.timeScale = 1f;
            _paused = false;
            Helpers.DispatchEvent<IGameUnpausedEventTarget>(x => x.GameUnpaused());
        }
        
        private static void Quit()
        {
            Helpers.Quit();
        }

        private void Awake()
        {
            Helpers.EnsureAllEventTargetsHaveImplementations();
            GetLevelNumbers();
        }

        private void GetLevelNumbers()
        {
            var sceneCount = SceneManager.sceneCount;
            var foundCurrentLevel = false;
            for (var i = 0; i < sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (scene.name == "GameManagerScene") continue;
                
                _currentLevel = scene.buildIndex;
                foundCurrentLevel = true;
                break;
            }

            if (!foundCurrentLevel)
            {
                throw new InvalidOperationException("GameManagerScene can not be loaded by itself");
            }
            
            _lastLevel = SceneManager.sceneCountInBuildSettings - 2;
        }

    }
}