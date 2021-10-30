using Shared;
using UnityEngine;

namespace Core.Managers
{
    public class LevelManager : Singleton<LevelManager>
    {
        public void FinishLevel()
        {
            Debug.Log("Well Done!");
            Helpers.Quit();
        }
    }
}