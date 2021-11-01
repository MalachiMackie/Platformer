using Core.Managers;
using Shared;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI
{
    public class Hud : MonoBehaviour, IRequireSetup
    {
        [SerializeField] private Text playerLivesText;
        
        public void Setup()
        {
            Helpers.AssertScriptFieldIsAssignedOrQuit(this, x => x.playerLivesText);
            LevelManager.Instance.PlayerLivesChanged += OnPlayerLivesChanged;
        }

        private void OnPlayerLivesChanged(object sender, int e)
        {
            playerLivesText.text = e.ToString();
        }
    }
}