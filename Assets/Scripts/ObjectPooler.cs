using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{

    // The pool
    // Key: a unique name generated from the prefab name
    // Value: a list of instances of this prefab
    private static Dictionary<string, Stack<GameObject>> pool = new Dictionary<string, Stack<GameObject>>();

    // A word added at the end of the objects instances name to know they are handled by the pool.
    // It's just for the user, not useful for the pool to work and can be left empty.
    private const string instancesOptionalNameEnding = "(Pool)";

    /// <summary>
    /// Creates <paramref name="number"/> instances of the <paramref name="prefab"/>, deactivates them and adds them to the pool.
    /// The new objects will be parented by <paramref name="parent"/> if not null.
    /// </summary>
    public static void PreLoadInstances(GameObject prefab, int number, Transform parent = null)
    {
        GameObject prefabInstance;
        bool setParent = parent != null;
        for (int i = 0; i < number; i++)
        {
            prefabInstance = Instantiate(prefab);
            if (setParent)
                prefabInstance.transform.SetParent(parent,false);

            StoreInstance(prefabInstance);
        }
    }

    /// <summary>
    /// Returns an instance of the <paramref name="prefab"/>. The returned gameobject is active.
    /// The instance will be parented by <paramref name="parent"/> if not null.
    /// </summary>
    public static GameObject GetInstance(GameObject prefab, Transform parent = null)
    {
        GameObject prefabInstance = GetInstanceFromPool(prefab);
        if (prefabInstance == null)
        {
            prefabInstance = Instantiate(prefab);
        }

        GameObject gameObjectInstance = prefabInstance.gameObject;
        gameObjectInstance.SetActive(true);

        if (parent != null)
            gameObjectInstance.transform.SetParent(parent,false);

        return gameObjectInstance;
    }

 
    public static void StoreInstance(GameObject gameObjectInstance)
    {
        gameObjectInstance.gameObject.SetActive(false);
        Stack<GameObject> instancesList;
        if (pool.TryGetValue(gameObjectInstance.name, out instancesList))
        {
            instancesList.Push(gameObjectInstance);
        }
        else
        {
            instancesList = new Stack<GameObject>();
            instancesList.Push(gameObjectInstance);
            pool.Add(gameObjectInstance.name, instancesList);
        }
    }

    public static void Clear()
    {
        pool.Clear();
    }

    
    private static GameObject GetInstanceFromPool(GameObject prefab)
    {
        GameObject prefabInstance = null;
        Stack<GameObject> instancesList;
        if (pool.TryGetValue(GeneratePrefabInstancesName(prefab), out instancesList))
        {
            if (instancesList.Count != 0)
            {
                prefabInstance = instancesList.Pop();
                //instancesList.RemoveAt(0);
            }
        }

        return prefabInstance;
    }

    
    private static GameObject Instantiate(GameObject prefab)
    {
        GameObject prefabInstance = Object.Instantiate(prefab);
        prefabInstance.name = GeneratePrefabInstancesName(prefab);
        return prefabInstance;
    }

   
    private static string GeneratePrefabInstancesName(GameObject prefab)
    {
        return prefab.name + prefab.GetInstanceID() + instancesOptionalNameEnding;
    }
}