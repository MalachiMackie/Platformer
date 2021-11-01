using System;
using Core.Managers;
using UnityEngine;

namespace Core.Menus
{
    public class PauseMenu : MonoBehaviour, IRequireSetup
    {
        public void Setup()
        {
            LevelManager.Instance.GamePaused += OnGamePaused;
            LevelManager.Instance.GameUnpaused += OnGameUnpaused;
            gameObject.SetActive(false);
        }

        public void Resume()
        {
            LevelManager.Instance.Unpause();
        }

        public void Quit()
        {
            LevelManager.Instance.Quit();
        }

        private void OnGamePaused(object sender, EventArgs e)
        {
            gameObject.SetActive(true);
        }

        private void OnGameUnpaused(object sender, EventArgs e)
        {
            gameObject.SetActive(false);
        }
    }
}