using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class FoodManager : MonoBehaviour
{
    [SerializeField]
    private List<Food> foods;

    private Dictionary<string, Food> spawnedFoods = new Dictionary<string, Food>();
    private Dictionary<string, ImageTargetBehaviour> imageTargets = new Dictionary<string, ImageTargetBehaviour>();

    private void Awake()
    {
        // Register all Image Targets in the scene
        foreach (var observer in FindObjectsByType<ImageTargetBehaviour>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID))
        {
            string targetName = observer.TargetName;
            imageTargets[targetName] = observer;

            observer.OnTargetStatusChanged += (ObserverBehaviour behaviour, TargetStatus status) =>
            {
                HandleTargetStatusChanged(behaviour, status);
            };
        }

        foreach (Food item in foods)
        {
            Food food = Instantiate(item, imageTargets[item.TargetID].transform);
            food.gameObject.SetActive(false);
            spawnedFoods[item.TargetID] = food;
        }

    }

    private void HandleTargetStatusChanged(ObserverBehaviour behaviour, TargetStatus status)
    {
        string targetName = behaviour.TargetName;
        if (!spawnedFoods.ContainsKey(targetName))
            return;
        Food food = spawnedFoods[targetName];
        food.gameObject.SetActive(status.Status == Status.TRACKED);
    }
}
