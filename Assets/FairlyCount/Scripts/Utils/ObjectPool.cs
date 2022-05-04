using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance
    {
        get
        {
            return _instance;
        }
    }
    private static ObjectPool _instance;

    private static Dictionary<string, Queue<GameObject>> GoPools = new Dictionary<string, Queue<GameObject>>();
    private static Dictionary<string, List<GameObject>> GoUnPools = new Dictionary<string, List<GameObject>>();


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        _instance = this;
    }

    /// <summary>
    /// Méthode de Spawn locale pour les GameObject classiques
    /// </summary>
    /// <param name="toSpawnPrefab"></param>
    /// <param name="position"></param>
    /// <param name="parent"></param>
    /// <param name="spawnData"></param>
    /// <returns></returns>
    public GameObject SpawnGo(GameObject toSpawnPrefab, Vector3 position, Transform parent = null)
    {
        GameObject spawned = null;

        // Recherche dans les pools si une instance est disponible ou création
        if (GoPools.ContainsKey(toSpawnPrefab.name))
        {
            if (GoPools[toSpawnPrefab.name].Count > 0)
            {
                spawned = GoPools[toSpawnPrefab.name].Dequeue();
            }

            if (spawned == null)
            {
                spawned = Instantiate(toSpawnPrefab, parent == null ? transform : parent);
            }
        }
        else
        {
            GoPools.Add(toSpawnPrefab.name, new Queue<GameObject>());
            spawned = Instantiate(toSpawnPrefab, parent == null ? transform : parent);
        }

        // Stockage des objets en cours d'utilisation dans les Unpools

        if (!GoUnPools.ContainsKey(toSpawnPrefab.name))
        {
            GoUnPools.Add(toSpawnPrefab.name, new List<GameObject>());
        }
        GoUnPools[toSpawnPrefab.name].Add(spawned);

        // Positionnement de l'objet et activation
        if (position != new Vector3(-1, -1, -1))
        {
            spawned.transform.position = position;
        }
        spawned.gameObject.SetActive(true);
        
        return spawned;
    }

    public void DespawnGo(GameObject toDespawn)
    {
        toDespawn.SetActive(false);

        if (GoUnPools.ContainsKey(toDespawn.name))
        {
            GoUnPools[toDespawn.name].Remove(toDespawn);
        }

        if (GoPools.ContainsKey(toDespawn.name))
        {
            GoPools[toDespawn.name].Enqueue(toDespawn);
        }
        else
        {
            GoPools.Add(toDespawn.name, new Queue<GameObject>());
            GoPools[toDespawn.name].Enqueue(toDespawn);
        }
    }
}
