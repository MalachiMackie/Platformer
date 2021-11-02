﻿using System;
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
        [SerializeField] private EnemyTypeAndPrefabTuple[] enemyPrefabs;
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private GameObject collectablePrefab;
        
        private PlayerBehaviour _playerBehaviour;
        private Transform _playerTransform;
        private Transform _startPoint;
        private readonly HashSet<int> _collectedCollectables = new();
        private int _totalCollectables;

        public event EventHandler<int> PlayerPickedUpCollectable;

        public int GetTotalCollectables() => _totalCollectables;
        public int GetTotalCollectedCollectables() => _collectedCollectables.Count;

        public void FinishLevel()
        {
            _playerBehaviour.ReachedGoal();
            Debug.Log("Well Done!");
            Helpers.Quit();
        }

        public void OnPlayerPickedUpCollectable(int collectableNum)
        {
            _collectedCollectables.Add(collectableNum);
            PlayerPickedUpCollectable?.Invoke(this, _collectedCollectables.Count);
        }

        public void PlayerDied()
        {
            GameManager.Instance.PlayerDied();
            if (GameManager.Instance.PlayerHasMoreLives())
            {
                ResetLevel();
                StartCoroutine(Helpers.DoNextFrame(this, x => x.SetupLevel()));
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
            
            EnsureKillZoneExists();
            EnsureGoalExists();

            SpawnEnemies(); 
            SpawnCollectables();

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
    }
}