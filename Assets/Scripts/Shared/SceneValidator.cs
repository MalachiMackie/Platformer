using System;
using System.Collections.Generic;
using System.Linq;
using Core.Managers;
using Core.Spawns;
using Gameplay;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Shared
{
    [ExecuteInEditMode]
    public class SceneValidator : MonoBehaviour
    {
        private void Start()
        {
            if (!CompareTag("EditorOnly"))
            {
                Debug.LogError("Scene Validator must be tagged with 'EditorOnly'");
                return;
            }
            
            var isValid = ValidateGoalExists();
            isValid = ValidateKillZoneExist() && isValid;
            isValid = ValidateStartPointExists() && isValid;
            isValid = ValidateCollectablesAreUnique() && isValid;
            isValid = ValidateAllEventTargetsHaveImplementations() && isValid;
            isValid = ValidateSceneHasLevelManager() && isValid;
            isValid = ValidateSceneDoesNotHaveGameManager() && isValid;

            if (!isValid && EditorApplication.isPlaying)
            {
                Helpers.Quit();
            }
        }

        private bool ValidateSceneHasLevelManager()
        {
            if (FindObjectOfType<LevelManager>() is null)
            {
                Debug.LogError("Could not find level manager in scene");
                return false;
            }

            return true;
        }

        private bool ValidateSceneDoesNotHaveGameManager()
        {
            if (!EditorApplication.isPlaying && FindObjectOfType<GameManager>() is not null)
            {
                Debug.LogError("Scene should not have a GameManager");
                return false;
            }

            return true;
        }
        
        private bool ValidateGoalExists()
        {
            var succeed = true;
            var goals = GameObject.FindGameObjectsWithTag(Tags.Goal);
            if (!goals.Any())
            {
                succeed = false;
                Debug.LogError("No goals found in scene");
            }

            foreach (var goal in goals.Where(x => !x.TryGetComponent<Goal>(out _)))
            {
                succeed = false;
                Debug.LogError($"Goal '{goal.name}' does not have a goal script component");
            }

            return succeed;
        }

        private bool ValidateKillZoneExist()
        {
            var succeed = true;
            var killZones = GameObject.FindGameObjectsWithTag(Tags.KillZone);
            if (!killZones.Any())
            {
                succeed = false;
                Debug.LogError("No kill zones found in scene");
            }

            foreach (var killZone in killZones.Where(x => !x.TryGetComponent<KillZone>(out _)))
            {
                succeed = false;
                Debug.LogError($"Kill Zone '{killZone.name}' does not have kill zone script component");
            }

            return succeed;
        }

        private bool ValidateStartPointExists()
        {
            var startPoint = GameObject.FindWithTag(Tags.StartPoint);
            if (startPoint is not null) return true;
            Debug.LogError("No start point found in scene");
            return false;

        }

        private bool ValidateCollectablesAreUnique()
        {
            var collectableSpawns = FindObjectsOfType<CollectableSpawn>();
            var distinctCollectableSpawnsCount =
                collectableSpawns.Select(x => x.GetCollectableNum()).Distinct().Count();
            if (distinctCollectableSpawnsCount == collectableSpawns.Length) return true;
            Debug.LogError("Each collectable in the scene must have a unique number");
            return false;

        }
        
        
        private bool ValidateAllEventTargetsHaveImplementations()
        {
            var handlers = new List<Type>();
            var behaviours = new List<Type>();
            var allTypes = typeof(Helpers).Assembly.GetTypes();

            var monoBehaviourType = typeof(MonoBehaviour);
            var eventHandlerType = typeof(IEventSystemHandler);

            foreach (var type in allTypes)
            {
                if (type.IsSubclassOf(monoBehaviourType))
                {
                    behaviours.Add(type);
                    continue;
                }

                if (type.IsInterface && type.GetInterfaces().Any(x => x == eventHandlerType))
                {
                    handlers.Add(type);
                }
            }

            var missingImplementations = handlers
                .Where(eventSystemHandler => !behaviours.Any(eventSystemHandler.IsAssignableFrom)).ToList();

            if (!missingImplementations.Any()) return true;
            
            Debug.LogError(
                $"The following event targets are missing implementations: {string.Join(";", missingImplementations.Select(x => x.Name))}");
            return false;

        }
        
    }
}