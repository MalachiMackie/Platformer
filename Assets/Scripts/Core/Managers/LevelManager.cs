using System.Linq;
using Gameplay;
using Gameplay.Player;
using Shared;
using UnityEngine;

namespace Core.Managers
{
    public class LevelManager : Singleton<LevelManager>
    {
        private PlayerBehaviour _playerBehaviour;
        private Transform _playerTransform;
        private Transform _startPoint;
        
        public void FinishLevel()
        {
            _playerBehaviour.ReachedGoal();
            Debug.Log("Well Done!");
            Helpers.Quit();
        }

        public void PlayerDied()
        {
            Helpers.Quit();
        }

        private void Start()
        {
            _playerBehaviour = GetPlayerBehaviour();
            _playerTransform = _playerBehaviour.transform;
            _startPoint = GetStartPoint();
            EnsureLevelSetup();

            _playerTransform.position = _startPoint.position;
        }

        private void EnsureLevelSetup()
        {
            EnsureKillZoneExists();
            EnsureGoalExists();
        }

        private Transform GetStartPoint()
        {
            var startPoint = GameObject.FindWithTag(Tags.StartPoint);
            Helpers.AssertIsNotNullOrQuit(startPoint, "Could not find start point is scene");
            return startPoint.transform;
        }

        private PlayerBehaviour GetPlayerBehaviour()
        {
            var player = GameObject.FindWithTag(Tags.Player);
            Helpers.AssertIsNotNullOrQuit(player, "Could not find player in scene");
            return Helpers.AssertGameObjectHasComponent<PlayerBehaviour>(player);
        }

        private void EnsureKillZoneExists()
        {
            var killZones = GameObject.FindGameObjectsWithTag(Tags.KillZone);
            Helpers.AssertIsTrueOrQuit(killZones.Any(), "Could not find any kill zones in scene");
            foreach (var killZone in killZones)
            {
                Helpers.AssertGameObjectHasComponent<KillZone>(killZone);
            }
        }

        private void EnsureGoalExists()
        {
            var goals = GameObject.FindGameObjectsWithTag(Tags.Goal);
            Helpers.AssertIsTrueOrQuit(goals.Any(), "Could not find any goals in the scene");
            foreach (var goal in goals)
            {
                Helpers.AssertGameObjectHasComponent<Goal>(goal);
            }
        }
    }
}