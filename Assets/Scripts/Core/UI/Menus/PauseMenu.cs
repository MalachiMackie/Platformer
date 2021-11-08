using System;
using Core.Managers;
using Core.MessageTargets;
using Core.MessageTargets.GameEvents;
using Shared;
using UnityEngine;

namespace Core.UI.Menus
{
    public class PauseMenu : MonoBehaviour, IRequireSetup,
        IGamePausedEventTarget,
        IGameUnpausedEventTarget
    {
        public void Setup()
        {
            gameObject.SetActive(false);
        }

        public void Resume()
        {
            Helpers.DispatchEvent<IGameUnpauseRequestedEventTarget>(x => x.GameUnpauseRequested());
        }

        public void Quit()
        {            
            Helpers.DispatchEvent<IGameQuitRequestedEventTarget>(x => x.GameQuitRequested());
        }

        public void GamePaused()
        {
            gameObject.SetActive(true);
        }

        public void GameUnpaused()
        {
            gameObject.SetActive(false);
        }
    }
}