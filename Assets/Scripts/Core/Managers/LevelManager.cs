using System;
using System.Collections.Generic;
using System.Linq;
using Gameplay;
using Gameplay.Player;
using Shared;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Core.Managers
{
    [Serializable]
    public class EnemyTypeAndPrefabTuple
    {
        public Enemy.EnemyType enemyType;
        public Enemy enemyPrefab;
    }
    
    public class LevelManager : Singleton<LevelManager>
    {
        [SerializeField] private int playerLives = 3;
        [SerializeField] private EnemyTypeAndPrefabTuple[] enemyPrefabs;
        [SerializeField] private GameObject playerPrefab;
        
        private PlayerBehaviour _playerBehaviour;
        private Transform _playerTransform;
        private Transform _startPoint;

        private bool _paused;

        public event EventHandler GamePaused;
        public event EventHandler GameUnpaused;
        
        public void FinishLevel()
        {
            _playerBehaviour.ReachedGoal();
            Debug.Log("Well Done!");
            Helpers.Quit();
        }

        public void PlayerDied()
        {
            playerLives -= 1;
            if (playerLives <= 0)
            {
                Helpers.Quit();
                return;
            }

            ResetLevel();
        }

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

        private void Start()
        {
            SetupLevel();
        }

        private void SetupLevel()
        {
            _playerBehaviour = GetPlayerBehaviour();
            _playerTransform = _playerBehaviour.transform;
            _startPoint = GetStartPoint();
            
            EnsureKillZoneExists();
            EnsureGoalExists();

            SpawnEnemies(); 

            _playerTransform.position = _startPoint.position;

            foreach (var requiresSetup in FindObjectsOfType<Object>())
            {
                if (requiresSetup is IRequireSetup requireSetup)
                {
                    requireSetup.Setup();
                }
            }
            
        }

        private void ResetLevel()
        {
            foreach (var enemy in FindObjectsOfType<Enemy>())
            {
                Destroy(enemy.gameObject);
            }

            _playerTransform = null;
            _playerBehaviour = null;
            foreach (var player in FindObjectsOfType<PlayerBehaviour>())
            {
                Destroy(player.gameObject);
            }

            StartCoroutine(Helpers.DoNextFrame(this, x => x.SetupLevel()));
            
        }

        private void SpawnEnemies()
        {
            var enemyTypesCount = Enum.GetValues(typeof(Enemy.EnemyType)).Length;
            var enemyPrefabsCount = enemyPrefabs.Length;
            Helpers.AssertIsTrueOrQuit(enemyPrefabsCount == enemyTypesCount,
                $"Expected {enemyTypesCount} enemy prefabs, but got {enemyPrefabsCount}");
            Helpers.AssertIsTrueOrQuit(enemyPrefabs.Select(x => x.enemyType).Distinct().Count() == enemyTypesCount,
                "enemyPrefabs should only have one of each enemy type");
            var enemyTypesDictionary = enemyPrefabs.ToDictionary(x => x.enemyType);
            foreach (var enemySpawn in FindObjectsOfType<EnemySpawn>())
            {
                var spawnTransform = enemySpawn.transform;
                Instantiate(enemyTypesDictionary[enemySpawn.type].enemyPrefab, spawnTransform.position, spawnTransform.rotation);
            }
        }

        private Transform GetStartPoint()
        {
            var startPoint = GameObject.FindWithTag(Tags.StartPoint);
            Helpers.AssertIsNotNullOrQuit(startPoint, "Could not find start point is scene");
            return startPoint.transform;
        }

        private PlayerBehaviour GetPlayerBehaviour()
        {
            Helpers.AssertScriptFieldIsAssignedOrQuit(this, x => x.playerPrefab);
            var player = Instantiate(playerPrefab);
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