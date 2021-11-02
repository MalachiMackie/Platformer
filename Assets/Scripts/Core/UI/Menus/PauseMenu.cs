using System;
using Core.Managers;
using UnityEngine;

namespace Core.UI.Menus
{
    public class PauseMenu : MonoBehaviour, IRequireSetup
    {
        public void Setup()
        {
            GameManager.Instance.GamePaused += OnGamePaused;
            GameManager.Instance.GameUnpaused += OnGameUnpaused;
            gameObject.SetActive(false);
        }

        public void Resume()
        {
            GameManager.Instance.Unpause();
        }

        public void Quit()
        {
            GameManager.Instance.Quit();
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