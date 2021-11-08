using System;
using System.Collections.Generic;
using System.Linq;
using Core.MessageTargets;
using Core.MessageTargets.LevelEvents;
using Core.MessageTargets.PlayerEvents;
using Core.Spawns;
using Gameplay;
using Gameplay.Player;
using Shared;
using Shared.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core.Managers
{
    [Serializable]
    public class EnemyTypeAndPrefabTuple
    {
        public Enemy.EnemyType enemyType;
        public Enemy enemyPrefab;
    }
    
    public class LevelManager : MonoBehaviour,
        IPlayerReachedFinishEventTarget,
        IPlayerCollectedCollectableEventTarget,
        INextLevelRequestEventTarget,
        IRestartLevelRequestEventTarget,
        IRequestTotalCollectablesEventTarget,
        IRequestCollectedCollectablesEventTarget,
        IPlayerLivesChangedEventTarget
    {
        [SerializeField] private EnemyTypeAndPrefabTuple[] enemyPrefabs;
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private GameObject collectablePrefab;
        
        private PlayerBehaviour _playerBehaviour;
        private Transform _playerTransform;
        private Transform _startPoint;
        private readonly HashSet<int> _collectedCollectables = new();
        private int _totalCollectables;
        private bool _isLevelFinished;


        // ReSharper disable once MemberCanBeMadeStatic.Global
        public void PlayerReachedFinish()
        {
            _isLevelFinished = true;
            DisableEnemies();
        }

        public void RequestTotalCollectables(out int totalCollectables)
        {
            totalCollectables = _totalCollectables;
        }

        public void NextLevelRequested()
        {
            if (_isLevelFinished)
            {
                Helpers.DispatchEvent<INextLevelStartedEventTarget>(x => x.NextLevelStarted());
            }            
        }

        public void RestartLevelRequested()
        {
            if (_isLevelFinished)
            {
                ResetLevel();
                Helpers.DispatchEvent<ILevelRestartedEventTarget>(x => x.LevelRestarted());
                this.DoNextFrame(x => x.SetupLevel());
            }
        }

        public void PlayerCollectedCollectable(int collectableNum)
        {
            _collectedCollectables.Add(collectableNum);
            Helpers.DispatchEvent<ICollectedCollectablesChangedEventTarget>(x =>
                x.CollectedCollectablesChanged(_collectedCollectables.Count));
        }

        public void PlayerLivesChanged(int lives)
        {
            if (lives <= 0) return;
            
            ResetLevel();
            
            this.DoNextFrame(x => x.SetupLevel());
        }

        public void RequestCollectedCollectables(out int collectedCollectables)
        {
            collectedCollectables = _collectedCollectables.Count;
        }

        private void Awake()
        {
            var foundGameManagerScene = false;
            var sceneCount = SceneManager.sceneCount;
            for (var i = 0; i < sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (scene.name == "GameManagerScene")
                {
                    foundGameManagerScene = true;
                    break;
                }
            }

            if (!foundGameManagerScene)
            {
                SceneManager.LoadScene("GameManagerScene", LoadSceneMode.Additive);
            }
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
            _isLevelFinished = false;
            
            EnsureKillZoneExists();
            EnsureGoalExists();

            SpawnEnemies(); 
            SpawnCollectables();

            _playerTransform.position = _startPoint.position;

            foreach (var requiresSetup in Helpers.FindWithInterface<IRequireSetup>())
            {
                requiresSetup.Setup();
            }

            Time.timeScale = 1f;
        }

        private void ResetLevel()
        {
            Time.timeScale = 0f;
            foreach (var enemy in FindObjectsOfType<Enemy>())
            {
                Destroy(enemy.gameObject);
            }

            foreach (var collectable in FindObjectsOfType<Collectable>())
            {
                Destroy(collectable.gameObject);
            }
            
            Destroy(_playerBehaviour.gameObject);

            _playerTransform = null;
            _playerBehaviour = null;
            
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

        private void SpawnCollectables()
        {
            Helpers.AssertScriptFieldIsAssignedOrQuit(this, x => x.collectablePrefab);
            var collectableSpawns = FindObjectsOfType<CollectableSpawn>();
            var distinctCollectableSpawnsCount =
                collectableSpawns.Select(x => x.GetCollectableNum()).Distinct().Count();
            Helpers.AssertIsTrueOrQuit(distinctCollectableSpawnsCount == collectableSpawns.Length, 
                "Each collectable spawn must have a unique number");
            foreach (var collectableSpawn in collectableSpawns)
            {
                if (_collectedCollectables.Contains(collectableSpawn.GetCollectableNum()))
                {
                    continue;
                }
                var spawnTransform = collectableSpawn.transform;
                var collectable = Instantiate(collectablePrefab, spawnTransform.position, spawnTransform.rotation);
                collectable.GetComponent<Collectable>().SetCollectableNum(collectableSpawn.GetCollectableNum());
            }

            _totalCollectables = collectableSpawns.Length;
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

        private void DisableEnemies()
        {
            var enemies = FindObjectsOfType<Enemy>();
            foreach (var enemy in enemies)
            {
                enemy.StopMoving();
            }
        }

        private void OnDisable()
        {
            foreach (var requiresTareDown in Helpers.FindWithInterface<IRequireTareDown>())
            {
                requiresTareDown.TareDown();
            }
        }
    }
}