using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public SerializedDictionary<string, Queue<GameObject>> poolDictionary = new SerializedDictionary<string, Queue<GameObject>>();
    public ItemData[] pickups;
    public ItemManager objectPool;

    private void Start()
    {
        foreach (ItemData pickup in pickups)
        {
            CreatePool(pickup.itemId.ToString(), pickup.relatedGameObject, 10);
        }
    }
    public void CreatePool(string key, GameObject prefab, int size)
    {
        if (!poolDictionary.ContainsKey(key))
        {
            poolDictionary[key] = new Queue<GameObject>();
            for (int i = 0; i < size; i++)
            {
                GameObject obj = Instantiate(prefab);
                obj.SetActive(false);
                poolDictionary[key].Enqueue(obj);
            }
        }
    }

    public GameObject GetFromPool(string key)
    {
        if (poolDictionary.ContainsKey(key) && poolDictionary[key].Count > 0)
        {
            GameObject obj = poolDictionary[key].Dequeue();
            obj.SetActive(true);
            return obj;
        }

        Debug.LogWarning($"No hay objetos disponibles en la pool de {key}");
        return null;
    }

    public void ReturnToPool(string key, GameObject obj)
    {
        obj.SetActive(false);
        if (poolDictionary.ContainsKey(key))
        {
            poolDictionary[key].Enqueue(obj);
        }
    }
}
